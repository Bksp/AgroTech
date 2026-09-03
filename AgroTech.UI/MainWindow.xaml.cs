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
            MessageBox.Show("Inicio de sesión simulado. (Próximamente conexión a DB)", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}