using System.Windows.Controls;
using AgroTech.UI.ViewModels;

namespace AgroTech.UI.Views
{
    public partial class UsuariosPage : Page
    {
        public UsuariosPage()
        {
            InitializeComponent();
            this.DataContext = new UsuariosViewModel();
        }
    }
}
