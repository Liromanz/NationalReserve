using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для AnimalFeedView.xaml
    /// </summary>
    public partial class AnimalFeedView : UserControl
    {
        public AnimalFeedViewModel ViewModel => DataContext as AnimalFeedViewModel;
        public AnimalFeedView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new AnimalFeed();
        }
    }
}
