using System.Windows;
using System.Windows.Input;

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
            var authService = new AgroTech.BLL.AuthService();
            var user = authService.Authenticate(txtUsername.Text, txtPassword.Password);

            if (user != null)
            {
                var dashboard = new Dashboard();
                dashboard.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Credenciales incorrectas.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}