using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для ZoneView.xaml
    /// </summary>
    public partial class ZoneView : UserControl
    {
        public ZoneViewModel ViewModel => DataContext as ZoneViewModel;
        public ZoneView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Zone();
        }
    }
}
