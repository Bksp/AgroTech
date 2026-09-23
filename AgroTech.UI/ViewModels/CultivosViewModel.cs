using System;
using System.Collections.ObjectModel;
using System.Windows;
using AgroTech.BLL;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class CultivosViewModel : ObservableObject
    {
        private readonly CultivoService _cultivoService = new();

        [ObservableProperty]
        private ObservableCollection<Cultivo> _cultivos = new();

        [ObservableProperty]
        private Cultivo? _cultivoSeleccionado;

        [ObservableProperty]
        private string _estadoSeleccionado = "Activo";

        [ObservableProperty]
        private DateTime? _fechaSiembraSeleccionada;

        [ObservableProperty]
        private DateTime? _fechaCosechaSeleccionada;

        public bool PuedeEditar => !SessionManager.EsTrabajador;
        public Visibility AccionesVisibility => PuedeEditar ? Visibility.Visible : Visibility.Collapsed;

        public ObservableCollection<string> EstadosDisponibles { get; } = new()
        {
            "Activo",
            "Cosechado",
            "Perdido por Plaga"
        };

        public CultivosViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _cultivoService.ObtenerTodos();
                Cultivos = new ObservableCollection<Cultivo>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los cultivos.\n\nDetalle: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool HayCultivoSeleccionado => CultivoSeleccionado != null;

        partial void OnCultivoSeleccionadoChanged(Cultivo? value)
        {
            OnPropertyChanged(nameof(HayCultivoSeleccionado));
            if (value != null)
            {
                EstadoSeleccionado = value.Estado ?? "Activo";
                FechaSiembraSeleccionada = value.FechaSiembra;
                FechaCosechaSeleccionada = value.FechaEstimadaCosecha;
            }
            else
            {
                EstadoSeleccionado = "Activo";
                FechaSiembraSeleccionada = null;
                FechaCosechaSeleccionada = null;
            }
        }

        [RelayCommand]
        private void GuardarCambios()
        {
            if (CultivoSeleccionado == null)
            {
                MessageBox.Show("Debes seleccionar un cultivo primero.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!FechaSiembraSeleccionada.HasValue || !FechaCosechaSeleccionada.HasValue)
            {
                MessageBox.Show("Las fechas no pueden estar vacías.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _cultivoService.ActualizarCultivo(
                    CultivoSeleccionado.IdCultivo, 
                    FechaSiembraSeleccionada.Value, 
                    FechaCosechaSeleccionada.Value, 
                    EstadoSeleccionado);
                
                MessageBox.Show("Cultivo actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                CultivoSeleccionado = null;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += $"\nDetalle: {ex.InnerException.Message}";
                MessageBox.Show(msg, "Error al actualizar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Eliminar()
        {
            if (CultivoSeleccionado == null)
            {
                MessageBox.Show("Debes seleccionar un cultivo para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Estás seguro que deseas eliminar el cultivo {CultivoSeleccionado.TipoCultivo}?", 
                                         "Confirmar eliminación", 
                                         MessageBoxButton.YesNo, 
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _cultivoService.EliminarCultivo(CultivoSeleccionado.IdCultivo);
                    MessageBox.Show("Cultivo eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    CultivoSeleccionado = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
