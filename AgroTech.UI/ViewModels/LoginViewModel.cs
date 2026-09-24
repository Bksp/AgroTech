using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using AgroTech.BLL;
using AgroTech.DAL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class LoginViewModel : ObservableValidator
    {
        private readonly AuthService _authService;

        public Action? RequestClose { get; set; }
        public Action? RequestShowDashboard { get; set; }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+@agrotech\.cl$", ErrorMessage = "Debe ser un correo @agrotech.cl válido (solo letras, números, puntos, guiones).")]
        private string _username = string.Empty;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        private void Login(object? parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show("Por favor, ingresa un correo válido antes de continuar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parameter is not Wpf.Ui.Controls.PasswordBox passwordBox) return;

            try
            {
                var usuario = _authService.Authenticate(Username, passwordBox.Password);
                SessionManager.IniciarSesion(usuario);

                RequestShowDashboard?.Invoke();
                RequestClose?.Invoke();
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message, "No fue posible iniciar sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos de Supabase. Verifica tu conexión a internet, " +
                    "y revisa la cadena de conexión en AgroTechDbContext.cs.\n\n" +
                    $"Detalle técnico: {ex.Message}",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
