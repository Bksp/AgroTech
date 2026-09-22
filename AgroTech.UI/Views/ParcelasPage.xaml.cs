using System.Windows.Controls;
using AgroTech.UI.ViewModels;

namespace AgroTech.UI.Views
{
    public partial class ParcelasPage : Page
    {
        public ParcelasPage()
        {
            InitializeComponent();
            this.DataContext = new ParcelasViewModel();
        }
    }
}
