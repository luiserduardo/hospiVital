document.addEventListener("DOMContentLoaded", function () {
    const colorPrincipal = "#e61c5d";

    function alerta(icon, title, text) {
        if (window.Swal) {
            return Swal.fire({
                icon,
                title,
                text,
                confirmButtonColor: colorPrincipal,
                customClass: {
                    popup: "rounded-4"
                }
            });
        }

        alert(text);
        return Promise.resolve();
    }


    function cambiarVisibilidadPassword(btn, mostrar) {
        const wrap = btn.closest(".password-wrap");
        const input = wrap?.querySelector(".password-input");

        if (!input) return;

        input.type = mostrar ? "text" : "password";
        btn.classList.toggle("active", mostrar);
    }

    document.querySelectorAll(".password-toggle").forEach(btn => {
        btn.addEventListener("mousedown", function (e) {
            e.preventDefault();
            cambiarVisibilidadPassword(btn, true);
        });

        btn.addEventListener("mouseup", function () {
            cambiarVisibilidadPassword(btn, false);
        });

        btn.addEventListener("mouseleave", function () {
            cambiarVisibilidadPassword(btn, false);
        });

        btn.addEventListener("touchstart", function (e) {
            e.preventDefault();
            cambiarVisibilidadPassword(btn, true);
        }, { passive: false });

        btn.addEventListener("touchend", function () {
            cambiarVisibilidadPassword(btn, false);
        });

        btn.addEventListener("touchcancel", function () {
            cambiarVisibilidadPassword(btn, false);
        });

        btn.addEventListener("blur", function () {
            cambiarVisibilidadPassword(btn, false);
        });
    });

    function validarFormulario(form, campos) {
        let valido = true;

        campos.forEach(nombre => {
            const input = form.querySelector(`[name='${nombre}']`) || document.querySelector(`[form='${form.id}'][name='${nombre}']`);
            if (!input) return;

            input.classList.remove("is-invalid");

            if (!input.value || input.value.trim() === "") {
                input.classList.add("is-invalid");
                valido = false;
            }
        });

        return valido;
    }

    const crearForm = document.getElementById("crearUsuarioForm");
    crearForm?.addEventListener("submit", function (e) {
        const ok = validarFormulario(crearForm, ["nuevoUsuario", "nuevaContrasena", "nuevoNombre", "nuevoRol"]);

        if (!ok) {
            e.preventDefault();
            alerta("warning", "Campos incompletos", "Complete usuario, contraseña, nombre y rol para crear el usuario.");
        }
    });

    document.querySelectorAll(".editar-usuario-form").forEach(form => {
        form.addEventListener("submit", function (e) {
            const ok = validarFormulario(form, ["usuarioEditado", "contrasenaEditada", "nombreEditado", "rolEditado"]);

            if (!ok) {
                e.preventDefault();
                alerta("warning", "Campos incompletos", "Complete usuario, contraseña, nombre y rol antes de guardar.");
            }
        });
    });

    document.querySelectorAll(".usuario-delete-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            const formId = btn.dataset.form;
            const usuario = btn.dataset.usuario || "este usuario";
            const esPrincipal = btn.dataset.principal === "true" || usuario.toLowerCase() === "admin";
            const form = document.getElementById(formId);

            if (!form) return;

            if (esPrincipal) {
                alerta("info", "Administrador principal", "No se puede eliminar el usuario administrador principal del sistema.");
                return;
            }

            if (window.Swal) {
                Swal.fire({
                    icon: "warning",
                    title: "Eliminar usuario",
                    html: `¿Desea eliminar el usuario <strong>${usuario}</strong>?`,
                    showCancelButton: true,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "Cancelar",
                    confirmButtonColor: colorPrincipal,
                    cancelButtonColor: "#64748b",
                    reverseButtons: true,
                    customClass: {
                        popup: "rounded-4"
                    }
                }).then(result => {
                    if (result.isConfirmed) {
                        form.submit();
                    }
                });
            } else if (confirm(`¿Desea eliminar el usuario ${usuario}?`)) {
                form.submit();
            }
        });
    });
});
