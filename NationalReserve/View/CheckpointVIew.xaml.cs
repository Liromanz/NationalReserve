using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для CheckpointVIew.xaml
    /// </summary>
    public partial class CheckpointVIew : UserControl
    {
        public CheckpointViewModel ViewModel => DataContext as CheckpointViewModel;
        public CheckpointVIew()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Checkpoint();
        }
    }
}
