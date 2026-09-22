using System;
using System.Windows;
using System.Windows.Controls;
using AgroTech.BLL;
using AgroTech.DAL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public Action? RequestClose { get; set; }
        public Action? RequestShowDashboard { get; set; }

        [ObservableProperty]
        private string _username = string.Empty;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        private void Login(object? parameter)
        {
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
