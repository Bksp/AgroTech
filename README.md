# AgroTech SmartFields

AgroTech SmartFields es una aplicación de escritorio moderna desarrollada en **WPF** (.NET 10) con arquitectura de N-Capas (DAL, BLL, UI) utilizando **Entity Framework Core** y **SQLite**. Está diseñada con los estilos y directrices visuales nativos de Windows 11 mediante WPF-UI.

## Requisitos Previos

Para poder compilar y ejecutar este proyecto, necesitas tener instalado:

1. [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/) (recomendado) o Visual Studio Code.
2. [SDK de .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

## Cómo Levantar el Proyecto

Sigue estos pasos para clonar y ejecutar el proyecto en tu máquina local:

1. **Clonar el repositorio:**
   `ash
   git clone git@github.com:Bksp/AgroTech.git
   cd AgroTech
   `

2. **Restaurar las dependencias y compilar:**
   `ash
   dotnet restore
   dotnet build
   `

3. **Ejecutar la aplicación:**
   Puedes iniciar la aplicación directamente desde la línea de comandos ejecutando el proyecto de la Interfaz de Usuario (UI):
   `ash
   dotnet run --project AgroTech.UI/AgroTech.UI.csproj
   `
   *Alternativamente, puedes abrir la solución AgroTech.sln en Visual Studio 2022, establecer AgroTech.UI como proyecto de inicio y presionar F5.*

## Credenciales de Acceso (Modo Pruebas)

Al iniciar la aplicación por primera vez, la base de datos de SQLite (grotech.db) se generará automáticamente e insertará algunos datos semilla (Seed Data) para facilitar el testing de la interfaz.

Puedes utilizar cualquiera de las siguientes cuentas para acceder al **Dashboard**:

| Rol                     | Correo Electrónico                | Contraseña |
| ----------------------- | --------------------------------- | ---------- |
| Administrador del Sist. | dmin@agrotech.cl               | dmin123 |
| Supervisor              | carlos.supervisor@agrotech.cl   | carlos123|
| Trabajador en Terreno   | juan.trabajador@agrotech.cl     | juan123  |

## Arquitectura del Proyecto

El proyecto está dividido en tres capas principales:

* **AgroTech.DAL (Data Access Layer)**: Maneja la conexión con la base de datos SQLite utilizando Entity Framework Core. Contiene los modelos (Cultivo.cs, Parcela.cs, Rol.cs, Usuario.cs) y el contexto AgroTechDbContext.cs.
* **AgroTech.BLL (Business Logic Layer)**: Contiene las reglas de negocio y validaciones. Aquí se encuentra el AuthService.cs para el login.
* **AgroTech.UI (User Interface)**: Contiene la interfaz gráfica de usuario en XAML y Code-Behind. Emplea componentes NavigationView y temas oscuros/claros propios de Windows 11 (Fluent).
