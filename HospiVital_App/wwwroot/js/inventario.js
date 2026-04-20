// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//dom cargado
$(document).ready(function () {
       function generarCodigo() {
            return $.ajax({
                url: '/inventario/generarCodigo',
                type: 'GET'
            });
        }

        // ABRIR MODAL
        $('#btnAbrirDonacion').on('click', function () {
            setFechaHoy();
            generarCodigo()
                .done(function (data) {
                    $('#idUnidad').val(data);
                })
                .fail(function () {
                    console.error("Error al generar el código");
                });

            $('#inventarioModalBackdrop').addClass('open');
        });

        // CERRAR MODAL (botones)
        $('#btnCerrarDonacion, #btnCancelarDonacion').on('click', function () {
            $('#inventarioModalBackdrop').removeClass('open');
            limpiarFormulario();
        });

        // CERRAR AL HACER CLICK FUERA
        $('#inventarioModalBackdrop').on('click', function (e) {
            if (e.target === this) {
                $(this).removeClass('open');
                limpiarFormulario();
            }
        });

    function limpiarFormulario() {
        const form = $("#formularioAgregarUnidad");

        form[0].reset();


        form.find(".is-invalid").removeClass("is-invalid");
     
    }



    $('.toggle-group .toggle-option').on('click', function () {

        const btn = $(this);
        const group = btn.closest('.toggle-group');
        const target = group.data('target');
        const value = btn.data('value');

        group.find('.toggle-option').removeClass('active');
        btn.addClass('active');

        $('#' + target).val(value);
    });

    //validaciones del formulario que esta en la parte de obtenerListado, formularioAgregarUnidad

    //OJO, todo lo que diga  Swal.fire, es de una libreria para ventanas emergentes

    $("#formularioAgregarUnidad").on("submit", function (e) {

        // Limpiar estados de error previos
        $(".form-control, .form-select").removeClass("is-invalid");

        let incompleto = false;

        // Captura de valores básicos
        let peso = parseFloat($("input[name='peso']").val());
        let dui = $("input[name='dui']").val();
        let telefono = $("input[name='telefono']").val();

        let tipoSangre = $("select[name='tipoSangre']").val();
        let factorRh = $("select[name='factorRh']").val();
        let enfermedad = $("select[name='enfermedades']").val();

        let fechaIngresoStr = $("input[name='fechaIngreso']").val();
        let fechaCaducidadStr = $("input[name='fechaCaducidad']").val();


        if (!tipoSangre) {
            $("select[name='tipoSangre']").addClass("is-invalid");
            incompleto = true;
        }

        if (!factorRh) {
            $("select[name='factorRh']").addClass("is-invalid");
            incompleto = true;
        }

        if (!dui || dui.trim() === "") {
            $("input[name='dui']").addClass("is-invalid");
            incompleto = true;
        }

        if (!telefono || telefono.trim() === "") {
            $("input[name='telefono']").addClass("is-invalid");
            incompleto = true;
        }

        if (!$("input[name='peso']").val()) {
            $("input[name='peso']").addClass("is-invalid");
            incompleto = true;
        }

        if (!fechaIngresoStr) {
            $("input[name='fechaIngreso']").addClass("is-invalid");
            incompleto = true;
        }

        if (!fechaCaducidadStr) {
            $("input[name='fechaCaducidad']").addClass("is-invalid");
            incompleto = true;
        }

        if (incompleto) {
            e.preventDefault();
            return Swal.fire('Campos Vacíos', 'Por favor complete todos los campos obligatorios.', 'warning');
        }

  

        let fechaIngreso = new Date(fechaIngresoStr);
        let fechaCaducidad = new Date(fechaCaducidadStr);

       
        let hoy = new Date().toISOString().split("T")[0];
        if (fechaIngreso < hoy) {
            e.preventDefault();
            return mostrarError("input[name='fechaIngreso']", "La fecha de ingreso no puede ser anterior a hoy.");
        }

        if (fechaCaducidad <= fechaIngreso) {
            e.preventDefault();
            return mostrarError("input[name='fechaCaducidad']", "La fecha de caducidad debe ser posterior a la de ingreso.");
        }

   //peso
        if (isNaN(peso) || peso < 50) {
            e.preventDefault();
            return mostrarError("input[name='peso']", "El peso mínimo requerido es 50 kg.");
        }

  

        let telefonoLimpio = telefono.replace(/[-\s]/g, "");
        const telefonoRegex = /^\d{8}$/;

        if (!telefonoRegex.test(telefonoLimpio)) {
            e.preventDefault();
            return mostrarError("input[name='telefono']", "Formato inválido (0000-0000).");
        }

      
        const duiRegex = /^\d{8}-\d{1}$/;
        if (!duiRegex.test(dui)) {
            e.preventDefault();
            return mostrarError("input[name='dui']", "Formato DUI inválido (00000000-0).");
        }

      
        if ($("input[name='haDesayunado']").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber desayunado lo vuelve excluyente.', 'error');
        }

        if ($("input[name='haDormido']").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber dormido lo vuelve excluyente.', 'error');
        }

        if ($("input[name='realizadoTratuajes']").val() === "true") {
            e.preventDefault();
            return Swal.fire('Excluido', 'Tatuajes recientes lo vuelven excluyente.', 'error');
        }

        if ($("input[name='haConsumido']").val() === "true") {
            e.preventDefault();
            return Swal.fire('Excluido', 'Alcohol reciente lo vuelve excluyente.', 'error');
        }

        if (enfermedad !== "Nada") {
            e.preventDefault();
            $("select[name='enfermedades']").addClass("is-invalid");
            return Swal.fire('Exclusión Médica', `Padecer ${enfermedad} es excluyente.`, 'error');
        }

    });
    // Funcion para marcar en rojo
    function mostrarError(selector, mensaje) {
        $(selector).addClass("is-invalid");
        Swal.fire({
            icon: 'error',
            title: 'Validación',
            text: mensaje,
            confirmButtonColor: '#d33'
        });
        return false;
    }

    //funcion para recha automatica
    function setFechaHoy() {
        const hoy = new Date();
        const yyyy = hoy.getFullYear();
        const mm = String(hoy.getMonth() + 1).padStart(2, '0');
        const dd = String(hoy.getDate()).padStart(2, '0');

        const fechaFormateada = `${yyyy}-${mm}-${dd}`;

        $("input[name='fechaIngreso']").val(fechaFormateada);
    }


    //filtro
    // Filtro — toggle directo en el botón
    $("#btnToggleFiltros").on("click", function (e) {
        e.stopPropagation();
        $("#inventarioFilterPanel").toggleClass("open");
    });

    // Cerrar al hacer click fuera
    $(document).on("click", function (e) {
        if (!$(e.target).closest(".inventario-filter-wrap").length) {
            $("#inventarioFilterPanel").removeClass("open");
        }
    });


    let sangreSeleccionada = "";

    // clicks en los chips de sangre
    $("#bloodFilterGrid .blood-filter-chip").on("click", function () {

        const value = $(this).data("blood");

        // toggle (seleccionar / deseleccionar)
        if (sangreSeleccionada === value) {
            sangreSeleccionada = "";
            $(this).removeClass("active");
        } else {
            sangreSeleccionada = value;

            $("#bloodFilterGrid .blood-filter-chip").removeClass("active");
            $(this).addClass("active");
        }

    });

    //modal agregar detalles
    // --- MANEJO DE DETALLES ---

    $(document).on("click", "[data-view-detail]", function (e) {
        e.preventDefault();
        const id = $(this).data("view-detail");
        const detallesBackdrop = $("#inventarioDetallesBackdrop");

        // Cerramos el popover de acciones si está abierto
        $(".actions-popover").removeClass("open");

        $.ajax({
            url: '/inventario/generarDetalleUnidad',
            type: 'POST',
            data: { idUnidad: id }
        })
            .done(function (res) {
                if (res.success) {
                    $("#detalleSubtitulo").text("ID: " + res.unidad.idUnidad);
                    $("#detalleIdUnidad").text(res.unidad.idUnidad);

                    const bloodClass = getBloodBadgeClass(res.unidad.tipoSangre + res.unidad.factorRh);
                    $("#detalleTipoSangre").html(`<span class="badge-soft ${bloodClass}">${res.unidad.tipoSangre}${res.unidad.factorRh}</span>`);

                    $("#detalleCantidad").text(res.unidad.cantidad + " bolsa");

                    const estadoClass = res.unidad.estadoUnidad === "Vencido" ? "badge-danger" : "badge-success";
                    $("#detalleEstado").html(`<span class="badge-soft ${estadoClass}">${res.unidad.estado}</span>`);

                    $("#detalleNombre").text(res.unidad.nombreDonante + " "+ res.unidad.apellidoDonante);
                    $("#detalleDui").text(res.unidad.dui);
                    $("#detallePeso").text(res.unidad.peso); 
                    $("#detalleTelefono").text(res.unidad.telefono);

                    $("#detalleFechaIngreso").text(res.unidad.fechaIngreso);
                    $("#detalleFechaVencimiento").text(res.unidad.fechaCaducidad);

                    $("#inventarioDetallesBackdrop").addClass("open");
                } else {
                    Swal.fire("Aviso", res.message || "No se encontró la información", "info");
                }
            })
            .fail(function () {
                Swal.fire("Error", "No se pudo conectar con el servidor", "error");
            });
    });

    
    function getBloodBadgeClass(tipo) {
        const t = tipo.trim().toUpperCase();

        return {
            'A+': 'badge-blue',
            'A-': 'badge-blue',
            'B+': 'badge-primary',
            'B-': 'badge-primary',
            'AB+': 'badge-pink',
            'AB-': 'badge-pink',
            'O+': 'badge-orange',
            'O-': 'badge-red'
        }[t] || 'badge-warning';
    }

    // Función para el botón de los tres puntos (⋮)
    $(document).on("click", "[data-action-toggle]", function (e) {
        e.stopPropagation();
        const id = $(this).data("action-toggle");
        const menu = $(`[data-action-menu="${id}"]`);

        $(".actions-popover").not(menu).removeClass("open");
        menu.toggleClass("open");
    });

    $('#btnCerrarDetalles').on('click', function () {
        $('#inventarioDetallesBackdrop').removeClass('open');
    });

    $('#inventarioDetallesBackdrop').on('click', function (e) {
        if (e.target === this) {
            $(this).removeClass('open');
        }
    });


    //Funcionalidad de busqueda en tiempo real  

    let searchTimer = null;   

    $("#inventarioSearch").on("input", function () {
        clearTimeout(searchTimer);

        const termino = $(this).val().trim();

        searchTimer = setTimeout(function () {
            buscarYRenderizar(termino);
        }, 300);
    });

    function buscarYRenderizar(termino) {
        $.ajax({
            url: '/inventario/buscarUnidades',
            type: 'GET',
            data: { termino: termino }
        })
            .done(function (data) {
                renderizarTabla(data);
            })
            .fail(function () {
                Swal.fire("Error", "No se pudo realizar la búsqueda.", "error");
            });
    }

    function renderizarTabla(unidades) {
        const tbody = $("#inventarioTableBody");
        tbody.empty();

        if (unidades.length === 0) {
            tbody.append(`
            <tr>
                <td colspan="6" style="text-align:center; padding: 2rem; color: var(--text-muted);">
                    No se encontraron unidades.
                </td>
            </tr>
        `);
            $("#inventarioCount").text("Mostrando 0 de 0");
            return;
        }

        unidades.forEach(function (u) {
            const esVencido = u.estadoUnidad === "Vencido";
            const bloodClass = getBloodBadgeClass(u.tipoSangre + u.factorRh);
            const estadoClass = esVencido ? "badge-danger" : "badge-success";
            const estadoLabel = esVencido ? "Vencido" : "Disponible";
            const rowClass = esVencido ? "row-expired" : "";
            const fechaClass = esVencido ? "unit-code" : "";

            tbody.append(`
            <tr class="${rowClass}">
                <td class="unit-code">${u.idUnidad}</td>
                <td>
                    <span class="badge-soft ${bloodClass}">
                        ${u.tipoSangre}${u.factorRh}
                    </span>
                </td>
                <td>${u.fechaIngreso}</td>
                <td class="${fechaClass}">${u.fechaCaducidad}</td>
                <td>
                    <span class="badge-soft ${estadoClass}">${estadoLabel}</span>
                </td>
                <td>
                    <td class="actions-cell">
                        <div class="actions-dropdown">
                            <button type="button" class="actions-menu"
                                    data-action-toggle="${u.idUnidad}">⋮</button>
                            <div class="actions-popover"
                                 data-action-menu="${u.idUnidad}">
                                <button type="button" class="actions-popover-item"
                                        data-view-detail="${u.idUnidad}">
                                    Ver detalles
                                </button>
                            </div>
                        </div>
                    </td>
                </td>
            </tr>
        `);
        });

        $("#inventarioCount").text(`Mostrando ${unidades.length} de ${unidades.length}`);
    }


    //funcionalidad para el filtro

    $("#btnAplicarFiltros").on("click", function () {
        aplicarFiltros();
    });

    $("#btnLimpiarFiltros").on("click", function () {
        $("#filterFecha").val("all");
        $("#filterEstado").val("all");
        sangreSeleccionada = "";
        $("#bloodFilterGrid .blood-filter-chip").removeClass("active");
        $("#inventarioFilterPanel").removeClass("open");
        aplicarFiltros();
    });

    function aplicarFiltros() {
        const fechaVal = $("#filterFecha").val();
        let fechaParam = "todos";
        if (fechaVal === "today") fechaParam = "hoy";
        else if (fechaVal === "last7") fechaParam = "7";
        else if (fechaVal === "last30") fechaParam = "30";

        const estadoParam = $("#filterEstado").val() === "all" ? "todos" : $("#filterEstado").val();
        const sangreParam = sangreSeleccionada === "" ? "todos" : sangreSeleccionada;

        $.ajax({
            url: '/inventario/filtrarUnidades',
            type: 'GET',
            data: {
                fecha: fechaParam,
                estado: estadoParam,
                tipoSangre: sangreParam
            },
            beforeSend: function () {
                $("#inventarioTableBody").html(
                    '<tr><td colspan="6" style="text-align:center; padding:1rem;">Filtrando...</td></tr>'
                );
            }
        })
            .done(function (data) {
                renderizarTabla(data);
                $("#inventarioFilterPanel").removeClass("open");
            })
            .fail(function () {
                Swal.fire("Error", "No se pudieron aplicar los filtros.", "error");
            });
    }


    function formatearDui(valor) {
        const numeros = valor.replace(/\D/g, "").slice(0, 9);
        if (numeros.length <= 8) return numeros;
        return numeros.slice(0, 8) + "-" + numeros.slice(8);
    }

    function formatearTelefono(valor) {
        const numeros = valor.replace(/\D/g, "").slice(0, 8);
        if (numeros.length <= 4) return numeros;
        return numeros.slice(0, 4) + "-" + numeros.slice(4);
    }


    function formatearDui(valor) {
        const numeros = valor.replace(/\D/g, "").slice(0, 9);
        if (numeros.length <= 8) return numeros;
        return numeros.slice(0, 8) + "-" + numeros.slice(8);
    }

    function formatearTelefono(valor) {
        const numeros = valor.replace(/\D/g, "").slice(0, 8);
        if (numeros.length <= 4) return numeros;
        return numeros.slice(0, 4) + "-" + numeros.slice(4);
    }

    let activeActionId = null;
    const floatingMenu = $("#actionsFloatingMenu");
    const floatingViewBtn = $("#btnViewDetailsFloating");

    function closeFloatingMenu() {
        activeActionId = null;
        floatingMenu.removeClass("open");
    }

    function openFloatingMenu(button, id) {
        if (!floatingMenu.length) return;

        const rect = button.get(0).getBoundingClientRect();
        activeActionId = id;

        floatingMenu.css({ display: "block", visibility: "hidden", left: "0px", top: "0px" });

        const menuWidth = floatingMenu.outerWidth();
        const menuHeight = floatingMenu.outerHeight();
        const gap = 8;
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;

        let left = rect.right + gap;
        if (left + menuWidth > viewportWidth - 12) left = rect.left - menuWidth - gap;
        if (left < 12) left = 12;

        let top = rect.top + (rect.height / 2) - (menuHeight / 2);
        if (top < 12) top = 12;
        if (top + menuHeight > viewportHeight - 12) top = viewportHeight - menuHeight - 12;

        floatingMenu.css({ left: left + "px", top: top + "px", display: "", visibility: "" });
        floatingMenu.addClass("open");
    }

    floatingViewBtn.on("click", function (e) {
        e.preventDefault();
        if (!activeActionId) return;

        $.ajax({
            url: '/inventario/generarDetalleUnidad',
            type: 'POST',
            data: { idUnidad: activeActionId }
        })
            .done(function (res) {
                if (res.success) {
                    $("#detalleSubtitulo").text("ID: " + res.unidad.idUnidad);
                    $("#detalleIdUnidad").text(res.unidad.idUnidad);

                    const bloodClass = getBloodBadgeClass(res.unidad.tipoSangre + res.unidad.factorRh);
                    $("#detalleTipoSangre").html(
                        `<span class="badge-soft ${bloodClass}">${res.unidad.tipoSangre}${res.unidad.factorRh}</span>`
                    );

                    $("#detalleCantidad").text(res.unidad.cantidad + " bolsa");

                    const estadoClass = res.unidad.estadoUnidad === "Vencido" ? "badge-danger" : "badge-success";
                    $("#detalleEstado").html(
                        `<span class="badge-soft ${estadoClass}">${res.unidad.estado}</span>`
                    );

                    $("#detalleNombre").text(res.unidad.nombreDonante + " " + res.unidad.apellidoDonante);
                    $("#detalleDui").text(res.unidad.dui);
                    $("#detallePeso").text(res.unidad.peso);
                    $("#detalleTelefono").text(res.unidad.telefono);
                    $("#detalleFechaIngreso").text(res.unidad.fechaIngreso);
                    $("#detalleFechaVencimiento").text(res.unidad.fechaCaducidad);

                    $("#inventarioDetallesBackdrop").addClass("open");
                    closeFloatingMenu();
                } else {
                    Swal.fire("Aviso", res.message || "No se encontró la información", "info");
                }
            })
            .fail(function () {
                Swal.fire("Error", "No se pudo conectar con el servidor", "error");
            });
    });

    $(window).on("scroll resize", function () {
        closeFloatingMenu();
    });

    $(document).on("input", "#duiInput", function () {
        $(this).val(formatearDui($(this).val()));
    });
    $(document).on("input", "#telefonoInput", function () {
        $(this).val(formatearTelefono($(this).val()));
    });
});