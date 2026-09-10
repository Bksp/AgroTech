using System;
using System.Windows;
using AgroTech.BLL;
using AgroTech.DAL;

namespace AgroTech.UI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var authService = new AuthService();

            try
            {
                var usuario = authService.Authenticate(txtUsername.Text, txtPassword.Password);

                SessionManager.IniciarSesion(usuario);

                var dashboard = new Dashboard();
                dashboard.Show();
                this.Close();
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message, "No fue posible iniciar sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos MySQL. Verifica que el servidor esté levantado " +
                    "y que hayas ejecutado docs/bbdd.sql, y revisa la cadena de conexión en AgroTechDbContext.cs.\n\n" +
                    $"Detalle técnico: {ex.Message}",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}