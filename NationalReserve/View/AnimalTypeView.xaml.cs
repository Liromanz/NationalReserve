﻿using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для AnimalTypeView.xaml
    /// </summary>
    public partial class AnimalTypeView : UserControl
    {
        public AnimalTypeViewModel ViewModel => DataContext as AnimalTypeViewModel;
        public AnimalTypeView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new AnimalType();
        }
    }
}