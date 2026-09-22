# AgroTech SmartFields

AgroTech SmartFields es una aplicación de escritorio desarrollada en **WPF** (.NET 10) con arquitectura de N-Capas (DAL, BLL, UI) utilizando el patrón **MVVM** (CommunityToolkit.Mvvm). Inicialmente se planteó usar una base de datos MySQL o en la nube de Azure, **pero Azure dio muchos problemas de configuración y estabilidad**, por lo que se optó por migrar a **PostgreSQL alojado en Supabase** usando `Npgsql.EntityFrameworkCore.PostgreSQL`. La interfaz usa los estilos nativos de Windows 11 mediante WPF-UI.

## Requisitos Previos

1. [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/) (recomendado) o Visual Studio Code.
2. [SDK de .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
3. Una cuenta en [Supabase](https://supabase.com) (o un servidor PostgreSQL local) para alojar la base de datos.

## Cómo Levantar el Proyecto

### 1. Levantar la base de datos en Supabase

Debes crear un proyecto en Supabase, ir al **SQL Editor** y ejecutar el script completo ubicado en `docs/supabase_schema.sql`.

Esto crea las 4 tablas con sus `CHECK` y restricciones de llaves foráneas, y siembra Roles + 3 Usuarios de prueba + Parcelas + Cultivos de ejemplo.

### 2. Configurar la cadena de conexión

Abre `AgroTech.DAL/AgroTechDbContext.cs` y ajusta las constantes al inicio del archivo (`Host`, `Port`, `UserId`, `Password`) con las credenciales que te entregó Supabase.

### 3. Restaurar, compilar y ejecutar

```bash
dotnet restore
dotnet build
dotnet run --project AgroTech.UI/AgroTech.UI.csproj
```

*Alternativamente, abre `AgroTech.sln` en Visual Studio 2022, establece `AgroTech.UI` como proyecto de inicio y presiona F5.*

## Credenciales de Acceso (datos semilla)

Las contraseñas ya están guardadas como hash BCrypt real en la base de datos (RNF-02). Puedes iniciar sesión con:

| Rol           | Correo Electrónico          | Contraseña    |
| ------------- | ---------------------------- | ------------- |
| Administrador | a.valenzuela@agrotech.cl     | `Admin123!`   |
| Supervisor    | m.fuentes@agrotech.cl        | `Super123!`   |
| Trabajador    | p.carrasco@agrotech.cl       | `Trabajo123!` |

Se recomienda cambiar estas contraseñas (o crear usuarios reales) desde la pantalla de **Gestión de Usuarios** apenas tengas el sistema funcionando — el registro de usuarios nuevos hashea la contraseña con BCrypt automáticamente.

## Reglas de acceso por rol (RBAC)

* **Administrador**: único que ve y usa el módulo "Gestión de Usuarios" (alta, edición, suspensión/reactivación — no hay borrado físico, se preserva la trazabilidad).
* **Supervisor**: acceso total a "Parcelas y Cultivos" (registrar y consultar).
* **Trabajador**: sólo lectura en "Parcelas y Cultivos" (los campos y botones de registro aparecen deshabilitados).

## Arquitectura del Proyecto

* **AgroTech.DAL (Data Access Layer)**: `AgroTechDbContext.cs` (EF Core + Npgsql/PostgreSQL, mapeado 1:1 contra `docs/supabase_schema.sql`), modelos (`Usuario`, `Rol`, `Parcela`, `Cultivo`) y `SessionManager.cs` (usuario logueado en memoria, para trazabilidad y RBAC).
* **AgroTech.BLL (Business Logic Layer)**: `AuthService.cs` (login con BCrypt), `ParcelaService.cs` y `CultivoService.cs` (RF-01, validaciones de negocio), `UsuarioService.cs` (RF-02, CRUD + baja lógica).
* **AgroTech.UI (User Interface)**: XAML + ViewModels (MVVM). Las vistas (`Views/ParcelasPage`, `Views/UsuariosPage`, `Dashboard`, `MainWindow`) están separadas de la lógica mediante `CommunityToolkit.Mvvm` usando **Data Binding**.

## Pendientes conocidos (fuera del alcance de esta iteración)

* RF-05 (plan de pruebas unitarias automatizado) aún no está implementado como proyecto de tests.
* No hay una pantalla dedicada para editar/eliminar cultivos individuales (por ahora se listan y se registran junto a la parcela).
