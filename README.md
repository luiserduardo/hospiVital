# HospiVital

Guía de configuración y ejecución del sistema **HospiVital_App**, desarrollado en .NET bajo el patrón de arquitectura **ASP.NET MVC**.

---

## Requisitos Previos

Para ejecutar el proyecto en un entorno local, se recomienda contar con lo siguiente:

* **Visual Studio 2022** 
* **Git** (opcional, solo si se desea clonar el repositorio)

---

## Obtención del Proyecto

El proyecto puede obtenerse de dos formas:

###  Opción 1: Clonar el repositorio (recomendado)

Se debe abrir una terminal o consola de comandos y ejecutar:

```bash
git clone https://github.com/luiserduardo/HospiVital.git
```

###  Opción 2: Descargar en formato ZIP

Si el usuario no desea utilizar Git, puede descargar el proyecto manualmente:

1. Acceder al repositorio en GitHub
2. Hacer clic en el botón **"Code"**
3. Seleccionar **"Download ZIP"**
4. Extraer el contenido en una carpeta local

---

## Apertura del proyecto

Una vez obtenido el proyecto, se deben seguir los siguientes pasos:

1. Navegar a la carpeta del proyecto
2. Localizar el archivo:

```
HospiVital_App.sln
```

3. Abrirlo con Visual Studio 2022

---

##  Restauración de dependencias

Una vez abierto el proyecto, las dependencias suelen restaurarse automáticamente. En caso contrario:

* Hacer clic derecho sobre la solución
* Seleccionar **"Restaurar paquetes NuGet"**

---

## Ejecución

Para ejecutar el sistema:

1. Establecer **HospiVital_App** como proyecto de inicio
2. Ejecutar el proyecto presionando:

```
F5
```

O utilizar **IIS Express** (botón ubicado en la parte superior, tipo fecha verde) desde Visual Studio

3. El sistema se abrirá automáticamente en una dirección similar a:

```
https://localhost:[puerto]
```

---

##  Notas
El sistema cuenta con tres usuarios locales para las vistas, cada uno con una funcionalidad. Importante ya que la primera pantallas que se carga es el login

1.Asistente, muestra el panel de inventario donde se pueden ver los viales de sangre
2.Médico, muestra el panel de las salidas de bolsas de sangre.
3.Admin, muestra el panel con los usuarios que se tienen.

| Usuaria    | Clave                          |
|-----------|--------------------------------------|
| asistente | 1234    |
| medico       | 1234  |
| admin | 1234    |
