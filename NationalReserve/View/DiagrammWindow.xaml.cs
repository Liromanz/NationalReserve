using System.Windows;
using System.Windows.Input;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для DiagrammWindow.xaml
    /// </summary>
    public partial class DiagrammWindow : Window
    {
        public DiagrammWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog(object sender, MouseButtonEventArgs e) => Close();

        private void DragWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

    }
}
