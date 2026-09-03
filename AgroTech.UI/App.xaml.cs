using System.Configuration;
using System.Data;
using System.Windows;

using System;

namespace AgroTech.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Error (UI): {e.Exception.Message}\n\nStack: {e.Exception.StackTrace}", "Crash Report", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception ex)
            {
                MessageBox.Show($"Error (Core): {ex.Message}\n\nStack: {ex.StackTrace}", "Crash Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
