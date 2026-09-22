using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroTech.BLL
{
    public class UsuarioService
    {
        private static readonly Regex CorreoInstitucional =
            new(@"^[A-Za-z0-9+_.-]+@agrotech\.cl$", RegexOptions.Compiled);
        private readonly Func<AgroTechDbContext> _contextFactory;

        public UsuarioService() : this(() => new AgroTechDbContext()) { }

        public UsuarioService(Func<AgroTechDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Rol> ObtenerRoles()
        {
            using var db = _contextFactory();
            return db.Roles.AsNoTracking().OrderBy(r => r.IdRol).ToList();
        }

        public List<Usuario> ObtenerTodos()
        {
            using var db = _contextFactory();
            return db.Usuarios.Include(u => u.Rol).AsNoTracking().OrderBy(u => u.NombreCompleto).ToList();
        }

        public Usuario RegistrarUsuario(string nombreCompleto, string correo, string passwordTemporal, int idRol)
        {
            ValidarDatosBasicos(nombreCompleto, correo, idRol);

            if (string.IsNullOrWhiteSpace(passwordTemporal) || passwordTemporal.Length < 6)
                throw new ArgumentException("La contraseña temporal debe tener al menos 6 caracteres.");

            using var db = _contextFactory();

            correo = correo.Trim();
            if (db.Usuarios.Any(u => u.CorreoElectronico == correo))
                throw new ArgumentException("Ya existe un usuario registrado con ese correo.");

            if (!db.Roles.Any(r => r.IdRol == idRol))
                throw new ArgumentException("El rol seleccionado no es válido.");

            var usuario = new Usuario
            {
                NombreCompleto = nombreCompleto.Trim(),
                CorreoElectronico = correo,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordTemporal),
                IdRol = idRol,
                Estado = "Activo",
                FechaCreacion = DateTime.Now
            };

            db.Usuarios.Add(usuario);
            db.SaveChanges();
            return usuario;
        }

        public void ModificarUsuario(int idUsuario, string nombreCompleto, string correo, int idRol, string? nuevaPasswordOpcional)
        {
            ValidarDatosBasicos(nombreCompleto, correo, idRol);

            using var db = _contextFactory();
            var usuario = db.Usuarios.Find(idUsuario)
                ?? throw new ArgumentException("El usuario indicado no existe.");

            correo = correo.Trim();
            if (db.Usuarios.Any(u => u.CorreoElectronico == correo && u.IdUsuario != idUsuario))
                throw new ArgumentException("Ya existe otro usuario registrado con ese correo.");

            if (!db.Roles.Any(r => r.IdRol == idRol))
                throw new ArgumentException("El rol seleccionado no es válido.");

            usuario.NombreCompleto = nombreCompleto.Trim();
            usuario.CorreoElectronico = correo;
            usuario.IdRol = idRol;

            if (!string.IsNullOrWhiteSpace(nuevaPasswordOpcional))
            {
                if (nuevaPasswordOpcional.Length < 6)
                    throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.");
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPasswordOpcional);
            }

            db.SaveChanges();
        }

        public void SuspenderUsuario(int idUsuario, int idUsuarioQueEjecutaLaAccion)
        {
            if (idUsuario == idUsuarioQueEjecutaLaAccion)
                throw new InvalidOperationException("No puedes suspender tu propia cuenta mientras tienes la sesión activa.");

            using var db = _contextFactory();
            var usuario = db.Usuarios.Find(idUsuario)
                ?? throw new ArgumentException("El usuario indicado no existe.");

            usuario.Estado = "Suspendido";
            db.SaveChanges();
        }

        public void ReactivarUsuario(int idUsuario)
        {
            using var db = _contextFactory();
            var usuario = db.Usuarios.Find(idUsuario)
                ?? throw new ArgumentException("El usuario indicado no existe.");

            usuario.Estado = "Activo";
            db.SaveChanges();
        }

        private static void ValidarDatosBasicos(string nombreCompleto, string correo, int idRol)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                throw new ArgumentException("El nombre completo es obligatorio.");

            if (string.IsNullOrWhiteSpace(correo) || !CorreoInstitucional.IsMatch(correo.Trim()))
                throw new ArgumentException("El correo debe tener el formato institucional @agrotech.cl.");

            if (idRol <= 0)
                throw new ArgumentException("Debe seleccionar un rol.");
        }
    }
}
