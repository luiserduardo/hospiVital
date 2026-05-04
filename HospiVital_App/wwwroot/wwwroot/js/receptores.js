document.addEventListener("DOMContentLoaded", function () {
    const modalBackdrop = document.getElementById("receptorModalBackdrop");
    const btnAbrir = document.getElementById("btnAbrirReceptor");
    const btnCerrar = document.getElementById("btnCerrarReceptor");
    const btnCancelar = document.getElementById("btnCancelarReceptor");

    const detallesBackdrop = document.getElementById("receptorDetallesBackdrop");
    const btnCerrarDetalles = document.getElementById("btnCerrarDetallesReceptor");

    const duiInput = document.getElementById("receptorDui");
    const tipoSangre = document.getElementById("tipoSangreReceptor");
    const factorRh = document.getElementById("factorRhReceptor");
    const selectedUnitInput = document.getElementById("idUnidadSeleccionada");
    const filterChip = document.getElementById("receptorFilterChip");
    const searchInput = document.getElementById("receptoresSearch");

    const filterFecha = document.getElementById("filterFechaReceptor");
    const filterEstado = document.getElementById("filterEstadoReceptor");
    const filterTipoSangre = document.getElementById("filterTipoSangreReceptor");
    const filterFactorRh = document.getElementById("filterFactorRhReceptor");
    const bloodFilterButtons = document.querySelectorAll("[data-blood-filter]");
    const btnAplicarFiltros = document.getElementById("btnAplicarFiltrosReceptores");
    const btnLimpiarFiltros = document.getElementById("btnLimpiarFiltrosReceptores");

    const compatTableBody = document.getElementById("compatTableBody");
    const receptorForm = document.getElementById("receptorForm");

    const floatingMenu = document.getElementById("receptoresActionsFloatingMenu");
    const btnViewDetails = document.getElementById("btnViewReceptorDetails");

    const btnToggleFiltrosReceptores = document.getElementById("btnToggleFiltrosReceptores");
    const receptoresFilterPanel = document.getElementById("receptoresFilterPanel");

    let activeReceptorId = null;

    if (window.receptorServerMessage?.text) {
        mostrarAlerta(
            window.receptorServerMessage.icon || "info",
            window.receptorServerMessage.icon === "success" ? "Registro completado" : "Validación",
            window.receptorServerMessage.text
        );
    }

    function abrirModal() {
        receptorForm?.reset();
        receptorForm?.querySelectorAll(".is-invalid").forEach(x => x.classList.remove("is-invalid"));
        if (selectedUnitInput) selectedUnitInput.value = "";
        compatTableBody?.querySelectorAll(".select-unit-btn").forEach(x => x.classList.remove("active"));
        renderCompatibles([], "Seleccione tipo y Rh");
        modalBackdrop?.classList.add("open");
    }

    function cerrarModal() {
        modalBackdrop?.classList.remove("open");
    }

    function abrirDetalles() {
        detallesBackdrop?.classList.add("open");
    }

    function cerrarDetalles() {
        detallesBackdrop?.classList.remove("open");
    }

    function closeFloatingMenu() {
        activeReceptorId = null;
        floatingMenu?.classList.remove("open");
    }

    function openFloatingMenu(button, id) {
        if (!floatingMenu) return;

        const rect = button.getBoundingClientRect();

        activeReceptorId = id;

        floatingMenu.style.display = "block";
        floatingMenu.style.visibility = "hidden";
        floatingMenu.style.left = "0px";
        floatingMenu.style.top = "0px";

        const menuWidth = floatingMenu.offsetWidth;
        const menuHeight = floatingMenu.offsetHeight;

        const gap = 8;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let left = rect.right + gap;

        if (left + menuWidth > viewportWidth - 12) {
            left = rect.left - menuWidth - gap;
        }

        if (left < 12) left = 12;

        let top = rect.top + (rect.height / 2) - (menuHeight / 2);

        if (top < 12) top = 12;
        if (top + menuHeight > viewportHeight - 12) {
            top = viewportHeight - menuHeight - 12;
        }

        floatingMenu.style.left = `${left}px`;
        floatingMenu.style.top = `${top}px`;
        floatingMenu.style.display = "";
        floatingMenu.style.visibility = "";
        floatingMenu.classList.add("open");
    }

    function formatearDui(valor) {
        const numeros = valor.replace(/\D/g, "").slice(0, 9);
        if (numeros.length <= 8) return numeros;
        return numeros.slice(0, 8) + "-" + numeros.slice(8);
    }

    function mostrarAlerta(icon, title, text) {
        if (window.Swal) {
            return Swal.fire({
                icon,
                title,
                text,
                confirmButtonColor: "#e61c5d",
                customClass: {
                    popup: "rounded-4"
                }
            });
        }

        alert(text);
        return Promise.resolve();
    }

    function marcarInvalido(input, invalido) {
        if (!input) return;
        input.classList.toggle("is-invalid", invalido);
    }

    function nombrePersonaValido(valor) {
        return /^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñÜü]+)*$/.test(valor);
    }

    function tipoSangreValido(valor) {
        return ["A", "B", "AB", "O"].includes((valor || "").toUpperCase());
    }

    function factorRhValido(valor) {
        return valor === "+" || valor === "-";
    }

    function duiYaRegistrado(dui) {
        return Array.from(document.querySelectorAll("[data-receptor-dui]"))
            .some(row => (row.dataset.receptorDui || "").trim().toLowerCase() === dui.trim().toLowerCase());
    }

    function validarFormularioReceptor(e) {
        if (!receptorForm) return;

        const nombre = receptorForm.querySelector("[name='nombre']");
        const apellido = receptorForm.querySelector("[name='apellido']");
        const tipo = receptorForm.querySelector("[name='tipoSangreRequerido']");
        const rh = receptorForm.querySelector("[name='factorRhRequerido']");
        const duiValor = (duiInput?.value || "").trim();
        const nombreValor = (nombre?.value || "").replace(/\s+/g, " ").trim();
        const apellidoValor = (apellido?.value || "").replace(/\s+/g, " ").trim();

        if (nombre) nombre.value = nombreValor;
        if (apellido) apellido.value = apellidoValor;

        [duiInput, nombre, apellido, tipo, rh].forEach(input => marcarInvalido(input, false));

        const camposVacios = [
            { input: duiInput, valor: duiValor },
            { input: nombre, valor: nombreValor },
            { input: apellido, valor: apellidoValor },
            { input: tipo, valor: tipo?.value },
            { input: rh, valor: rh?.value }
        ].filter(campo => !campo.valor || campo.valor.trim() === "");

        if (camposVacios.length > 0) {
            e.preventDefault();
            camposVacios.forEach(campo => marcarInvalido(campo.input, true));
            mostrarAlerta("warning", "Campos vacíos", "Complete DUI, nombres, apellidos, tipo de sangre y factor Rh.");
            return;
        }

        const duiRegex = /^\d{8}-\d{1}$/;
        if (!duiRegex.test(duiValor) || duiValor === "00000000-0") {
            e.preventDefault();
            marcarInvalido(duiInput, true);
            mostrarAlerta("error", "DUI inválido", "El DUI debe tener el formato 00000000-0 y no puede ser 00000000-0.");
            return;
        }

        if (duiYaRegistrado(duiValor)) {
            e.preventDefault();
            marcarInvalido(duiInput, true);
            mostrarAlerta("error", "DUI repetido", "Ya existe un receptor registrado con ese DUI.");
            return;
        }

        if (nombreValor.length < 2 || nombreValor.length > 60 || !nombrePersonaValido(nombreValor)) {
            e.preventDefault();
            marcarInvalido(nombre, true);
            mostrarAlerta("error", "Nombre inválido", "El nombre debe tener entre 2 y 60 caracteres y solo puede contener letras y espacios.");
            return;
        }

        if (apellidoValor.length < 2 || apellidoValor.length > 60 || !nombrePersonaValido(apellidoValor)) {
            e.preventDefault();
            marcarInvalido(apellido, true);
            mostrarAlerta("error", "Apellido inválido", "El apellido debe tener entre 2 y 60 caracteres y solo puede contener letras y espacios.");
            return;
        }

        if (!tipoSangreValido(tipo?.value)) {
            e.preventDefault();
            marcarInvalido(tipo, true);
            mostrarAlerta("error", "Tipo de sangre inválido", "Seleccione A, B, AB u O.");
            return;
        }

        if (!factorRhValido(rh?.value)) {
            e.preventDefault();
            marcarInvalido(rh, true);
            mostrarAlerta("error", "Factor Rh inválido", "Seleccione Rh + o Rh -.");
            return;
        }

        const vialesCompatibles = compatTableBody
            ? compatTableBody.querySelectorAll(".select-unit-btn").length
            : 0;

        if (!selectedUnitInput?.value) {
            e.preventDefault();

            if (vialesCompatibles === 0) {
                mostrarAlerta(
                    "warning",
                    "Sin vial seleccionado",
                    "No hay viales vigentes compatibles para el tipo de sangre y factor Rh seleccionados. No se puede registrar sin escoger un vial seguro."
                );
            } else {
                mostrarAlerta(
                    "warning",
                    "Seleccione un vial",
                    "Debe seleccionar manualmente un vial compatible. El sistema no hará una asignación automática."
                );
            }

            return;
        }
    }

    function separarTipoRh(tipoCompleto) {
        const valor = (tipoCompleto ?? "").trim().toUpperCase();

        if (!valor || valor === "ALL") {
            return { tipo: "all", rh: "all" };
        }

        const rh = valor.endsWith("+") || valor.endsWith("-") ? valor.slice(-1) : "all";
        const tipo = rh === "all" ? valor : valor.slice(0, -1);

        return { tipo, rh };
    }

    function aplicarFiltros() {
        const params = new URLSearchParams();
        const busqueda = searchInput?.value.trim() ?? "";
        const fecha = filterFecha?.value ?? "all";
        const estado = filterEstado?.value ?? "all";
        const tipo = filterTipoSangre?.value ?? "all";
        const rh = filterFactorRh?.value ?? "all";

        if (busqueda !== "") params.set("busqueda", busqueda);
        if (fecha !== "all") params.set("fecha", fecha);
        if (estado !== "all") params.set("estado", estado);
        if (tipo !== "all") params.set("tipoSangre", tipo);
        if (rh !== "all") params.set("factorRh", rh);

        const query = params.toString();
        window.location.href = query ? `/receptores/obtenerListado?${query}` : "/receptores/obtenerListado";
    }

    function limpiarFiltros() {
        window.location.href = "/receptores/obtenerListado";
    }

    function obtenerDatoNodo(nodo) {
        return nodo?.dato ?? nodo?.Dato ?? null;
    }

    function obtenerSiguienteNodo(nodo) {
        return nodo?.siguiente ?? nodo?.Siguiente ?? null;
    }

    function convertirListaEnlazada(cabeza) {
        const datos = [];
        let actual = cabeza;

        while (actual) {
            const dato = obtenerDatoNodo(actual);
            if (dato) datos.push(dato);
            actual = obtenerSiguienteNodo(actual);
        }

        return datos;
    }

    function valorPropiedad(obj, camel, pascal) {
        return obj?.[camel] ?? obj?.[pascal] ?? "";
    }

    function getBloodBadgeClass(tipo) {
        const t = (tipo || "").trim().toUpperCase();
        return {
            "A+": "badge-blue",
            "A-": "badge-blue",
            "B+": "badge-primary",
            "B-": "badge-primary",
            "AB+": "badge-pink",
            "AB-": "badge-pink",
            "O+": "badge-orange",
            "O-": "badge-red"
        }[t] || "badge-warning";
    }

    function renderCompatibles(data, filtro) {
        if (!compatTableBody) return;

        compatTableBody.innerHTML = "";
        if (selectedUnitInput) selectedUnitInput.value = "";

        if (!data || data.length === 0) {
            compatTableBody.innerHTML = `
                <tr>
                    <td colspan="5" style="text-align:center; padding:16px;">
                        No hay viales vigentes disponibles para el tipo de sangre y factor Rh seleccionados.
                    </td>
                </tr>
            `;
        } else {
            data.forEach(item => {
                const idVial = valorPropiedad(item, "idVial", "IdVial");
                const tipo = valorPropiedad(item, "tipo", "Tipo");
                const ubicacion = valorPropiedad(item, "ubicacion", "Ubicacion");
                const donante = valorPropiedad(item, "donante", "Donante");

                compatTableBody.insertAdjacentHTML("beforeend", `
                    <tr data-tipo="${tipo}">
                        <td>${idVial}</td>
                        <td><span class="badge-soft ${getBloodBadgeClass(tipo)}">${tipo}</span></td>
                        <td>${donante}</td>
                        <td>${ubicacion}</td>
                        <td>
                            <button type="button" class="select-unit-btn" data-unit="${idVial}">
                                ✓ SELECCIONAR
                            </button>
                        </td>
                    </tr>
                `);
            });
        }

        if (filterChip) {
            filterChip.textContent = "FILTRADO POR: " + filtro;
        }
    }

    function cargarCompatibles() {
        const tipo = tipoSangre?.value ?? "";
        const rh = factorRh?.value ?? "";

        if (!tipo || !rh) {
            renderCompatibles([], "Seleccione tipo y Rh");
            return;
        }

        fetch(`/receptores/ObtenerCompatibles?tipoSangre=${encodeURIComponent(tipo)}&factorRh=${encodeURIComponent(rh)}`)
            .then(r => r.json())
            .then(res => {
                if (res.success) {
                    const cabeza = res.cabeza ?? res.Cabeza ?? null;
                    const datos = convertirListaEnlazada(cabeza);
                    renderCompatibles(datos, res.filtro ?? res.Filtro ?? `${tipo}${rh}`);
                }
            });
    }

    btnAbrir?.addEventListener("click", abrirModal);
    btnCerrar?.addEventListener("click", cerrarModal);
    btnCancelar?.addEventListener("click", cerrarModal);

    modalBackdrop?.addEventListener("click", function (e) {
        if (e.target === modalBackdrop) {
            cerrarModal();
        }
    });

    btnCerrarDetalles?.addEventListener("click", cerrarDetalles);

    detallesBackdrop?.addEventListener("click", function (e) {
        if (e.target === detallesBackdrop) {
            cerrarDetalles();
        }
    });

    receptorForm?.addEventListener("submit", validarFormularioReceptor);

    duiInput?.addEventListener("input", function () {
        duiInput.value = formatearDui(duiInput.value);
        duiInput.classList.remove("is-invalid");
    });

    [document.getElementById("receptorNombre"), document.getElementById("receptorApellido")].forEach(input => {
        input?.addEventListener("input", function () {
            input.value = input.value
                .replace(/[^A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]/g, "")
                .replace(/\s{2,}/g, " ")
                .slice(0, 60);
            input.classList.remove("is-invalid");
        });
    });

    receptorForm?.querySelectorAll("input, select").forEach(input => {
        input.addEventListener("input", function () {
            input.classList.remove("is-invalid");
        });
        input.addEventListener("change", function () {
            input.classList.remove("is-invalid");
        });
    });

    tipoSangre?.addEventListener("change", cargarCompatibles);
    factorRh?.addEventListener("change", cargarCompatibles);

    compatTableBody?.addEventListener("click", function (e) {
        const btn = e.target.closest(".select-unit-btn");
        if (!btn) return;

        compatTableBody.querySelectorAll(".select-unit-btn").forEach(x => x.classList.remove("active"));
        btn.classList.add("active");

        if (selectedUnitInput) {
            selectedUnitInput.value = btn.dataset.unit ?? "";
        }
    });

    searchInput?.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            aplicarFiltros();
        }
    });

    bloodFilterButtons.forEach(button => {
        button.addEventListener("click", function () {
            const estabaActivo = button.classList.contains("active");

            bloodFilterButtons.forEach(x => x.classList.remove("active"));

            if (estabaActivo) {
                if (filterTipoSangre) filterTipoSangre.value = "all";
                if (filterFactorRh) filterFactorRh.value = "all";
                return;
            }

            button.classList.add("active");
            const partes = separarTipoRh(button.dataset.bloodFilter);

            if (filterTipoSangre) filterTipoSangre.value = partes.tipo;
            if (filterFactorRh) filterFactorRh.value = partes.rh;
        });
    });

    btnAplicarFiltros?.addEventListener("click", aplicarFiltros);
    btnLimpiarFiltros?.addEventListener("click", limpiarFiltros);

    btnToggleFiltrosReceptores?.addEventListener("click", function (e) {
        e.stopPropagation();
        receptoresFilterPanel?.classList.toggle("open");
    });

    document.addEventListener("click", function (e) {
        const btn = e.target.closest("[data-action-toggle]");

        if (btn) {
            e.stopPropagation();
            const id = btn.dataset.actionToggle;

            if (floatingMenu?.classList.contains("open") && activeReceptorId === id) {
                closeFloatingMenu();
            } else {
                openFloatingMenu(btn, id);
            }
            return;
        }

        const insideFloating = e.target.closest("#receptoresActionsFloatingMenu");
        if (!insideFloating) {
            closeFloatingMenu();
        }

        const dentroFiltro = e.target.closest(".inventario-filter-wrap");
        if (!dentroFiltro) {
            receptoresFilterPanel?.classList.remove("open");
        }
    });

    btnViewDetails?.addEventListener("click", function (e) {
        e.preventDefault();
        if (!activeReceptorId) return;

        fetch("/receptores/generarDetalleReceptor", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            body: `idReceptor=${encodeURIComponent(activeReceptorId)}`
        })
            .then(r => r.json())
            .then(res => {
                if (!res.success) return;

                const item = res.receptor;

                document.getElementById("detalleReceptorId").textContent = item.idReceptor ?? "-";
                document.getElementById("detalleReceptorTipo").textContent = item.tipoSangre ?? "-";
                document.getElementById("detalleReceptorUnidad").textContent = item.unidadAsignada ?? "-";
                document.getElementById("detalleReceptorNombre").textContent = item.beneficiario ?? "-";
                document.getElementById("detalleReceptorDui").textContent = item.duiReceptor ?? "-";
                document.getElementById("detalleReceptorFecha").textContent = item.fechaTransfusion ?? "-";
                document.getElementById("detalleReceptorEstado").textContent = item.estado ?? "-";
                document.getElementById("detalleReceptorDonante").textContent = item.donante ?? "-";
                document.getElementById("detalleReceptorDonanteDui").textContent = item.donanteDui ?? "-";
                document.getElementById("detalleReceptorDonanteTelefono").textContent = item.donanteTelefono ?? "-";

                abrirDetalles();
                closeFloatingMenu();
            });
    });

    window.addEventListener("scroll", closeFloatingMenu, true);
    window.addEventListener("resize", closeFloatingMenu);
});
