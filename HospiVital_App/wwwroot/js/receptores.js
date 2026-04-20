document.addEventListener("DOMContentLoaded", function () {
    const modalBackdrop = document.getElementById("receptorModalBackdrop");
    const btnAbrir = document.getElementById("btnAbrirReceptor");
    const btnCerrar = document.getElementById("btnCerrarReceptor");
    const btnCancelar = document.getElementById("btnCancelarReceptor");

    const detallesBackdrop = document.getElementById("receptorDetallesBackdrop");
    const btnCerrarDetalles = document.getElementById("btnCerrarDetallesReceptor");

    const duiInput = document.getElementById("receptorDui");
    const tipoSangre = document.getElementById("tipoSangreReceptor");
    const filterChip = document.getElementById("receptorFilterChip");
    const searchInput = document.getElementById("receptoresSearch");
    const tableBody = document.getElementById("receptoresTableBody");

    const compatTableBody = document.getElementById("compatTableBody");

    const floatingMenu = document.getElementById("receptoresActionsFloatingMenu");
    const btnViewDetails = document.getElementById("btnViewReceptorDetails");

    const btnToggleFiltrosReceptores = document.getElementById("btnToggleFiltrosReceptores");
    const receptoresFilterPanel = document.getElementById("receptoresFilterPanel");

    let activeReceptorId = null;

    function abrirModal() {
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

    function filtrarTabla() {
        if (!searchInput || !tableBody) return;

        const term = searchInput.value.trim().toLowerCase();
        const rows = tableBody.querySelectorAll("tr");
        let visibles = 0;

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const mostrar = text.includes(term);
            row.style.display = mostrar ? "" : "none";
            if (mostrar) visibles++;
        });

        const count = document.getElementById("receptoresCount");
        if (count) {
            count.textContent = `Mostrando ${visibles === 0 ? 0 : 1}-${visibles} de ${rows.length}`;
        }
    }

    function renderCompatibles(data, filtro) {
        if (!compatTableBody) return;

        compatTableBody.innerHTML = "";

        if (!data || data.length === 0) {
            compatTableBody.innerHTML = `
                <tr>
                    <td colspan="4" style="text-align:center; padding:16px;">
                        No hay unidades compatibles disponibles.
                    </td>
                </tr>
            `;
        } else {
            data.forEach(item => {
                compatTableBody.insertAdjacentHTML("beforeend", `
                    <tr data-tipo="${item.tipo}">
                        <td>${item.idVial}</td>
                        <td>${item.tipo}</td>
                        <td>${item.ubicacion}</td>
                        <td>
                            <button type="button" class="select-unit-btn" data-unit="${item.idVial}">
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

    duiInput?.addEventListener("input", function () {
        duiInput.value = formatearDui(duiInput.value);
    });

    tipoSangre?.addEventListener("change", function () {
        const valor = tipoSangre.value;
        const tipo = valor.slice(0, -1);
        const rh = valor.slice(-1);

        fetch(`/receptores/ObtenerCompatibles?tipoSangre=${encodeURIComponent(tipo)}&factorRh=${encodeURIComponent(rh)}`)
            .then(r => r.json())
            .then(res => {
                if (res.success) {
                    renderCompatibles(res.data, res.filtro);
                }
            });
    });

    compatTableBody?.addEventListener("click", function (e) {
        const btn = e.target.closest(".select-unit-btn");
        if (!btn) return;

        compatTableBody.querySelectorAll(".select-unit-btn").forEach(x => x.classList.remove("active"));
        btn.classList.add("active");
    });

    searchInput?.addEventListener("input", filtrarTabla);

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
                document.getElementById("detalleReceptorFecha").textContent = item.fechaTransfusion ?? "-";
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