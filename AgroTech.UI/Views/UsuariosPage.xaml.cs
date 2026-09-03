using AgroTech.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroTech.UI.Views
{
    public partial class UsuariosPage : Page
    {
        public UsuariosPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AgroTechDbContext())
            {
                var usuarios = db.Usuarios.Include(u => u.Rol).ToList();
                GridUsuarios.ItemsSource = usuarios;
            }
        }

        private void EliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Usuario eliminado (Simulado)", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ModificarUsuario_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Usuario modificado (Simulado)", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RegistrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nuevo usuario registrado (Simulado)", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
