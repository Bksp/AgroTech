using AgroTech.DAL;
using System.Linq;
using System.Windows;
using Wpf.Ui.Controls;

namespace AgroTech.UI
{
    public partial class Dashboard : FluentWindow
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AgroTechDbContext())
            {
                // MVP: Cargar las parcelas desde SQLite
                var parcelas = db.Parcelas.ToList();
                GridParcelas.ItemsSource = parcelas;
            }
        }

        private void NuevaParcela_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Módulo en construcción para el siguiente avance.", "AgroTech", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
