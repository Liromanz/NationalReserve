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
    }
}
