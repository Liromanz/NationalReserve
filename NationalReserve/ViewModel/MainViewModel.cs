using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{

    public class MainViewModel : ObservableObject
    {
        #region Команды
        public RelayCommand AnimalViewCommand { get; set; }
        public RelayCommand AnimalFeedViewCommand { get; set; }
        public RelayCommand AnimalTypeViewCommand { get; set; }
        public RelayCommand CheckpointViewCommand { get; set; }
        public RelayCommand CheckpointPassViewCommand { get; set; }
        public RelayCommand SecurityListViewCommand { get; set; }
        public RelayCommand ZoneViewCommand { get; set; }
        public RelayCommand HumanViewCommand { get; set; }
        public RelayCommand StaffDocumentCommand { get; set; }
        public RelayCommand RoleViewCommand { get; set; }
        public RelayCommand MaterialViewCommand { get; set; }
        public RelayCommand MaterialTypeViewCommand { get; set; }
        public RelayCommand SupplierViewCommand { get; set; }
        public RelayCommand SupplyViewCommand { get; set; }
        public RelayCommand SponsorshipViewCommand { get; set; }
        public RelayCommand PaymentTypeCommand { get; set; }
        public RelayCommand PlantListCommand { get; set; }
        #endregion

        #region Верстки
        public AnimalViewModel AnimalVm { get; set; }
        public AnimalFeedViewModel AnimalFeedVm { get; set; }
        public AnimalTypeViewModel AnimalTypeVm { get; set; }
        public CheckpointViewModel CheckpointVm { get; set; }
        public CheckpointPassViewModel CheckpointPassVm { get; set; }
        public SecurityListViewModel SecurityListVm { get; set; }
        public ZoneViewModel ZoneVm { get; set; }
        public HumanViewModel HumanVm { get; set; }
        public StaffDocumentViewModel StaffDocumentVm { get; set; }
        public RoleViewModel RoleVm { get; set; }
        public MaterialViewModel MaterialVm { get; set; }
        public MaterialTypeViewModel MaterialTypeVm { get; set; }
        public SupplierViewModel SupplierVm { get; set; }
        public SupplyViewModel SupplyVm { get; set; }
        public SponsorshipViewModel SponsorshipVm { get; set; }
        public PaymentTypeViewModel PaymentTypeVm { get; set; }
        public PlantListViewModel PlantListVm { get; set; }
        #endregion

        private Authorization _authorization;
        public Authorization Authorizated
        {
            get => _authorization;
            set
            {
                _authorization = value; 
                OnPropertyChanged();
            }
        }

        private bool _nothingSelected;

        public bool NothingSelected
        {
            get { return _nothingSelected; }
            set
            {
                _nothingSelected = value;
                OnPropertyChanged();
            }
        }


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
            NothingSelected = true;

            AnimalVm = new AnimalViewModel();
            AnimalFeedVm = new AnimalFeedViewModel();
            AnimalTypeVm = new AnimalTypeViewModel();
            CheckpointVm = new CheckpointViewModel();
            CheckpointPassVm = new CheckpointPassViewModel();
            SecurityListVm = new SecurityListViewModel();
            ZoneVm = new ZoneViewModel();
            HumanVm = new HumanViewModel();
            RoleVm = new RoleViewModel();
            StaffDocumentVm = new StaffDocumentViewModel();
            MaterialVm = new MaterialViewModel();
            MaterialTypeVm = new MaterialTypeViewModel();
            SupplierVm = new SupplierViewModel();
            SupplyVm = new SupplyViewModel();
            SponsorshipVm = new SponsorshipViewModel();
            PaymentTypeVm = new PaymentTypeViewModel();
            PlantListVm = new PlantListViewModel();

            AnimalViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalVm); });
            AnimalFeedViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalFeedVm); });
            AnimalTypeViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalTypeVm); });
            CheckpointViewCommand = new RelayCommand(o => { ChangeCurrentView(CheckpointVm); });
            CheckpointPassViewCommand = new RelayCommand(o => { ChangeCurrentView(CheckpointPassVm); });
            SecurityListViewCommand = new RelayCommand(o => { ChangeCurrentView(SecurityListVm); });
            ZoneViewCommand = new RelayCommand(o => { ChangeCurrentView(ZoneVm); });
            HumanViewCommand = new RelayCommand(o => { ChangeCurrentView(HumanVm); });
            StaffDocumentCommand = new RelayCommand(o => { ChangeCurrentView(StaffDocumentVm); });
            RoleViewCommand = new RelayCommand(o => { ChangeCurrentView(RoleVm); });
            MaterialViewCommand = new RelayCommand(o => { ChangeCurrentView(MaterialVm); });
            MaterialTypeViewCommand = new RelayCommand(o => { ChangeCurrentView(MaterialTypeVm); });
            SupplierViewCommand = new RelayCommand(o => { ChangeCurrentView(SupplierVm); });
            SupplyViewCommand = new RelayCommand(o => { ChangeCurrentView(SupplyVm); });
            SponsorshipViewCommand = new RelayCommand(o => { ChangeCurrentView(SponsorshipVm); });
            PaymentTypeCommand = new RelayCommand(o => { ChangeCurrentView(PaymentTypeVm); });
            PlantListCommand = new RelayCommand(o => { ChangeCurrentView(PlantListVm); });
        }

        private void ChangeCurrentView(object viewToChange)
        {
            CurrentView = viewToChange;
            NothingSelected = false;
        }
    }
}