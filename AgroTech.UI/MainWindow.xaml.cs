using System.Windows;
using AgroTech.UI.ViewModels;

namespace AgroTech.UI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            var viewModel = new LoginViewModel();
            viewModel.RequestClose = this.Close;
            viewModel.RequestShowDashboard = () => 
            {
                var dashboard = new Dashboard();
                dashboard.Show();
            };
            this.DataContext = viewModel;
        }
    }
}