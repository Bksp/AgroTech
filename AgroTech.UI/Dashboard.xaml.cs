using AgroTech.UI.ViewModels;
using Wpf.Ui.Controls;

namespace AgroTech.UI
{
    public partial class Dashboard : FluentWindow
    {
        public Dashboard()
        {
            InitializeComponent();

            var viewModel = new DashboardViewModel();
            viewModel.RequestClose = this.Close;
            viewModel.RequestShowLogin = () =>
            {
                var login = new MainWindow();
                login.Show();
            };
            this.DataContext = viewModel;

            // Navegar automáticamente a la primera página al cargar
            Loaded += (s, e) => RootNavigation.Navigate(typeof(Views.ParcelasPage));
        }
    }
}
