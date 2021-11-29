using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для PaymentTypeView.xaml
    /// </summary>
    public partial class PaymentTypeView : UserControl
    {
        public PaymentTypeViewModel ViewModel => DataContext as PaymentTypeViewModel;
        public PaymentTypeView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new PaymentType();
        }
    }
}
