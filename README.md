# AgroTech SmartFields

AgroTech SmartFields es una aplicación de escritorio desarrollada en **WPF** (.NET 10) con arquitectura de N-Capas (DAL, BLL, UI), usando **Entity Framework Core** contra una base de datos **MySQL local** (vía Pomelo.EntityFrameworkCore.MySql), tal como lo exige el documento de requisitos (Restricción: motor MySQL local, script `.sql`). La interfaz usa los estilos nativos de Windows 11 mediante WPF-UI.

## Requisitos Previos

1. [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/) (recomendado) o Visual Studio Code.
2. [SDK de .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
3. **Servidor MySQL local** (8.0+) corriendo — por ejemplo XAMPP, WAMP, o MySQL Server + MySQL Workbench.

## Cómo Levantar el Proyecto

### 1. Levantar la base de datos

Con tu servidor MySQL corriendo, ejecuta el script completo en `docs/bbdd.sql` (por ejemplo desde MySQL Workbench, o por línea de comandos):

```bash
mysql -u root -p < docs/bbdd.sql
```

Esto crea la base `agrotech_db`, las 4 tablas con sus `CHECK` y `FOREIGN KEY ... ON DELETE RESTRICT`, y siembra Roles + 3 Usuarios de prueba + Parcelas + Cultivos de ejemplo.

### 2. Configurar la cadena de conexión

Abre `AgroTech.DAL/AgroTechDbContext.cs` y ajusta las constantes al inicio del archivo (`Server`, `Port`, `UserId`, `Password`) según tu instalación local de MySQL. Por defecto asume `localhost:3306`, usuario `root` sin contraseña.

### 3. Restaurar, compilar y ejecutar

```bash
dotnet restore
dotnet build
dotnet run --project AgroTech.UI/AgroTech.UI.csproj
```

*Alternativamente, abre `AgroTech.sln` en Visual Studio 2022, establece `AgroTech.UI` como proyecto de inicio y presiona F5.*

## Credenciales de Acceso (datos semilla de `docs/bbdd.sql`)

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

* **AgroTech.DAL (Data Access Layer)**: `AgroTechDbContext.cs` (EF Core + Pomelo/MySQL, mapeado 1:1 contra `docs/bbdd.sql`), modelos (`Usuario`, `Rol`, `Parcela`, `Cultivo`) y `SessionManager.cs` (usuario logueado en memoria, para trazabilidad y RBAC).
* **AgroTech.BLL (Business Logic Layer)**: `AuthService.cs` (login con BCrypt), `ParcelaService.cs` y `CultivoService.cs` (RF-01, validaciones de negocio), `UsuarioService.cs` (RF-02, CRUD + baja lógica).
* **AgroTech.UI (User Interface)**: XAML + code-behind. `MainWindow` (login), `Dashboard` (navegación + RBAC), `Views/ParcelasPage` y `Views/UsuariosPage` conectadas de verdad a la base de datos (sin datos "simulados").

## Pendientes conocidos (fuera del alcance de esta iteración)

* RF-05 (plan de pruebas unitarias automatizado) aún no está implementado como proyecto de tests.
* No hay una pantalla dedicada para editar/eliminar cultivos individuales (por ahora se listan y se registran junto a la parcela).
