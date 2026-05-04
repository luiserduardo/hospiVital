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

        // Espacio reservado para el futuro módulo de viales vencidos.
        // La lista queda creada, pero sin lógica ni carga de datos automática.
        public listaEnlazadaVialesVencidos VialesVencidos { get; } = new listaEnlazadaVialesVencidos();

        private readonly servicioCompatibilidad compatibilidad = new servicioCompatibilidad();

        private baseDatosInterna()
        {
            CargarUsuariosIniciales();
            CargarDatosIniciales();
        }

        public AppUser? ValidarUsuario(string usuario, string contrasena)
        {
            return Usuarios.validarCredenciales(usuario, contrasena);
        }

        public listaEnlazadaUsuarios ObtenerUsuarios()
        {
            return Usuarios;
        }

        public bool CrearUsuario(AppUser usuario)
        {
            if (usuario == null)
            {
                return false;
            }

            usuario.Usuario = (usuario.Usuario ?? string.Empty).Trim();
            usuario.Contrasena = (usuario.Contrasena ?? string.Empty).Trim();
            usuario.Nombre = (usuario.Nombre ?? string.Empty).Trim();
            usuario.Rol = NormalizarRol(usuario.Rol);
            usuario.EsAdministradorPrincipal = false;

            if (string.IsNullOrWhiteSpace(usuario.Usuario) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena) ||
                string.IsNullOrWhiteSpace(usuario.Nombre))
            {
                return false;
            }

            if (Usuarios.buscarPorUsuario(usuario.Usuario) != null)
            {
                return false;
            }

            Usuarios.insertarFinal(usuario);
            return true;
        }

        public bool ActualizarUsuario(string usuarioOriginal, AppUser actualizado)
        {
            if (actualizado == null || string.IsNullOrWhiteSpace(usuarioOriginal))
            {
                return false;
            }

            actualizado.Usuario = (actualizado.Usuario ?? string.Empty).Trim();
            actualizado.Contrasena = (actualizado.Contrasena ?? string.Empty).Trim();
            actualizado.Nombre = (actualizado.Nombre ?? string.Empty).Trim();
            actualizado.Rol = NormalizarRol(actualizado.Rol);

            if (string.IsNullOrWhiteSpace(actualizado.Usuario) ||
                string.IsNullOrWhiteSpace(actualizado.Contrasena) ||
                string.IsNullOrWhiteSpace(actualizado.Nombre))
            {
                return false;
            }

            AppUser? duplicado = Usuarios.buscarPorUsuario(actualizado.Usuario);
            if (duplicado != null && !usuarioOriginal.Equals(actualizado.Usuario, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return Usuarios.actualizar(usuarioOriginal.Trim(), actualizado);
        }

        public bool EliminarUsuario(string usuario)
        {
            AppUser? existente = Usuarios.buscarPorUsuario(usuario);

            if (existente == null)
            {
                return false;
            }

            // Única restricción solicitada: no eliminar el administrador principal inicial.
            if (existente.EsAdministradorPrincipal ||
                (existente.Usuario.Equals("admin", StringComparison.OrdinalIgnoreCase) && existente.Contrasena == "1234"))
            {
                return false;
            }

            return Usuarios.eliminar(usuario);
        }

        public Donante RegistrarDonante(Donante donante)
        {
            Donante? existente = RegistroDonantes.buscarPorDui(donante.Dui);

            if (existente != null)
            {
                existente.Nombre = donante.Nombre;
                existente.Apellido = donante.Apellido;
                existente.Telefono = donante.Telefono;

                if (donante.Peso > 0)
                {
                    existente.Peso = donante.Peso;
                }

                return existente;
            }

            if (donante.IdDonante <= 0)
            {
                donante.IdDonante = RegistroDonantes.Total + 1;
            }

            RegistroDonantes.insertarFinal(donante);
            return donante;
        }

        public Donante RegistrarDonante(string nombre, string apellido, string dui, string telefono, double peso = 0)
        {
            Donante donante = new Donante
            {
                Nombre = nombre,
                Apellido = apellido,
                Dui = dui,
                Telefono = telefono,
                Peso = peso
            };

            return RegistrarDonante(donante);
        }

        public bool AgregarUnidad(unidadDeSangre unidad)
        {
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.IdUnidad))
            {
                return false;
            }

            unidad.IdUnidad = unidad.IdUnidad.Trim();

            // Una unidad de sangre es única: no se puede registrar dos veces
            // ni volver a ingresar una unidad que ya fue asignada a un receptor.
            if (Inventario.buscarUnidad(unidad.IdUnidad) != null ||
                AsignacionesRealizadas.existeUnidadAsignada(unidad.IdUnidad))
            {
                return false;
            }

            if (unidad.Donante != null)
            {
                unidad.Donante = RegistrarDonante(unidad.Donante);
            }

            return Inventario.agregarUnidad(unidad);
        }

        public receptorAsignacion RegistrarPacienteYAsignar(paciente nuevoPaciente, string? idUnidadSeleccionada = null)
        {
            if (nuevoPaciente == null)
            {
                return CrearAsignacionPendiente(new paciente());
            }

            RegistroPacientes.insertarFinal(nuevoPaciente);
            receptorAsignacion resultado = AsignarUnidadCompatible(nuevoPaciente, idUnidadSeleccionada);
            AsignacionesRealizadas.insertarFinal(resultado);
            return resultado;
        }

        public receptorAsignacion AsignarUnidadCompatible(paciente paciente, string? idUnidadSeleccionada = null)
        {
            if (paciente == null || Inventario.estaVacio() || string.IsNullOrWhiteSpace(idUnidadSeleccionada))
            {
                return CrearAsignacionPendiente(paciente);
            }

            // La asignación ya no se hace automáticamente.
            // Solo se asigna el vial que el usuario seleccionó explícitamente en el formulario.
            string idUnidad = idUnidadSeleccionada.Trim();

            if (AsignacionesRealizadas.existeUnidadAsignada(idUnidad))
            {
                return CrearAsignacionPendiente(paciente);
            }

            unidadDeSangre? unidadSeleccionada = Inventario.buscarUnidad(idUnidad);

            if (UnidadDisponibleYCompatible(unidadSeleccionada, paciente))
            {
                return CrearAsignacionCompletada(paciente, unidadSeleccionada!);
            }

            return CrearAsignacionPendiente(paciente);
        }

        public string? ValidarUnidadSeleccionadaParaReceptor(string? idUnidadSeleccionada, string tipoSangre, string factorRh)
        {
            string id = (idUnidadSeleccionada ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                return "Debe seleccionar un vial compatible antes de registrar al receptor.";
            }

            if (AsignacionesRealizadas.existeUnidadAsignada(id))
            {
                return "Este vial ya fue asignado a otro receptor. Cada unidad de sangre solo puede entregarse una vez.";
            }

            unidadDeSangre? unidad = Inventario.buscarUnidad(id);

            if (unidad == null)
            {
                return "El vial seleccionado ya no se encuentra en el inventario. Actualice la selección.";
            }

            if (unidad.estaVencida() || unidad.EstadoUnidad != "Disponible")
            {
                return "El vial seleccionado está vencido o no está disponible. Seleccione otro vial seguro.";
            }

            if (!compatibilidad.esCompatible(unidad, tipoSangre, factorRh))
            {
                return "El vial seleccionado no coincide con el tipo de sangre y factor Rh del receptor.";
            }

            return null;
        }

        public listaEnlazadaUnidadesCompatibles ObtenerCompatiblesPorTipo(string tipoSangre, string factorRh)
        {
            string tipo = (tipoSangre ?? string.Empty).Trim().ToUpper();
            string rh = (factorRh ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(tipo) || string.IsNullOrWhiteSpace(rh))
            {
                return new listaEnlazadaUnidadesCompatibles();
            }

            // Se filtra recorriendo los nodos del inventario, sin List<T>.
            // La cola de prioridad del inventario no se modifica; solamente se consulta desde el frente.
            return compatibilidad.filtrarCompatibles(Inventario.obtenerFrente(), tipo, rh, AsignacionesRealizadas);
        }

        private bool UnidadDisponibleYCompatible(unidadDeSangre? unidad, paciente paciente)
        {
            return unidad != null &&
                   compatibilidad.esCompatible(unidad, paciente) &&
                   !unidad.estaVencida() &&
                   unidad.EstadoUnidad == "Disponible";
        }

        private receptorAsignacion CrearAsignacionCompletada(paciente paciente, unidadDeSangre unidadAsignada)
        {
            // La unidad NO se elimina del inventario.
            // Se mantiene en la misma TAD colaPrioridadViales como registro histórico compartido
            // para que médico y asistente vean el mismo ID de unidad.
            // Al cambiar su estado a Asignada, deja de aparecer como opción para nuevos receptores.
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

        private string NormalizarRol(string rol)
        {
            return rol switch
            {
                AppRoles.AsistenteMedico => AppRoles.AsistenteMedico,
                AppRoles.Medico => AppRoles.Medico,
                AppRoles.Admin => AppRoles.Admin,
                _ => AppRoles.AsistenteMedico
            };
        }

        public bool AgregarVialVencido(unidadDeSangre unidad)
        {
            if (unidad == null || string.IsNullOrWhiteSpace(unidad.IdUnidad))
                return false;

            VialesVencidos.insertarFinal(unidad);
            return true;
        }
        private void CargarUsuariosIniciales()
        {
            Usuarios.insertarFinal(new AppUser
            {
                Usuario = "asistente",
                Contrasena = "1234",
                Nombre = "Asistente Médico",
                Rol = AppRoles.AsistenteMedico
            });

            Usuarios.insertarFinal(new AppUser
            {
                Usuario = "medico",
                Contrasena = "1234",
                Nombre = "Médico",
                Rol = AppRoles.Medico
            });

            Usuarios.insertarFinal(new AppUser
            {
                Usuario = "admin",
                Contrasena = "1234",
                Nombre = "Administrador",
                Rol = AppRoles.Admin,
                EsAdministradorPrincipal = true
            });
        }

        private void CargarDatosIniciales()
        {
            Donante d1 = RegistrarDonante(new Donante(1, "Ana", "Martínez", "05123456-7", "7123-4567") { Peso = 58 });
            Donante d2 = RegistrarDonante(new Donante(2, "Carlos", "Pérez", "03456789-2", "7234-5678") { Peso = 72 });
            Donante d3 = RegistrarDonante(new Donante(3, "Elena", "Rodríguez", "02345671-5", "7345-6789") { Peso = 64 });
            Donante d4 = RegistrarDonante(new Donante(4, "Juan", "López", "01987654-3", "7456-7890") { Peso = 80 });
            Donante d5 = RegistrarDonante(new Donante(5, "María", "García", "05123456-1", "7100-9988") { Peso = 62 });

            DateTime hoy = DateTime.Today;

            // Viales iniciales asignados explícitamente a los receptores de muestra.
            // Esto evita que la pantalla inicial aparezca con registros pendientes,
            // sin cambiar la regla del formulario: los nuevos receptores deben seleccionar un vial manualmente.
            AgregarUnidad(new unidadDeSangre(
                "HP20260001",
                "A",
                "+",
                hoy.AddDays(-2),
                hoy.AddDays(20),
                "Disponible",
                1,
                d1
            ));

            AgregarUnidad(new unidadDeSangre(
                "HP20260002",
                "B",
                "-",
                hoy.AddDays(-2),
                hoy.AddDays(20),
                "Disponible",
                1,
                d2
            ));

            AgregarUnidad(new unidadDeSangre(
                "HP20260003",
                "O",
                "+",
                hoy.AddDays(-2),
                hoy.AddDays(20),
                "Disponible",
                1,
                d3
            ));

            Random rnd = new Random(2026);

            string[] grupos = { "A", "B", "O", "AB" };
            string[] factores = { "+", "-" };
            Donante[] donantes = { d1, d2, d3, d4, d5 };

            for (int i = 1; i <= 100; i++)
            {
                string grupo = grupos[rnd.Next(grupos.Length)];
                string factor = factores[rnd.Next(factores.Length)];
                int diasOffset = rnd.Next(-10, 20);
                DateTime fechaCadu = hoy.AddDays(diasOffset);
                string estado = fechaCadu < hoy ? "Vencido" : "Disponible";
                Donante donanteAzar = donantes[rnd.Next(donantes.Length)];

                AgregarUnidad(new unidadDeSangre(
                    $"HP2026{1000 + i}",
                    grupo,
                    factor,
                    hoy.AddDays(-5),
                    fechaCadu,
                    estado,
                    1,
                    donanteAzar
                ));
            }


            // 50 viales vencidos precargados en la lista de viales vencidos
            Donante[] donantesV = { d1, d2, d3, d4, d5 };
            string[] gruposV = { "A", "B", "O", "AB" };
            string[] factoresV = { "+", "-" };
            Random rndV = new Random(999);

            for (int i = 1; i <= 50; i++)
            {
                string grupo = gruposV[rndV.Next(gruposV.Length)];
                string factor = factoresV[rndV.Next(factoresV.Length)];
                int diasVencido = rndV.Next(1, 60); // vencido entre 1 y 60 días atrás
                DateTime fechaCadu = hoy.AddDays(-diasVencido);
                Donante donanteAzar = donantesV[rndV.Next(donantesV.Length)];

                unidadDeSangre vialVencido = new unidadDeSangre(
                    $"HP2026{1000 + i}",
                    grupo,
                    factor,
                    hoy.AddDays(-diasVencido - 30),
                    fechaCadu,
                    "Vencido",
                    450,
                    donanteAzar
                );

                AgregarVialVencido(vialVencido);
            }
            RegistrarPacienteYAsignar(
                new paciente("06123456-7", "José", "López", "A", "+"),
                "HP20260001"
            );

            RegistrarPacienteYAsignar(
                new paciente("06234567-8", "Elena", "Rivas", "B", "-"),
                "HP20260002"
            );

            RegistrarPacienteYAsignar(
                new paciente("06345678-9", "Roberto", "Sosa", "O", "+"),
                "HP20260003"
            );
        }
    }
}
