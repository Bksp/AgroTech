using System;
using System.Linq;
using System.Text.RegularExpressions;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroTech.BLL
{
    
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }

    public class AuthService
    {
        private static readonly Regex CorreoInstitucional =
            new(@"^[A-Za-z0-9+_.-]+@agrotech\.cl$", RegexOptions.Compiled);
        private readonly Func<AgroTechDbContext> _contextFactory;

        public AuthService() : this(() => new AgroTechDbContext()) { }

        public AuthService(Func<AgroTechDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Usuario Authenticate(string correo, string contrasenaRaw)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasenaRaw))
                throw new AuthenticationException("Debe ingresar correo y contraseña.");

            correo = correo.Trim();

            if (!CorreoInstitucional.IsMatch(correo))
                throw new AuthenticationException("El correo debe tener el formato institucional @agrotech.cl.");

            using var db = _contextFactory();

            var usuario = db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.CorreoElectronico == correo);

            if (usuario == null)
                throw new AuthenticationException("Las credenciales ingresadas son incorrectas.");

            if (!string.Equals(usuario.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
                throw new AuthenticationException("La cuenta de usuario se encuentra suspendida.");

            bool passwordValida;
            try
            {
                passwordValida = BCrypt.Net.BCrypt.Verify(contrasenaRaw, usuario.PasswordHash);
            }
            catch (Exception)
            {
                passwordValida = false;
            }

            if (!passwordValida)
                throw new AuthenticationException("Las credenciales ingresadas son incorrectas.");

            return usuario;
        }
    }
}
