using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для AnimalView.xaml
    /// </summary>
    public partial class AnimalView : UserControl
    {
        public AnimalViewModel ViewModel => DataContext as AnimalViewModel;

        public AnimalView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.SelectedAnimal = new Animal();
        }
    }
}
