console.log("receptores.js cargado");

$(function () {

    $("#formularioReceptor").on("submit", function (e) {

        e.preventDefault();

        $(".form-control, .form-select").removeClass("is-invalid");

        let dui = $("input[name='dui']").val().trim();
        let nombres = $("input[name='nombres']").val().trim();
        let apellidos = $("input[name='apellidos']").val().trim();
        let vial = $("#vialSeleccionado").val();

        const duiRegex = /^[0-9]{8}-[0-9]$/;

        if (!duiRegex.test(dui)) {
            mostrarError("input[name='dui']", "Formato de DUI inválido (00000000-0)");
            return;
        }

        if (nombres === "") {
            mostrarError("input[name='nombres']", "Ingrese los nombres");
            return;
        }

        if (apellidos === "") {
            mostrarError("input[name='apellidos']", "Ingrese los apellidos");
            return;
        }

        
        if (!vial) {
            Swal.fire({
                icon: 'warning',
                title: 'Falta selección',
                text: 'Debe seleccionar un vial antes de continuar',
                confirmButtonColor: '#d33'
            });
            return;
        }

        Swal.fire({
            icon: 'success',
            title: 'Correcto',
            text: 'Formulario válido',
            confirmButtonColor: '#3085d6'
        }).then(() => {
            this.submit();
        });

    });

    function mostrarError(selector, mensaje) {
        $(selector).addClass("is-invalid");

        Swal.fire({
            icon: 'error',
            title: 'Validación',
            text: mensaje,
            confirmButtonColor: '#d33'
        });
    }

    // SELECCIONAR VIAL
    $("#tablaUnidades").on("click", ".seleccionar", function () {

        $(".seleccionar")
            .removeClass("btn-success")
            .addClass("btn-danger")
            .text("✔ Seleccionar");

        $(this)
            .removeClass("btn-danger")
            .addClass("btn-success")
            .text("✔ Seleccionado");

        let id = $(this).data("id");
        $("#vialSeleccionado").val(id);

    });

    // FILTRO
    $("#tipoSangre").on("change", function () {

        $("#vialSeleccionado").val("");

        $(".seleccionar")
            .removeClass("btn-success")
            .addClass("btn-danger")
            .text("✔ Seleccionar");

        let tipoSeleccionado = $(this).val();

        if (tipoSeleccionado) {
            $("#filtroContainer").removeClass("d-none");
            $("#filtroTexto").text("Filtrado por " + tipoSeleccionado);
        } else {
            $("#filtroContainer").addClass("d-none");
        }

        $("#tablaUnidades tbody tr").each(function () {

            let tipoFila = $(this).data("tipo");

            if (!tipoSeleccionado || tipoFila === tipoSeleccionado) {
                $(this).show();
            } else {
                $(this).hide();
            }

        });

    });

});

// BUSCADOR + FILTROS (INDEX)

let tipoSeleccionado = "";

function filtrarTabla() {

    let texto = $("#buscador").val().toLowerCase();

    $("#tablaReceptores tbody tr").each(function () {

        let id = ($(this).data("id") || "").toString().toLowerCase();
        let nombre = ($(this).data("nombre") || "").toLowerCase();
        let tipoFila = $(this).data("tipo");

        let coincideBusqueda =
            id.includes(texto) ||
            nombre.includes(texto);

        let coincideTipo =
            !tipoSeleccionado || tipoFila === tipoSeleccionado;

        if (coincideBusqueda && coincideTipo) {
            $(this).show();
        } else {
            $(this).hide();
        }

    });
}

// EVENTO BUSCADOR
$("#buscador").on("keyup", function () {
    filtrarTabla();
});

// EVENTO FILTRO
$(document).on("click", ".filtro-item", function () {

    tipoSeleccionado = $(this).data("tipo") || "";

    filtrarTabla();

});