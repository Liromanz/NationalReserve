using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для SupplyView.xaml
    /// </summary>
    public partial class SupplyView : UserControl
    {
        public SupplyViewModel ViewModel => DataContext as SupplyViewModel;
        public SupplyView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Supply();
        }
    }
}
