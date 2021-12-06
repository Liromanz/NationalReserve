using System;
using System.Windows;
using System.Windows.Input;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void CloseApplication(object sender, MouseButtonEventArgs e) => Environment.Exit(0);

        private void ShowHelp(object sender, MouseButtonEventArgs e)
        {
            var helper = new HelperWindow();
            helper.ShowDialog();
        }

        private void DragWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
