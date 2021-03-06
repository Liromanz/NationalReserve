using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class PlantListViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<PlantList> UpdatedCollection { get; set; }
        public ObservableCollection<PlantList> AddedCollection { get; set; }

        private ObservableCollection<PlantList> _deletedCollection;
        public ObservableCollection<PlantList> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private PlantList _deleted;
        public PlantList Deleted
        {
            get => _deleted;
            set
            {
                _deleted = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Данные с верстки

        private ObservableCollection<PlantList> _plantLists;
        public ObservableCollection<PlantList> PlantLists
        {
            get => _plantLists;
            set
            {
                _plantLists = value;
                OnPropertyChanged();
            }
        }

        private PlantList _plantList;
        public PlantList PlantList
        {
            get => _plantList;
            set
            {
                if (value != null)
                {
                    if (Zones != null && Zones.Any())
                        Zone = Zones.FirstOrDefault(x => x.IdZone == value.IdZone);
                    if (Humans != null && Humans.Any())
                        Human = Humans.FirstOrDefault(x => x.IdHuman == value.IdHuman);
                    if (Supplies != null && Supplies.Any())
                        Supply = Supplies.FirstOrDefault(x => x.IdSupply == value.IdSupply);
                    UpdatedCollection.Add(value);
                }
                _plantList = value;
                OnPropertyChanged();
            }
        }

        private PlantList _selected;
        public PlantList Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    PlantList = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Zone> _zones;
        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set
            {
                _zones = value;
                OnPropertyChanged();
            }
        }

        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set
            {
                _zone = value;
                PlantList.IdZone = value?.IdZone ?? PlantList.IdZone;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Human> _humans;
        public ObservableCollection<Human> Humans
        {
            get => _humans;
            set
            {
                _humans = value;
                OnPropertyChanged();
            }
        }

        private Human _human;
        public Human Human
        {
            get => _human;
            set
            {
                _human = value;
                PlantList.IdHuman = value?.IdHuman ?? PlantList.IdHuman;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Supply> _supplies;
        public ObservableCollection<Supply> Supplies
        {
            get => _supplies;
            set
            {
                _supplies = value;
                OnPropertyChanged();
            }
        }

        private Supply _supply;
        public Supply Supply
        {
            get => _supply;
            set
            {
                _supply = value;
                PlantList.IdSupply = value?.IdSupply ?? PlantList.IdSupply;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public PlantListViewModel()
        {
            IsBusy = true;
            InitAsync();
            ReadAsync();
        }

        private async void InitAsync()
        {
            AddCommand = new RelayCommand(o => { AddObject(); });
            SaveCommand = new RelayCommand(o => { SaveAsync(); });
            PhysicalDeleteCommand = new RelayCommand(o => { PhysicalDelete(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });
            ExportCommand = new RelayCommand(o => { ExportTable(); });
            ImportCommand = new RelayCommand(o => { ImportTable(); });

            Zones = await ApiConnector.GetAll<Zone>("Zones");
            var listHumans = await ApiConnector.GetAll<Human>("Humen");
            Humans = listHumans != null ?
                new ObservableCollection<Human>(listHumans.Where(x => x.IsStaff == 1)) :
                new ObservableCollection<Human>();
            Supplies = await ApiConnector.GetAll<Supply>("Supplies");
        }

        #region DataHandler

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<PlantList>();
            UpdatedCollection = new ObservableCollection<PlantList>();
            DeletedCollection = new ObservableCollection<PlantList>();

            PlantList = new PlantList();
            var fullTableList = await ApiConnector.GetAll<PlantList>("PlantLists");
            PlantLists = new ObservableCollection<PlantList>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<PlantList>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            PlantList.IdPlant = null;
            PlantLists.Add((PlantList)PlantList.Clone());
            AddedCollection.Add((PlantList)PlantList.Clone());

            Selected = new PlantList();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("PlantLists", Deleted.IdPlant.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (Selected?.IdPlant != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                PlantLists.Remove(Selected);

                Selected = new PlantList();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                PlantLists.Add(Deleted);
                DeletedCollection.Remove(Deleted);
            }
        }

        public async void SaveAsync()
        {
            IsBusy = true;
            if (!AddedCollection.Any() && !UpdatedCollection.Any() && !DeletedCollection.Any())
            {
                IsBusy = false;
                return;
            }
            try
            {
                StringBuilder allMessageBuilder = new StringBuilder();
                foreach (var addedHuman in AddedCollection)
                {
                    var addMessage = await ApiConnector.AddData<PlantList>("PlantLists", addedHuman);
                    allMessageBuilder.Append($"{addedHuman.Name}: {addMessage}\n");
                }
                foreach (var updatedHuman in UpdatedCollection.Where(x => x.IdPlant != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("PlantLists", updatedHuman, updatedHuman.IdPlant.Value);
                    allMessageBuilder.Append($"{updatedHuman.Name}: {updateMessage}\n");
                }
                MessageBox.Show(allMessageBuilder.ToString());
                ReadAsync();
            }
            catch (Exception e)
            {
                IsBusy = false;
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
        }
        public void ExportTable()
        {
            List<string> exportList = new List<string>();
            foreach (var item in PlantLists)
                exportList.Add($"{item.IdPlant}, {item.Name}, {item.IdZone}, {item.IdHuman}, " +
                               $"{item.DateGarden}, {item.Amount}, {item.DaysToCheck}, " +
                               $"{item.LastCheck}, {item.IdSupply}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "PlantLists");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(10);
            try
            {
                foreach (var items in imported)
                {
                    var item = new PlantList
                    {
                        IdPlant = null,
                        Name = items[1],
                        IdZone = Convert.ToInt32(items[2]),
                        IdHuman = Convert.ToInt32(items[3]),
                        DateGarden = Convert.ToDateTime(items[4]),
                        Amount = Convert.ToInt32(items[5]),
                        DaysToCheck = Convert.ToInt32(items[6]),
                        LastCheck = Convert.ToDateTime(items[7]),
                        IdSupply = Convert.ToInt32(items[8]),
                        IsDeleted = Convert.ToBoolean(items[9])
                    };
                    AddedCollection.Add(item);
                    PlantLists.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        public string ValidationErrorMessage()
        {
            if (Human == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(PlantList.Name)) return "Поле \"Наименование\" не заполнено";
            if (PlantList.DateGarden > DateTime.Now) return "Поле \"Дата посадки\" не может быть в будущем";
            if (PlantList.DateGarden.Year < 2010) return "Минимальное значение поля \"Дата посадки\" - 2010 год";
            if (PlantList.Amount <= 0) return "Поле \"Количество\" не может быть отрицательным";
            if (PlantList.DaysToCheck <= 0) return "Поле \"Дни до следующей проверки\" не может быть отрицательным";
            if (PlantList.LastCheck > DateTime.Now) return "Поле \"Последняя проверка\" не может быть в будущем";
            if (PlantList.LastCheck.Year < 2010) return "Минимальное значение поля \"Последняя проверка\" - 2010 год";
            if (!Zones.Select(x => x.IdZone).Contains(PlantList.IdZone)) return "Поле \"Зона\" не выбрано";
            if (!Humans.Select(x => x.IdHuman).Contains(PlantList.IdHuman)) return "Поле \"Проверяющий\" не выбрано";
            if (!Supplies.Select(x => x.IdSupply).Contains(PlantList.IdSupply)) return "Поле \"Поставщик\" не выбрано";

            return String.Empty;
        }

        #endregion
    }
}
