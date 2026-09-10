using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AgroTech.BLL;
using AgroTech.DAL;

namespace AgroTech.UI.Views
{
    public partial class ParcelasPage : Page
    {
        private readonly ParcelaService _parcelaService = new();
        private readonly CultivoService _cultivoService = new();

        public ParcelasPage()
        {
            InitializeComponent();
            AplicarPermisosPorRol();
            LoadData();
        }

        /// <summary>RF-03 (RBAC): el Trabajador sólo puede consultar, no registrar.</summary>
        private void AplicarPermisosPorRol()
        {
            if (!SessionManager.EsTrabajador) return;

            txtNombreParcela.IsEnabled = false;
            txtUbicacion.IsEnabled = false;
            txtDimensiones.IsEnabled = false;
            cmbTipoCultivo.IsEnabled = false;
            dpFechaSiembra.IsEnabled = false;
            dpFechaCosecha.IsEnabled = false;
            panelAccionesParcela.Visibility = Visibility.Collapsed;
        }

        private void LoadData()
        {
            try
            {
                GridParcelas.ItemsSource = _parcelaService.ObtenerTodas();
                GridCultivos.ItemsSource = _cultivoService.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo leer la base de datos MySQL. Verifica que el servidor esté levantado " +
                    $"y que hayas ejecutado docs/bbdd.sql.\n\nDetalle: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var usuarioActual = SessionManager.CurrentUser;
            if (usuarioActual == null)
            {
                MessageBox.Show("Tu sesión expiró. Vuelve a iniciar sesión.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validación de dimensiones en la capa de UI (RF-04: no llamar a la BD con datos claramente inválidos)
            if (!decimal.TryParse(txtDimensiones.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var dimensiones))
            {
                MessageBox.Show("Ingresa un valor numérico válido para las dimensiones (m²).", "Dato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var parcela = _parcelaService.RegistrarNuevaParcela(
                    txtNombreParcela.Text,
                    txtUbicacion.Text,
                    dimensiones,
                    usuarioActual.IdUsuario);

                // Si además se completaron los datos del cultivo, se registra el ciclo asociado.
                var tipoCultivoSeleccionado = (cmbTipoCultivo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                bool hayDatosDeCultivo = !string.IsNullOrWhiteSpace(tipoCultivoSeleccionado)
                                          && cmbTipoCultivo.SelectedIndex > 0
                                          && dpFechaSiembra.SelectedDate.HasValue
                                          && dpFechaCosecha.SelectedDate.HasValue;

                if (hayDatosDeCultivo)
                {
                    _cultivoService.RegistrarNuevoCultivo(
                        parcela.IdParcela,
                        tipoCultivoSeleccionado!,
                        dpFechaSiembra.SelectedDate!.Value,
                        dpFechaCosecha.SelectedDate!.Value,
                        usuarioActual.IdUsuario);

                    MessageBox.Show("Parcela y cultivo registrados correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Parcela registrada correctamente. Si además quieres asociar un cultivo, completa el tipo de cultivo y ambas fechas.",
                        "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LimpiarParcelas_Click(sender, e);
                LoadData();
            }
            catch (ArgumentException ex)
            {
                // Errores de validación de negocio (RF-01): ubicación, dimensiones, fechas, campos vacíos.
                MessageBox.Show(ex.Message, "Dato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo guardar el registro en MySQL.\n\nDetalle: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
