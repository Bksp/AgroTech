using Wpf.Ui.Controls;

namespace AgroTech.UI
{
    public partial class Dashboard : FluentWindow
    {
        public Dashboard()
        {
            InitializeComponent();
            
            // Navegar automáticamente a la primera página al cargar
            Loaded += (s, e) => RootNavigation.Navigate(typeof(Views.ParcelasPage));
        }
    }
}
