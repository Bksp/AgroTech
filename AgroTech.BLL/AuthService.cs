using AgroTech.DAL;
using AgroTech.DAL.Models;
using System.Linq;

namespace AgroTech.BLL
{
    public class AuthService
    {
        public Usuario? Authenticate(string email, string password)
        {
            using (var db = new AgroTechDbContext())
            {
                // Mejorar el sistema de validacion xd
                var user = db.Usuarios.FirstOrDefault(u => u.CorreoElectronico == email && u.PasswordHash == password);
                return user;
            }
        }
    }
}
