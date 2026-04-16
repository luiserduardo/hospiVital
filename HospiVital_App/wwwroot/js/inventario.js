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

    //escuchar el evento al abrir para ejecutar la funcion que esta dentro
    $('#modalAgregar').on('show.bs.modal', function () {

        //ejecuta la funcion generarCodigo, solo si la peticion es exitosa
        generarCodigo()
            .done(function (data) {
                //buscado valor y asigna
                $('#idUnidad').val(data);
            })
            .fail(function () {
                console.error("Error al generar el código desde el servidor");
            });

    });



    //validaciones del formulario que esta en la parte de obtenerListado, formularioAgregarUnidad

    //OJO, todo lo que diga  Swal.fire, es de una libreria para ventanas emergentes

    $("#formularioAgregarUnidad").on("submit", function (e) {
        // Limpiar estados de error previos
        $(".form-control, .form-select").removeClass("is-invalid");

        // Captura de valores básicos
        let peso = parseFloat($("input[name='peso']").val());
        let dui = $("input[name='dui']").val();
        let fechaIngreso = new Date($("input[name='fechaIngreso']").val());
        let fechaCaducidad = new Date($("input[name='fechaCaducidad']").val());
        let hoy = new Date();
        hoy.setHours(0, 0, 0, 0);

        // fechas no pueden ser antes de la actual
        if (fechaIngreso < hoy) {
            e.preventDefault();
            return mostrarError("input[name='fechaIngreso']", "La fecha de ingreso no puede ser anterior a hoy.");
        }

        // fecha vencimiento no puede ser menor a la actual
        if (fechaCaducidad <= fechaIngreso) {
            e.preventDefault();
            return mostrarError("input[name='fechaCaducidad']", "La fecha de caducidad debe ser posterior a la de ingreso.");
        }

        // peso no puede ser menor a 50 kg, mostrar mensaje de que no es apto para donar
        if (peso < 50) {
            e.preventDefault();
            return mostrarError("input[name='peso']", "El donante no es apto: El peso mínimo requerido es de 50 kg.");
        }

        // asegurar formato del dui con el guion
        const duiRegex = /^\d{8}-\d{1}$/;
        if (!duiRegex.test(dui)) {
            e.preventDefault();
            return mostrarError("input[name='dui']", "Formato de DUI inválido (00000000-0).");
        }

        // validar datos para exclusion (Marcadores Si / No)
        //si (se busca el valor que ha sido seleccionado es igual a condicion)

        // desayuno
        if ($("input[name='haDesayunado']:checked").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber desayunado lo vuelve excluyente para donar hoy por seguridad física.', 'error');
        }

        //  horas sue;o
        if ($("input[name='haDormido']:checked").val() === "false") {
            e.preventDefault();
            return Swal.fire('Excluido', 'No haber dormido al menos 6 horas lo vuelve excluyente para donar.', 'error');
        }

        // tiempo tatuajes
        if ($("input[name='realizadoTratuajes']:checked").val() === "true") {
            e.preventDefault();
            return Swal.fire('Excluido', 'Haberse realizado tatuajes en los últimos 12 meses lo vuelve excluyente.', 'error');
        }

        // bebidas
        if ($("input[name='haConsumido']:checked").val() === "true") {
            e.preventDefault();
            return Swal.fire('Excluido', 'El consumo de alcohol en las últimas 48 horas lo vuelve excluyente.', 'error');
        }

        // combobox enfermedad cronica
        let enfermedad = $("select[name='enfermedades']").val();
        if (enfermedad !== "Nada") {
            e.preventDefault();
            $("select[name='enfermedades']").addClass("is-invalid");
            return Swal.fire('Exclusión Médica', `Padecer ${enfermedad} es una condición de exclusión permanente.`, 'error');
        }

        //  ningun dato vacio o no seleccionado
        let incompleto = false;
        //para cada campo requerido ejecuta una funcion interna, donde de esa parte recupera el valor sin espacios y lo parca como incompleto
        $("input[required]").each(function () {
            if ($(this).val().trim() === "") {
                $(this).addClass("is-invalid");
                incompleto = true;
            }
        });

        //luego de marcar como incompleto, lanzar advertencia
        if (incompleto) {
            e.preventDefault();
            return Swal.fire('Campos Vacíos', 'Por favor complete todos los campos obligatorios.', 'warning');
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


}); 