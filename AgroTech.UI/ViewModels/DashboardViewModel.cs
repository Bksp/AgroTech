using System;
using System.Windows;
using AgroTech.DAL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroTech.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        public Action? RequestClose { get; set; }
        public Action? RequestShowLogin { get; set; }

        public string UsuarioSesion
        {
            get
            {
                var usuario = SessionManager.CurrentUser;
                return usuario != null ? $"{usuario.NombreCompleto} · {usuario.Rol?.NombreRol}" : string.Empty;
            }
        }

        public Visibility UsuariosVisibility => SessionManager.EsAdministrador ? Visibility.Visible : Visibility.Collapsed;

        [RelayCommand]
        private void CerrarSesion()
        {
            SessionManager.CerrarSesion();
            RequestShowLogin?.Invoke();
            RequestClose?.Invoke();
        }
    }
}
