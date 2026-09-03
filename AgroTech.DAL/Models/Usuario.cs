using System;

namespace AgroTech.DAL.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public Rol? Rol { get; set; }
    }
}
