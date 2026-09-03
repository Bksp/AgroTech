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
                // Simple MVP validation (in a real app, hash the password and compare)
                var user = db.Usuarios.FirstOrDefault(u => u.CorreoElectronico == email && u.PasswordHash == password);
                return user;
            }
        }
    }
}
