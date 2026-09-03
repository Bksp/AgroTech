using AgroTech.DAL;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AgroTech.UI.Views
{
    public partial class ParcelasPage : Page
    {
        public ParcelasPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AgroTechDbContext())
            {
                var parcelas = db.Parcelas.ToList();
                GridParcelas.ItemsSource = parcelas;
            }
        }

        private void LimpiarParcelas_Click(object sender, RoutedEventArgs e)
        {
            txtNombreParcela.Text = string.Empty;
            txtUbicacion.Text = string.Empty;
            txtDimensiones.Text = string.Empty;
            cmbTipoCultivo.SelectedIndex = 0;
            dpFechaSiembra.SelectedDate = null;
            dpFechaCosecha.SelectedDate = null;
        }

        private void GuardarParcela_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registro de parcela guardado (Simulado)", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
