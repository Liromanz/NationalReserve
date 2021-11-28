using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{

    class MainViewModel : ObservableObject
    {
        #region Команды
        public RelayCommand AnimalViewCommand { get; set; }
        public RelayCommand AnimalFeedViewCommand { get; set; }
        public RelayCommand AnimalTypeViewCommand { get; set; }
        public RelayCommand CheckpointViewCommand { get; set; }
        public RelayCommand CheckpointPassViewCommand { get; set; }
        public RelayCommand HumanViewCommand { get; set; }
        public RelayCommand RoleViewCommand { get; set; }
        public RelayCommand MaterialViewCommand { get; set; }
        #endregion

        #region Верстки
        public AnimalViewModel AnimalVm { get; set; }
        public AnimalFeedViewModel AnimalFeedVm { get; set; }
        public AnimalTypeViewModel AnimalTypeVm { get; set; }
        public CheckpointViewModel CheckpointVm { get; set; }
        public CheckpointPassViewModel CheckpointPassVm { get; set; }
        public HumanViewModel HumanVm { get; set; }
        public RoleViewModel RoleVm { get; set; }
        public MaterialViewModel MaterialVm { get; set; }
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
            AnimalVm = new AnimalViewModel();
            AnimalFeedVm = new AnimalFeedViewModel();
            AnimalTypeVm = new AnimalTypeViewModel();
            CheckpointVm = new CheckpointViewModel();
            CheckpointPassVm = new CheckpointPassViewModel();
            HumanVm = new HumanViewModel();
            RoleVm = new RoleViewModel();
            MaterialVm = new MaterialViewModel();
            ChangeCurrentView(HumanVm);

            AnimalViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalVm); });
            AnimalFeedViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalFeedVm); });
            AnimalTypeViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalTypeVm); });
            CheckpointViewCommand = new RelayCommand(o => { ChangeCurrentView(CheckpointVm); });
            CheckpointPassViewCommand = new RelayCommand(o => { ChangeCurrentView(CheckpointPassVm); });
            HumanViewCommand = new RelayCommand(o => { ChangeCurrentView(HumanVm); });
            RoleViewCommand = new RelayCommand(o => { ChangeCurrentView(RoleVm); });
            MaterialViewCommand = new RelayCommand(o => { ChangeCurrentView(MaterialVm); });
        }

        private void ChangeCurrentView(object viewToChange)
        {
            CurrentView = viewToChange;
        }
    }
}