using System;
using System.Windows;
using System.Windows.Controls;
using AgroTech.BLL;
using AgroTech.DAL;
using AgroTech.DAL.Models;

namespace AgroTech.UI.Views
{
    public partial class UsuariosPage : Page
    {
        private readonly UsuarioService _usuarioService = new();
        private Usuario? _usuarioSeleccionado;

        public UsuariosPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                cmbRoles.ItemsSource = _usuarioService.ObtenerRoles();
                GridUsuarios.ItemsSource = _usuarioService.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo leer la base de datos MySQL. Verifica que el servidor esté levantado " +
                    $"y que hayas ejecutado docs/bbdd.sql.\n\nDetalle: {ex.Message}",
                    "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GridUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _usuarioSeleccionado = GridUsuarios.SelectedItem as Usuario;
            if (_usuarioSeleccionado == null) return;

            txtNombreCompleto.Text = _usuarioSeleccionado.NombreCompleto;
            txtCorreo.Text = _usuarioSeleccionado.CorreoElectronico;
            txtPassTemp.Password = string.Empty;

            foreach (Rol rol in cmbRoles.Items)
            {
                if (rol.IdRol == _usuarioSeleccionado.IdRol)
                {
                    cmbRoles.SelectedItem = rol;
                    break;
                }
            }
        }

        private void LimpiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            _usuarioSeleccionado = null;
            GridUsuarios.SelectedItem = null;
            txtNombreCompleto.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtPassTemp.Password = string.Empty;
            cmbRoles.SelectedIndex = -1;
        }

        private void RegistrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var idRol = (cmbRoles.SelectedItem as Rol)?.IdRol ?? 0;

                _usuarioService.RegistrarUsuario(
                    txtNombreCompleto.Text,
                    txtCorreo.Text,
                    txtPassTemp.Password,
                    idRol);

                MessageBox.Show("Usuario registrado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarUsuario_Click(sender, e);
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

        private void ModificarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var idRol = (cmbRoles.SelectedItem as Rol)?.IdRol ?? 0;

                _usuarioService.ModificarUsuario(
                    _usuarioSeleccionado.IdUsuario,
                    txtNombreCompleto.Text,
                    txtCorreo.Text,
                    idRol,
                    string.IsNullOrWhiteSpace(txtPassTemp.Password) ? null : txtPassTemp.Password);

                MessageBox.Show("Usuario modificado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarUsuario_Click(sender, e);
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

        /// <summary>RF-02: no hay borrado físico; se marca estado = 'Suspendido' para conservar la trazabilidad.</summary>
        private void EliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Suspender el acceso de '{_usuarioSeleccionado.NombreCompleto}'? No podrá iniciar sesión hasta ser reactivado.",
                "Confirmar suspensión", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                var idUsuarioActual = SessionManager.CurrentUser?.IdUsuario ?? 0;
                _usuarioService.SuspenderUsuario(_usuarioSeleccionado.IdUsuario, idUsuarioActual);

                MessageBox.Show("Usuario suspendido correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarUsuario_Click(sender, e);
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

        private void ReactivarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona primero un usuario de la tabla.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _usuarioService.ReactivarUsuario(_usuarioSeleccionado.IdUsuario);
                MessageBox.Show("Usuario reactivado correctamente.", "AgroTech", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarUsuario_Click(sender, e);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo reactivar el usuario en MySQL.\n\nDetalle: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
