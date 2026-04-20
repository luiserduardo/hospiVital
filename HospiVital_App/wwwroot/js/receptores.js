console.log("receptores.js cargado correctamente");

//Filtro del Index
let tipoSeleccionadoGlobal = "";

$(function () {
    //Gestion del Modal de Receptores
    $("#tipoSangre").on("change", function () {
        const tipoSeleccionado = $(this).val();
        $("#vialSeleccionado").val(""); //Borrar selección anterior
        const $cuerpoTabla = $("#tablaUnidades tbody");

        if (tipoSeleccionado) {
            $("#filtroContainer").removeClass("d-none");
            $("#filtroTexto").text("Compatibles con " + tipoSeleccionado);

            //Mensaje para el usuario
            $cuerpoTabla.html('<tr><td colspan="4" class="text-center">Buscando unidades compatibles...</td></tr>');

            //Llamada AJAX al controlador
            $.get("/receptores/ObtenerVialesCompatibles", { tipoSangreCompleto: tipoSeleccionado }, function (data) {
                $cuerpoTabla.empty();
                if (data.length === 0) {
                    $cuerpoTabla.append('<tr><td colspan="4" class="text-center text-danger">No hay viales compatibles en inventario</td></tr>');
                } else {
                    data.forEach(vial => {
                        $cuerpoTabla.append(`
                            <tr>
                                <td>${vial.id}</td>
                                <td><span class="badge bg-primary bg-opacity-25 text-primary">${vial.tipo}</span></td>
                                <td>${vial.ubicacion}</td>
                                <td>
                                    <button type="button" class="btn btn-danger btn-sm seleccionar" data-id="${vial.id}">
                                        ✔ Seleccionar
                                    </button>
                                </td>
                            </tr>
                        `);
                    });
                }
            }).fail(function () {
                $cuerpoTabla.html('<tr><td colspan="4" class="text-center text-danger">Error al conectar con el servidor</td></tr>');
            });
        } else {
            $("#filtroContainer").addClass("d-none");
            $cuerpoTabla.html('<tr><td colspan="4" class="text-center">Seleccione un tipo de sangre</td></tr>');
        }



    });

    //Selección del vial de sangre
    $(document).on("click", ".seleccionar", function () {
        //Resetear botones
        $(".seleccionar").removeClass("btn-success").addClass("btn-danger").text("✔ Seleccionar");

        //Marcar vial seleccionado
        $(this).removeClass("btn-danger").addClass("btn-success").text("✔ Seleccionado");

        let id = $(this).data("id");
        $("#vialSeleccionado").val(id);
    });

    //Validaciones del Formulario
    $("#formularioReceptor").on("submit", function (e) {
        e.preventDefault();
        //Obtener los datos
        const dui = $("input[name='dui']").val().trim();
        const nombres = $("input[name='nombres']").val().trim();
        const apellidos = $("input[name='apellidos']").val().trim();
        const vial = $("#vialSeleccionado").val();
        const duiRegex = /^[0-9]{8}-[0-9]$/;

        //Validaciones
        if (!duiRegex.test(dui)) {
            Swal.fire('Error', 'Formato de DUI inválido (00000000-0)', 'error');
            return;
        }
        if (nombres === "" || apellidos === "") {
            Swal.fire('Atención', 'Debe completar el nombre y apellido del paciente', 'warning');
            return;
        }
        if (!vial) {
            Swal.fire('Falta Selección', 'Debe seleccionar un vial compatible de la tabla', 'warning');
            return;
        }

        //Si todo es correcto
        Swal.fire({
            icon: 'success',
            title: '¡Todo listo!',
            text: 'Registrando beneficiario y asignando unidad...',
            showConfirmButton: false,
            timer: 1500
        }).then(() => {
            // Enviamos el formulario al controlador
            document.getElementById("formularioReceptor").submit();
        });
    });

    //Buscar por ID o Nombre
    $("#buscador").on("keyup", function () {
        filtrarTablaPrincipal();
    });

    //Filtrar tipos de sangre
    $(document).on("click", ".filtro-item", function () {
        tipoSeleccionadoGlobal = $(this).data("tipo") || "";
        filtrarTablaPrincipal();
    });

    //Editar un registro
    $(document).on("click", ".btn-editar", function (e) {
        e.preventDefault();

        //Extraer datos de la tabla
        const idFull = $(this).data("id"); 
        const idNumerico = idFull.replace("REC-", "");
        const beneficiario = $(this).data("beneficiario");
        const tipoSangre = $(this).data("tipo");
        const dui = $(this).data("dui");

        //Separar nombres y apellidos
        const partes = beneficiario.split(' ');
        const nombres = partes[0] + (partes.length > 2 ? " " + partes[1] : "");
        const apellidos = partes.length > 2 ? partes.slice(2).join(' ') : partes[1];

        //Rellenar el modal con los datos obtenidos
        $("#modalReceptor h5").text("EDITAR RECEPTOR: " + idFull);
        $("input[name='dui']").val(dui);
        $("input[name='nombres']").val(nombres);
        $("input[name='apellidos']").val(apellidos);
        $("#tipoSangre").val(tipoSangre).trigger('change'); //Para buscar los viales compatibles

        //Muestra el número de dui (solo de lectura)
        $("input[name='dui']").val(dui).prop("readonly", true).css("background-color", "#e9ecef");

        //Enviar los cambios al método de editar del controlador
        $("#formularioReceptor").attr("action", "/receptores/Edit/" + idNumerico);

        //Mostrar el modal
        $("#modalReceptor").modal("show");
    });

    //Resetear el modal cuando se cierre
    $('#modalReceptor').on('hidden.bs.modal', function () {
        $("#formularioReceptor")[0].reset();
        $("input[name='dui']")
            .prop("readonly", false)      
            .removeAttr("readonly")   
            .css("background-color", "#fff");
        $("#formularioReceptor").attr("action", "/receptores/RegistrarAsignacion");
        $("#modalReceptor h5").text("REGISTRO DE RECEPTOR Y ASIGNACIÓN");
        $("#tablaUnidades tbody").html('<tr><td colspan="4" class="text-center text-muted">Seleccione un tipo de sangre para buscar viales</td></tr>');
    });

    //Para eliminar un registro
    $(document).on("click", ".btn-eliminar", function (e) {
        e.preventDefault();
        const idReceptor = $(this).data("id");

        Swal.fire({
            title: '¿Eliminar donativo?',
            text: "Si la unidad no está vencida, regresará al inventario disponible.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/receptores/Delete",
                    type: "POST",
                    data: {
                        id: idReceptor,
                        //Buscamos el token que genera el form de la página
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire('Actualizado', response.message, 'success')
                                .then(() => location.reload());
                        } else {
                            Swal.fire('Error', response.message, 'error');
                        }
                    },
                    error: function () {
                        Swal.fire('Error', 'No se pudo comunicar con el servidor', 'error');
                    }
                });
            }
        });
    });
});

//Función para filtrar la tabla del Index
function filtrarTablaPrincipal() {
    let texto = $("#buscador").val().toLowerCase();

    $("#tablaReceptores tbody tr").each(function () {
        let id = ($(this).data("id") || "").toString().toLowerCase();
        let nombre = ($(this).data("nombre") || "").toLowerCase();
        let tipoFila = $(this).data("tipo");

        let coincideBusqueda = id.includes(texto) || nombre.includes(texto);
        let coincideTipo = !tipoSeleccionadoGlobal || tipoFila === tipoSeleccionadoGlobal;

        if (coincideBusqueda && coincideTipo) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}