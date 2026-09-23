using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AgroTech.BLL;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class ParcelasViewModel : ObservableObject
    {
        private readonly ParcelaService _parcelaService = new();
        private readonly CultivoService _cultivoService = new();

        [ObservableProperty]
        private ObservableCollection<Parcela> _parcelas = new();

        [ObservableProperty]
        private ObservableCollection<Cultivo> _cultivos = new();

        [ObservableProperty]
        private string _nombreParcela = string.Empty;

        [ObservableProperty]
        private string _ubicacion = string.Empty;

        [ObservableProperty]
        private string _dimensiones = string.Empty;

        [ObservableProperty]
        private string? _selectedTipoCultivo = "Seleccione un cultivo...";

        public ObservableCollection<string> TiposDeCultivo { get; } = new()
        {
            "Seleccione un cultivo...",
            "Tomate",
            "Lechuga",
            "Zanahoria",
            "Cebolla",
            "Papa (Patata)",
            "Maíz (Choclo)",
            "Ajo",
            "Poroto (Frijol)",
            "Zapallo",
            "Sandía",
            "Melón",
            "Trigo",
            "Cereza",
            "Manzana"
        };

        [ObservableProperty]
        private DateTime? _fechaSiembra;

        [ObservableProperty]
        private DateTime? _fechaCosecha;

        // RBAC: el Trabajador sólo puede consultar, no registrar.
        public bool PuedeEditar => !SessionManager.EsTrabajador;
        
        public Visibility AccionesVisibility => PuedeEditar ? Visibility.Visible : Visibility.Collapsed;

        public ParcelasViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Parcelas = new ObservableCollection<Parcela>(_parcelaService.ObtenerTodas());
                Cultivos = new ObservableCollection<Cultivo>(_cultivoService.ObtenerTodos());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo leer la base de datos MySQL. Verifica que el servidor esté levantado " +
                    $"y que hayas ejecutado docs/bbdd.sql.\n\nDetalle: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Limpiar()
        {
            NombreParcela = string.Empty;
            Ubicacion = string.Empty;
            Dimensiones = string.Empty;
            SelectedTipoCultivo = TiposDeCultivo[0];
            FechaSiembra = null;
            FechaCosecha = null;
        }

        [RelayCommand]
        private void Guardar()
        {
            var usuarioActual = SessionManager.CurrentUser;
            if (usuarioActual == null)
            {
                MessageBox.Show("Tu sesión expiró. Vuelve a iniciar sesión.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(Dimensiones, NumberStyles.Number, CultureInfo.InvariantCulture, out var dimensionesDecimal))
            {
                MessageBox.Show("Ingresa un valor numérico válido para las dimensiones (m²).", "Dato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var parcela = _parcelaService.RegistrarNuevaParcela(
                    NombreParcela,
                    Ubicacion,
                    dimensionesDecimal,
                    usuarioActual.IdUsuario);

                var tipoCultivoSeleccionado = SelectedTipoCultivo;
                
                // Assuming "Seleccione un cultivo..." is either null because index 0 doesn't match a value, 
                // or we just check if it's an actual crop.
                bool isPlaceholder = tipoCultivoSeleccionado == "Seleccione un cultivo...";

                bool hayDatosDeCultivo = !string.IsNullOrWhiteSpace(tipoCultivoSeleccionado)
                                          && !isPlaceholder
                                          && FechaSiembra.HasValue
                                          && FechaCosecha.HasValue;

                if (hayDatosDeCultivo)
                {
                    _cultivoService.RegistrarNuevoCultivo(
                        parcela.IdParcela,
                        tipoCultivoSeleccionado!,
                        FechaSiembra!.Value,
                        FechaCosecha!.Value,
                        usuarioActual.IdUsuario);

                    MessageBox.Show("Parcela y cultivo registrados correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Parcela registrada correctamente. Si además quieres asociar un cultivo, completa el tipo de cultivo y ambas fechas.",
                        "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Limpiar();
                LoadData();
            }
            catch (ArgumentException ex)
            {
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
