using System.Windows.Controls;
using AgroTech.UI.ViewModels;

namespace AgroTech.UI.Views
{
    public partial class CultivosPage : Page
    {
        public CultivosPage()
        {
            InitializeComponent();
            DataContext = new CultivosViewModel();
        }
    }
}
