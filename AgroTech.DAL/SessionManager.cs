using AgroTech.DAL.Models;

namespace AgroTech.DAL
{
    public static class SessionManager
    {
        public static Usuario? CurrentUser { get; private set; }

        public static void IniciarSesion(Usuario usuario) => CurrentUser = usuario;

        public static void CerrarSesion() => CurrentUser = null;

        public static bool EsAdministrador => CurrentUser?.Rol?.NombreRol == "Administrador";
        public static bool EsSupervisor => CurrentUser?.Rol?.NombreRol == "Supervisor";
        public static bool EsTrabajador => CurrentUser?.Rol?.NombreRol == "Trabajador";
    }
}
