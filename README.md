# AgroTech SmartFields

AgroTech SmartFields es una aplicaciÃ³n de escritorio moderna desarrollada en **WPF** (.NET 10) con arquitectura de N-Capas (DAL, BLL, UI) utilizando **Entity Framework Core** y **SQLite**. EstÃ¡ diseÃ±ada con los estilos y directrices visuales nativos de Windows 11 mediante WPF-UI.

## Requisitos Previos

Para poder compilar y ejecutar este proyecto, necesitas tener instalado:

1. [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/) (recomendado) o Visual Studio Code.
2. [SDK de .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

## CÃ³mo Levantar el Proyecto

Sigue estos pasos para clonar y ejecutar el proyecto en tu mÃ¡quina local:

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

3. **Ejecutar la aplicaciÃ³n:**
   Puedes iniciar la aplicaciÃ³n directamente desde la lÃ­nea de comandos ejecutando el proyecto de la Interfaz de Usuario (UI):
   `ash
   dotnet run --project AgroTech.UI/AgroTech.UI.csproj
   `
   *Alternativamente, puedes abrir la soluciÃ³n AgroTech.sln en Visual Studio 2022, establecer AgroTech.UI como proyecto de inicio y presionar F5.*

## Credenciales de Acceso (Modo Pruebas)

Al iniciar la aplicaciÃ³n por primera vez, la base de datos de SQLite (grotech.db) se generarÃ¡ automÃ¡ticamente e insertarÃ¡ algunos datos semilla (Seed Data) para facilitar el testing de la interfaz.

Puedes utilizar cualquiera de las siguientes cuentas para acceder al **Dashboard**:

| Rol                     | Correo ElectrÃ³nico                | ContraseÃ±a |
| ----------------------- | --------------------------------- | ---------- |
| Administrador del Sist. | dmin@agrotech.cl               | dmin123 |
| Supervisor              | carlos.supervisor@agrotech.cl   | carlos123|
| Trabajador en Terreno   | juan.trabajador@agrotech.cl     | juan123  |

## Arquitectura del Proyecto

El proyecto estÃ¡ dividido en tres capas principales:

* **AgroTech.DAL (Data Access Layer)**: Maneja la conexiÃ³n con la base de datos SQLite utilizando Entity Framework Core. Contiene los modelos (Cultivo.cs, Parcela.cs, Rol.cs, Usuario.cs) y el contexto AgroTechDbContext.cs.
* **AgroTech.BLL (Business Logic Layer)**: Contiene las reglas de negocio y validaciones. AquÃ­ se encuentra el AuthService.cs para el login.
* **AgroTech.UI (User Interface)**: Contiene la interfaz grÃ¡fica de usuario en XAML y Code-Behind. Emplea componentes NavigationView y temas oscuros/claros propios de Windows 11 (Fluent).

