using System.Globalization;

namespace HospiVital_App.Models
{
    // Archivo central para manejar los datos internos del sistema.
    public class baseDatosInterna
    {
        private static readonly Lazy<baseDatosInterna> instancia = new(() => new baseDatosInterna());

        public static baseDatosInterna Instancia => instancia.Value;

        public inventario Inventario { get; } = new inventario();
        public listaEnlazadaPacientes RegistroPacientes { get; } = new listaEnlazadaPacientes();
        public listaEnlazadaDonantes RegistroDonantes { get; } = new listaEnlazadaDonantes();
        public listaEnlazadaReceptorAsignacion AsignacionesRealizadas { get; } = new listaEnlazadaReceptorAsignacion();
        public listaEnlazadaUsuarios Usuarios { get; } = new listaEnlazadaUsuarios();
        public listaEnlazadaVialesVencidos VialesVencidos { get; } = new listaEnlazadaVialesVencidos();

        // Pila LIFO: registra en orden cronológico inverso las salidas de viales vencidos.
        // Solo se apila cuando un responsable confirma la baja del vial del sistema.
        public pilaSalidasVialesVencidos SalidasVialesVencidos { get; } = new pilaSalidasVialesVencidos();

        private readonly servicioCompatibilidad compatibilidad = new servicioCompatibilidad();

        private baseDatosInterna()
        {
            CargarUsuariosIniciales();
            CargarDatosIniciales();
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GESTIÓN DE USUARIOS
        // ────────────────────────────────────────────────────────────────────────────

        public AppUser? ValidarUsuario(string usuario, string contrasena)
            => Usuarios.validarCredenciales(usuario, contrasena);

        public listaEnlazadaUsuarios ObtenerUsuarios() => Usuarios;

        public bool CrearUsuario(AppUser usuario)
        {
            if (usuario == null) return false;

            usuario.Usuario = (usuario.Usuario ?? string.Empty).Trim();
            usuario.Contrasena = (usuario.Contrasena ?? string.Empty).Trim();
            usuario.Nombre = (usuario.Nombre ?? string.Empty).Trim();
            usuario.Rol = NormalizarRol(usuario.Rol);
            usuario.EsAdministradorPrincipal = false;

            if (string.IsNullOrWhiteSpace(usuario.Usuario) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena) ||
                string.IsNullOrWhiteSpace(usuario.Nombre))
                return false;

            if (Usuarios.buscarPorUsuario(usuario.Usuario) != null) return false;

            Usuarios.insertarFinal(usuario);
            return true;
        }

        public bool ActualizarUsuario(string usuarioOriginal, AppUser actualizado)
        {
            if (actualizado == null || string.IsNullOrWhiteSpace(usuarioOriginal)) return false;

            actualizado.Usuario = (actualizado.Usuario ?? string.Empty).Trim();
            actualizado.Contrasena = (actualizado.Contrasena ?? string.Empty).Trim();
            actualizado.Nombre = (actualizado.Nombre ?? string.Empty).Trim();
            actualizado.Rol = NormalizarRol(actualizado.Rol);

            if (string.IsNullOrWhiteSpace(actualizado.Usuario) ||
                string.IsNullOrWhiteSpace(actualizado.Contrasena) ||
                string.IsNullOrWhiteSpace(actualizado.Nombre))
                return false;

            AppUser? duplicado = Usuarios.buscarPorUsuario(actualizado.Usuario);
            if (duplicado != null && !usuarioOriginal.Equals(actualizado.Usuario, StringComparison.OrdinalIgnoreCase))
                return false;

            return Usuarios.actualizar(usuarioOriginal.Trim(), actualizado);
        }

        public bool EliminarUsuario(string usuario)
        {
            AppUser? existente = Usuarios.buscarPorUsuario(usuario);
            if (existente == null) return false;

            // Restricción: no eliminar el administrador principal inicial.
            if (existente.EsAdministradorPrincipal ||
                (existente.Usuario.Equals("admin", StringComparison.OrdinalIgnoreCase) && existente.Contrasena == "1234"))
                return false;

            return Usuarios.eliminar(usuario);
        }

