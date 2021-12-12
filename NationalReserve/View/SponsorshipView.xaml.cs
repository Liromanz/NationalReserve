﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для SponsorshipView.xaml
    /// </summary>
    public partial class SponsorshipView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public SponsorshipViewModel ViewModel => DataContext as SponsorshipViewModel;
        public SponsorshipView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Sponsorship();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.Sponsorships = new ObservableCollection<Sponsorship>(
                    ViewModel.Sponsorships.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.Sponsorships = new ObservableCollection<Sponsorship>(
                    ViewModel.Sponsorships.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
