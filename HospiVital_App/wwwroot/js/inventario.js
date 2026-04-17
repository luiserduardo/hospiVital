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

        let hoy = new Date();
        hoy.setHours(0, 0, 0, 0);

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

      
        if ($("input[name='haDesayunado']:checked").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber desayunado lo vuelve excluyente.', 'error');
        }

        if ($("input[name='haDormido']:checked").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber dormido lo vuelve excluyente.', 'error');
        }

        if ($("input[name='realizadoTratuajes']:checked").val() === "true") {
            e.preventDefault();
            return Swal.fire('Excluido', 'Tatuajes recientes lo vuelven excluyente.', 'error');
        }

        if ($("input[name='haConsumido']:checked").val() === "true") {
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
        // Eliminamos espacios por si acaso
        const t = tipo.trim().toUpperCase();

        const mapas = {
            'O+': 'badge-o-pos',
            'O-': 'badge-o-neg',
            'A+': 'badge-a-pos',
            'A-': 'badge-a-neg',
            'B+': 'badge-b-pos',
            'B-': 'badge-b-neg',
            'AB+': 'badge-ab-pos',
            'AB-': 'badge-ab-neg'
        };

        return mapas[t] || 'badge-secondary';
    }

    // Función para el botón de los tres puntos (⋮)
    $(document).on("click", "[data-action-toggle]", function (e) {
        e.stopPropagation();
        const id = $(this).data("action-toggle");
        const menu = $(`[data-action-menu="${id}"]`);

        $(".actions-popover").not(menu).removeClass("open");
        menu.toggleClass("open");
    });


    // --- CERRAR MODAL DE DETALLES ---

    $('#btnCerrarDetalles').on('click', function () {
        $('#inventarioDetallesBackdrop').removeClass('open');
    });

    $('#inventarioDetallesBackdrop').on('click', function (e) {
        if (e.target === this) {
            $(this).removeClass('open');
        }
    });

  

});