        private string NormalizarRol(string rol) => rol switch
        {
            AppRoles.AsistenteMedico => AppRoles.AsistenteMedico,
            AppRoles.Medico => AppRoles.Medico,
            AppRoles.Admin => AppRoles.Admin,
            _ => AppRoles.AsistenteMedico
        };

        // ────────────────────────────────────────────────────────────────────────────
        // GESTIÓN DE DONANTES
        // ────────────────────────────────────────────────────────────────────────────

        public Donante RegistrarDonante(Donante donante)
        {
            Donante? existente = RegistroDonantes.buscarPorDui(donante.Dui);

            if (existente != null)
            {
                existente.Nombre = donante.Nombre;
                existente.Apellido = donante.Apellido;
                existente.Telefono = donante.Telefono;
                if (donante.Peso > 0) existente.Peso = donante.Peso;
                return existente;
            }

            if (donante.IdDonante <= 0)
                donante.IdDonante = RegistroDonantes.Total + 1;

            RegistroDonantes.insertarFinal(donante);
            return donante;
        }

        public Donante RegistrarDonante(string nombre, string apellido, string dui, string telefono, double peso = 0)
            => RegistrarDonante(new Donante { Nombre = nombre, Apellido = apellido, Dui = dui, Telefono = telefono, Peso = peso });

        // ────────────────────────────────────────────────────────────────────────────
        // GESTIÓN DE INVENTARIO
        // ────────────────────────────────────────────────────────────────────────────

        public bool AgregarUnidad(unidadDeSangre unidad)
        {
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.IdUnidad)) return false;

            unidad.IdUnidad = unidad.IdUnidad.Trim();

            // Una unidad de sangre es única: no puede registrarse dos veces
            // ni volver a ingresar si ya fue asignada a un receptor.
            if (Inventario.buscarUnidad(unidad.IdUnidad) != null ||
                AsignacionesRealizadas.existeUnidadAsignada(unidad.IdUnidad))
                return false;

            if (unidad.Donante != null)
                unidad.Donante = RegistrarDonante(unidad.Donante);

