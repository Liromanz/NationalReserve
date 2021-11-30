using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для SupplierView.xaml
    /// </summary>
    public partial class SupplierView : UserControl
    {
        public SupplierViewModel ViewModel => DataContext as SupplierViewModel;
        public SupplierView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Supplier();
        }
    }
}
