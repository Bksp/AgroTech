using AgroTech.DAL;
using Wpf.Ui.Controls;

namespace AgroTech.UI
{
    public partial class Dashboard : FluentWindow
    {
        public Dashboard()
        {
            InitializeComponent();

            var usuario = SessionManager.CurrentUser;
            txtUsuarioSesion.Text = usuario != null
                ? $"{usuario.NombreCompleto} · {usuario.Rol?.NombreRol}"
                : string.Empty;

            // RF-03 (RBAC): sólo el Administrador gestiona usuarios.
            navUsuarios.Visibility = SessionManager.EsAdministrador
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

            // Navegar automáticamente a la primera página al cargar
            Loaded += (s, e) => RootNavigation.Navigate(typeof(Views.ParcelasPage));
        }

        private void CerrarSesion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SessionManager.CerrarSesion();
            var login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
