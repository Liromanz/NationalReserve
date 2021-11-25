using System;
using System.Windows;
using System.Windows.Input;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{

    class MainViewModel : ObservableObject
    {
        #region Команды
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand AnimalViewCommand { get; set; }
        public RelayCommand AnimalFeedViewCommand { get; set; }
        #endregion

        #region Верстки
        public HumanViewModel HumanVm { get; set; }
        public AnimalViewModel AnimalVm { get; set; }
        public AnimalFeedViewModel AnimalFeedVm { get; set; }
        #endregion

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HumanVm = new HumanViewModel();
            AnimalVm = new AnimalViewModel();
            AnimalFeedVm = new AnimalFeedViewModel();
            ChangeCurrentView(HumanVm);

            HomeViewCommand = new RelayCommand(o => { ChangeCurrentView(HumanVm); });
            AnimalViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalVm); });
            AnimalFeedViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalFeedVm); });
        }

        private void ChangeCurrentView(object viewToChange)
        {
            CurrentView = viewToChange;
        }
    }
}