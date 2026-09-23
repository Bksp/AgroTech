using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AgroTech.BLL;
using AgroTech.DAL;
using AgroTech.DAL.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class UsuariosViewModel : ObservableObject
    {
        private readonly UsuarioService _usuarioService = new();

        [ObservableProperty]
        private ObservableCollection<Usuario> _usuarios = new();

        [ObservableProperty]
        private ObservableCollection<Rol> _roles = new();

        [ObservableProperty]
        private string _nombreCompleto = string.Empty;

        [ObservableProperty]
        private string _correo = string.Empty;

        [ObservableProperty]
        private Rol? _selectedRol;

        [ObservableProperty]
        private Usuario? _selectedUsuario;

        public UsuariosViewModel()
        {
            LoadData();
        }

        partial void OnSelectedUsuarioChanged(Usuario? value)
        {
            if (value != null)
            {
                NombreCompleto = value.NombreCompleto;
                Correo = value.CorreoElectronico;
                SelectedRol = Roles.FirstOrDefault(r => r.IdRol == value.IdRol);
            }
        }

        private void LoadData()
        {
            try
            {
                Roles = new ObservableCollection<Rol>(_usuarioService.ObtenerRoles());
                Usuarios = new ObservableCollection<Usuario>(_usuarioService.ObtenerTodos());

                if (SelectedRol == null && Roles.Any())
                {
                    SelectedRol = Roles.First();
                }
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
        private void Limpiar(Wpf.Ui.Controls.PasswordBox passwordBox)
        {
            SelectedUsuario = null;
            NombreCompleto = string.Empty;
            Correo = string.Empty;
            SelectedRol = Roles.FirstOrDefault();
            if (passwordBox != null)
            {
                passwordBox.Password = string.Empty;
            }
        }

        [RelayCommand]
        private void Registrar(Wpf.Ui.Controls.PasswordBox passwordBox)
        {
            try
            {
                var idRol = SelectedRol?.IdRol ?? 0;

                _usuarioService.RegistrarUsuario(
                    NombreCompleto,
                    Correo,
                    passwordBox?.Password ?? string.Empty,
                    idRol);

                MessageBox.Show("Usuario registrado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                Limpiar(passwordBox!);
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Dato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo registrar el usuario en MySQL.\n\nDetalle: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Modificar(Wpf.Ui.Controls.PasswordBox passwordBox)
        {
            if (SelectedUsuario == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var idRol = SelectedRol?.IdRol ?? 0;
                string? nuevaPass = (passwordBox != null && !string.IsNullOrWhiteSpace(passwordBox.Password)) 
                    ? passwordBox.Password : null;

                _usuarioService.ModificarUsuario(
                    SelectedUsuario.IdUsuario,
                    NombreCompleto,
                    Correo,
                    idRol,
                    nuevaPass);

                MessageBox.Show("Usuario modificado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                Limpiar(passwordBox!);
                LoadData();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Dato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo modificar el usuario en MySQL.\n\nDetalle: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Eliminar(Wpf.Ui.Controls.PasswordBox passwordBox)
        {
            if (SelectedUsuario == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Suspender el acceso de '{SelectedUsuario.NombreCompleto}'? No podrá iniciar sesión hasta ser reactivado.",
                "Confirmar suspensión", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                var idUsuarioActual = SessionManager.CurrentUser?.IdUsuario ?? 0;
                _usuarioService.SuspenderUsuario(SelectedUsuario.IdUsuario, idUsuarioActual);

                MessageBox.Show("Usuario suspendido correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                Limpiar(passwordBox!);
                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Acción no permitida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo suspender el usuario en MySQL.\n\nDetalle: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Reactivar(Wpf.Ui.Controls.PasswordBox passwordBox)
        {
            if (SelectedUsuario == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _usuarioService.ReactivarUsuario(SelectedUsuario.IdUsuario);
                MessageBox.Show("Usuario reactivado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                Limpiar(passwordBox!);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo reactivar el usuario en MySQL.\n\nDetalle: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
