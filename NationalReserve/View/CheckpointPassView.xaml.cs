using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для CheckpointPassView.xaml
    /// </summary>
    public partial class CheckpointPassView : UserControl
    {
        public CheckpointPassViewModel ViewModel => DataContext as CheckpointPassViewModel;
        public CheckpointPassView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new CheckpointPass { PassType = "Вход"};
        }
    }
}