            return Inventario.agregarUnidad(unidad);
        }

        // ── Viales vencidos ────────────────────────────────────────────────────────

        // Agrega un vial a la lista de vencidos.
        // Rechaza el vial si su ID ya existe en la lista, evitando duplicados
        // que podrían confundir el registro de salidas de la pila.
        public bool AgregarVialVencido(unidadDeSangre unidad)
        {
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.IdUnidad))
                return false;

            // Guardia contra duplicados: misma protección que AgregarUnidad aplica
            // en el inventario. Sin esto, DepurarVialesVencidos podría insertar
            // un ID que ya existe en la lista si el módulo se invoca varias veces.
            if (BuscarVialVencido(unidad.IdUnidad) != null)
                return false;

            VialesVencidos.insertarFinal(unidad);
            return true;
        }

        // Mueve al historial de vencidos todos los viales del inventario cuya
        // fecha de caducidad ya expiró. Devuelve la cantidad de viales depurados.
        public int DepurarVialesVencidos()
        {
            IEnumerable<unidadDeSangre> vencidos = Inventario.depurarVencidos();
            int contador = 0;

            foreach (unidadDeSangre vial in vencidos)
            {
                // AgregarVialVencido ya ignora duplicados, así que es seguro llamarlo
                // aunque DepurarVialesVencidos se ejecute más de una vez.
                if (AgregarVialVencido(vial))
                    contador++;
            }

            return contador;
        }

        // Devuelve los viales vencidos que aún no tienen registro de salida en la pila.
        public IEnumerable<unidadDeSangre> ObtenerVialesVencidosPendientesSalida()
        {
            nodoVialVencido? actual = VialesVencidos.Cabeza;

            while (actual != null)
            {
                if (actual.Dato != null &&
                    !SalidasVialesVencidos.existeRegistro(actual.Dato.IdUnidad))
                {
                    yield return actual.Dato;
                }

                actual = actual.Siguiente;
            }
        }

        // Registra la baja formal de un vial vencido apilando un registroSalidaVialVencido
        // en la pila LIFO. Devuelve false si el vial no existe en la lista o ya fue dado de baja.
        public bool RegistrarSalidaVialVencido(string idUnidad, string responsable)
        {
            unidadDeSangre? vial = BuscarVialVencido(idUnidad);

            // El vial debe existir en la lista de vencidos y no haber sido
            // dado de baja previamente (existeRegistro consulta la pila).
            if (vial == null || SalidasVialesVencidos.existeRegistro(vial.IdUnidad))
                return false;

            string usuarioResponsable = string.IsNullOrWhiteSpace(responsable)
                ? "Administrador"
                : responsable.Trim();

            registroSalidaVialVencido registro = new registroSalidaVialVencido(
                vial,
                DateTime.Now,
                usuarioResponsable);

            return SalidasVialesVencidos.apilar(registro);
        }

        // Búsqueda interna por ID en la lista enlazada de viales vencidos.
        private unidadDeSangre? BuscarVialVencido(string idUnidad)
        {
            if (string.IsNullOrWhiteSpace(idUnidad)) return null;

            string id = idUnidad.Trim();
            nodoVialVencido? actual = VialesVencidos.Cabeza;

            while (actual != null)
            {
                if (actual.Dato != null &&
                    actual.Dato.IdUnidad.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return actual.Dato;

                actual = actual.Siguiente;
            }

            return null;
        }

        // ────────────────────────────────────────────────────────────────────────────
        // GESTIÓN DE PACIENTES Y ASIGNACIONES
        // ────────────────────────────────────────────────────────────────────────────

        public receptorAsignacion RegistrarPacienteYAsignar(paciente nuevoPaciente, string? idUnidadSeleccionada = null)
        {
            if (nuevoPaciente == null) return CrearAsignacionPendiente(new paciente());

            RegistroPacientes.insertarFinal(nuevoPaciente);
            receptorAsignacion resultado = AsignarUnidadCompatible(nuevoPaciente, idUnidadSeleccionada);
            AsignacionesRealizadas.insertarFinal(resultado);
            return resultado;
        }

        public receptorAsignacion AsignarUnidadCompatible(paciente paciente, string? idUnidadSeleccionada = null)
        {
            if (paciente == null || Inventario.estaVacio() || string.IsNullOrWhiteSpace(idUnidadSeleccionada))
                return CrearAsignacionPendiente(paciente);

            // Solo se asigna el vial que el usuario seleccionó explícitamente.
            string idUnidad = idUnidadSeleccionada.Trim();

            if (AsignacionesRealizadas.existeUnidadAsignada(idUnidad))
                return CrearAsignacionPendiente(paciente);

            unidadDeSangre? unidadSeleccionada = Inventario.buscarUnidad(idUnidad);

            return UnidadDisponibleYCompatible(unidadSeleccionada, paciente)
                ? CrearAsignacionCompletada(paciente, unidadSeleccionada!)
                : CrearAsignacionPendiente(paciente);
        }

        public string? ValidarUnidadSeleccionadaParaReceptor(string? idUnidadSeleccionada, string tipoSangre, string factorRh)
        {
            string id = (idUnidadSeleccionada ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(id))
                return "Debe seleccionar un vial compatible antes de registrar al receptor.";

            if (AsignacionesRealizadas.existeUnidadAsignada(id))
                return "Este vial ya fue asignado a otro receptor. Cada unidad de sangre solo puede entregarse una vez.";

            unidadDeSangre? unidad = Inventario.buscarUnidad(id);

            if (unidad == null)
                return "El vial seleccionado ya no se encuentra en el inventario. Actualice la selección.";

            if (unidad.estaVencida() || unidad.EstadoUnidad != "Disponible")
                return "El vial seleccionado está vencido o no está disponible. Seleccione otro vial seguro.";

            if (!compatibilidad.esCompatible(unidad, tipoSangre, factorRh))
                return "El vial seleccionado no coincide con el tipo de sangre y factor Rh del receptor.";

            return null;
        }

        public listaEnlazadaUnidadesCompatibles ObtenerCompatiblesPorTipo(string tipoSangre, string factorRh)
        {
            string tipo = (tipoSangre ?? string.Empty).Trim().ToUpper();
            string rh = (factorRh ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(tipo) || string.IsNullOrWhiteSpace(rh))
                return new listaEnlazadaUnidadesCompatibles();

            return compatibilidad.filtrarCompatibles(Inventario.obtenerFrente(), tipo, rh, AsignacionesRealizadas);
        }

        private bool UnidadDisponibleYCompatible(unidadDeSangre? unidad, paciente paciente)
            => unidad != null &&
               compatibilidad.esCompatible(unidad, paciente) &&
               !unidad.estaVencida() &&
               unidad.EstadoUnidad == "Disponible";

        private receptorAsignacion CrearAsignacionCompletada(paciente paciente, unidadDeSangre unidadAsignada)
        {
            // La unidad NO se elimina del inventario; se marca como "Asignada"
            // para que deje de aparecer como opción sin perder el registro histórico.
            unidadAsignada.EstadoUnidad = "Asignada";
            DateTime fechaTransfusion = DateTime.Now;

            return new receptorAsignacion
            {
                IdReceptor = "REC-" + paciente.IdPaciente,
                IdPaciente = paciente.IdPaciente,
                DuiReceptor = paciente.Dui,
                Beneficiario = paciente.Nombre + " " + paciente.Apellido,
                Nombres = paciente.Nombre,
                Apellidos = paciente.Apellido,
                TipoSangre = paciente.TipoSangreRequerido + paciente.FactorRhRequerido,
                Donante = unidadAsignada.Donante.Nombre + " " + unidadAsignada.Donante.Apellido,
                DonanteDui = unidadAsignada.Donante.Dui,
                DonanteTelefono = unidadAsignada.Donante.Telefono,
                UnidadAsignada = unidadAsignada.IdUnidad,
                FechaTransfusion = fechaTransfusion
                    .ToString("dd MMM yyyy, HH:mm", new CultureInfo("es-ES"))
                    .Replace(".", ""),
                FechaTransfusionValor = fechaTransfusion,
                Estado = "Completado"
            };
        }

        private receptorAsignacion CrearAsignacionPendiente(paciente? paciente)
        {
            paciente ??= new paciente();

            return new receptorAsignacion
            {
                IdReceptor = "REC-" + paciente.IdPaciente,
                IdPaciente = paciente.IdPaciente,
                DuiReceptor = paciente.Dui,
                Beneficiario = paciente.Nombre + " " + paciente.Apellido,
                Nombres = paciente.Nombre,
                Apellidos = paciente.Apellido,
                TipoSangre = paciente.TipoSangreRequerido + paciente.FactorRhRequerido,
                Donante = "Sin asignar",
                DonanteDui = "-",
                DonanteTelefono = "-",
                UnidadAsignada = "Pendiente",
                FechaTransfusion = "Pendiente",
                Estado = "Pendiente"
            };
        }

        // ────────────────────────────────────────────────────────────────────────────
        // CARGA DE DATOS INICIALES
        // ────────────────────────────────────────────────────────────────────────────

        private void CargarUsuariosIniciales()
        {
            (string usuario, string contrasena, string nombre, string rol, bool esAdmin)[] usuarios =
            {
                ("asistente", "1234", "Asistente Médico", AppRoles.AsistenteMedico, false),
                ("medico",    "1234", "Médico",           AppRoles.Medico,          false),
                ("admin",     "1234", "Administrador",    AppRoles.Admin,           true ),
            };

            foreach (var u in usuarios)
            {
                Usuarios.insertarFinal(new AppUser
                {
                    Usuario = u.usuario,
                    Contrasena = u.contrasena,
                    Nombre = u.nombre,
                    Rol = u.rol,
                    EsAdministradorPrincipal = u.esAdmin
                });
            }
        }

        private void CargarDatosIniciales()
        {
            DateTime hoy = DateTime.Today;

            // ── 1. Donantes ──────────────────────────────────────────────────────────
            (int id, string nombre, string apellido, string dui, string telefono, double peso)[] infoDonantes =
            {
                (1, "Ana",    "Martínez",  "05123456-7", "7123-4567", 58),
                (2, "Carlos", "Pérez",     "03456789-2", "7234-5678", 72),
                (3, "Elena",  "Rodríguez", "02345671-5", "7345-6789", 64),
                (4, "Juan",   "López",     "01987654-3", "7456-7890", 80),
                (5, "María",  "García",    "05123456-1", "7100-9988", 62),
            };

            Donante[] donantes = new Donante[infoDonantes.Length];
            for (int i = 0; i < infoDonantes.Length; i++)
            {
                var d = infoDonantes[i];
                donantes[i] = RegistrarDonante(
                    new Donante(d.id, d.nombre, d.apellido, d.dui, d.telefono) { Peso = d.peso }
                );
            }

            // ── 2. Asignaciones garantizadas (sin estado Pendiente) ──────────────────
            // Prefijo HP2026A → rango exclusivo para pacientes de muestra.
            // Prefijo HP2026B → inventario general aleatorio (sección 3).
            // Prefijo HP2025V → viales vencidos precargados (sección 4).
            (string dui, string nombre, string apellido, string tipo, string factor, string idUnidad, int iDonante)[] asignaciones =
            {
                ("06123456-7", "José",      "López",     "A",  "+", "HP2026A001", 0),
                ("06234567-8", "Elena",     "Rivas",     "B",  "-", "HP2026A002", 1),
                ("06345678-9", "Roberto",   "Sosa",      "O",  "+", "HP2026A003", 2),
                ("07000001-1", "Luis",      "Hernández", "A",  "+", "HP2026A004", 0),
                ("07000002-2", "Carmen",    "Flores",    "B",  "-", "HP2026A005", 1),
                ("07000003-3", "Miguel",    "Castro",    "O",  "+", "HP2026A006", 2),
                ("07000004-4", "Andrea",    "Vargas",    "AB", "+", "HP2026A007", 3),
                ("07000005-5", "Pedro",     "Mendoza",   "A",  "-", "HP2026A008", 4),
                ("07000006-6", "Lucía",     "Ramírez",   "B",  "+", "HP2026A009", 0),
                ("07000007-7", "Jorge",     "Navarro",   "O",  "-", "HP2026A010", 1),
                ("07000008-8", "Sofía",     "Herrera",   "AB", "-", "HP2026A011", 2),
                ("07000009-9", "Raúl",      "Morales",   "A",  "+", "HP2026A012", 3),
                ("07000010-0", "Valeria",   "Ortega",    "B",  "-", "HP2026A013", 4),
                ("07000011-1", "Diego",     "Silva",     "O",  "+", "HP2026A014", 0),
                ("07000012-2", "Paola",     "Rojas",     "AB", "+", "HP2026A015", 1),
                ("07000013-3", "Fernando",  "Campos",    "A",  "-", "HP2026A016", 2),
                ("07000014-4", "Elisa",     "Cruz",      "B",  "+", "HP2026A017", 3),
                ("07000015-5", "Mario",     "Reyes",     "O",  "-", "HP2026A018", 4),
                ("07000016-6", "Gabriela",  "Pineda",    "AB", "-", "HP2026A019", 0),
                ("07000017-7", "Ricardo",   "Delgado",   "A",  "+", "HP2026A020", 1),
                ("07000018-8", "Natalia",   "Fuentes",   "B",  "-", "HP2026A021", 2),
                ("07000019-9", "Oscar",     "Aguilar",   "O",  "+", "HP2026A022", 3),
                ("07000020-0", "Daniela",   "Peña",      "AB", "+", "HP2026A023", 4),
                ("07000021-1", "Hugo",      "Salinas",   "A",  "-", "HP2026A024", 0),
                ("07000022-2", "Patricia",  "Mejía",     "B",  "+", "HP2026A025", 1),
                ("07000023-3", "Iván",      "Escobar",   "O",  "-", "HP2026A026", 2),
                ("07000024-4", "Claudia",   "Arias",     "AB", "-", "HP2026A027", 3),
                ("07000025-5", "Esteban",   "Benítez",   "A",  "+", "HP2026A028", 4),
                ("07000026-6", "Marisol",   "Núñez",     "B",  "-", "HP2026A029", 0),
                ("07000027-7", "Alberto",   "Linares",   "O",  "+", "HP2026A030", 1),
                ("07000028-8", "Karla",     "Molina",    "AB", "+", "HP2026A031", 2),
                ("07000029-9", "Tomás",     "Cáceres",   "A",  "-", "HP2026A032", 3),
                ("07000030-0", "Verónica",  "Segura",    "B",  "+", "HP2026A033", 4),
            };

            foreach (var a in asignaciones)
            {
                AgregarUnidad(new unidadDeSangre(
                    a.idUnidad,
                    a.tipo,
                    a.factor,
                    hoy.AddDays(-2),
                    hoy.AddDays(30),
                    "Disponible",
                    1,
                    donantes[a.iDonante]
                ));

                RegistrarPacienteYAsignar(
                    new paciente(a.dui, a.nombre, a.apellido, a.tipo, a.factor),
                    a.idUnidad
                );
            }

            // ── 3. Inventario general aleatorio (sin paciente asignado) ──────────────
            string[] grupos = { "A", "B", "O", "AB" };
            string[] factores = { "+", "-" };
            Random rnd = new Random(2026);

            for (int i = 1; i <= 100; i++)
            {
                string grupo = grupos[rnd.Next(grupos.Length)];
                string factor = factores[rnd.Next(factores.Length)];
                int diasOffset = rnd.Next(-10, 20);
                DateTime fechaCadu = hoy.AddDays(diasOffset);
                string estado = fechaCadu < hoy ? "Vencido" : "Disponible";
                Donante donante = donantes[rnd.Next(donantes.Length)];

                AgregarUnidad(new unidadDeSangre(
                    $"HP2026B{i:D3}",
                    grupo,
                    factor,
                    hoy.AddDays(-5),
                    fechaCadu,
                    estado,
                    1,
                    donante
                ));
            }

            // ── 4. Viales vencidos precargados en el historial de vencidos ───────────
            // Prefijo HP2025V es exclusivo: no colisiona con HP2026A ni HP2026B.
            // AgregarVialVencido rechaza duplicados, por lo que llamar a
            // DepurarVialesVencidos después no provoca entradas dobles.
            Random rndV = new Random(999);

            for (int i = 1; i <= 50; i++)
            {
                string grupo = grupos[rndV.Next(grupos.Length)];
                string factor = factores[rndV.Next(factores.Length)];
                int diasVencido = rndV.Next(1, 60);
                DateTime fechaCadu = hoy.AddDays(-diasVencido);
                Donante donante = donantes[rndV.Next(donantes.Length)];

                AgregarVialVencido(new unidadDeSangre(
                    $"HP2025V{i:D3}",
                    grupo,
                    factor,
                    hoy.AddDays(-diasVencido - 30),
                    fechaCadu,
                    "Vencido",
                    450,
                    donante
                ));
            }
        }
    }
}