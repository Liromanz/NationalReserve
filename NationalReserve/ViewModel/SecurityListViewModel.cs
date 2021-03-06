using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class SecurityListViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<SecurityList> UpdatedCollection { get; set; }
        public ObservableCollection<SecurityList> AddedCollection { get; set; }

        private ObservableCollection<SecurityList> _deletedCollection;
        public ObservableCollection<SecurityList> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private SecurityList _deleted;
        public SecurityList Deleted
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

        private ObservableCollection<SecurityList> _securityLists;
        public ObservableCollection<SecurityList> SecurityLists
        {
            get => _securityLists;
            set
            {
                _securityLists = value;
                OnPropertyChanged();
            }
        }

        private SecurityList _securityList;
        public SecurityList SecurityList
        {
            get => _securityList;
            set
            {
                if (value != null)
                {
                    if (Checkpoints != null && Checkpoints.Any())
                        Checkpoint = Checkpoints.FirstOrDefault(x => x.IdCheckpoint == value.IdCheckpoint);
                    if (Humans != null && Humans.Any())
                        Human = Humans.FirstOrDefault(x => x.IdHuman == value.IdHuman);
                    UpdatedCollection.Add(value);
                }
                _securityList = value;
                OnPropertyChanged();
            }
        }

        private SecurityList _selected;
        public SecurityList Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    SecurityList = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Checkpoint> _checkpoints;
        public ObservableCollection<Checkpoint> Checkpoints
        {
            get => _checkpoints;
            set
            {
                _checkpoints = value;
                OnPropertyChanged();
            }
        }

        private Checkpoint _checkpoint;
        public Checkpoint Checkpoint
        {
            get => _checkpoint;
            set
            {
                _checkpoint = value;
                SecurityList.IdCheckpoint = value?.IdCheckpoint ?? SecurityList.IdCheckpoint;
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
                SecurityList.IdHuman = value?.IdHuman ?? SecurityList.IdHuman;
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

        public SecurityListViewModel()
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

            Checkpoints = await ApiConnector.GetAll<Checkpoint>("Checkpoints");
            var listHumans = await ApiConnector.GetAll<Human>("Humen");
            Humans = listHumans != null ?
                new ObservableCollection<Human>(listHumans.Where(x=> x.IsStaff == 1 && !x.IsDeleted)) :
                new ObservableCollection<Human>();
        }

        #region DataHandler

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<SecurityList>();
            UpdatedCollection = new ObservableCollection<SecurityList>();

            SecurityList = new SecurityList();
            var fullTableList = await ApiConnector.GetAll<SecurityList>("SecurityLists");
            SecurityLists = new ObservableCollection<SecurityList>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<SecurityList>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            SecurityList.IdSecurity = null;
            SecurityLists.Add((SecurityList)SecurityList.Clone());
            AddedCollection.Add((SecurityList)SecurityList.Clone());

            Selected = new SecurityList();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("AnimalTypes", Deleted.IdSecurity.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (Selected?.IdSecurity != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                SecurityLists.Remove(Selected);

                Selected = new SecurityList();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                SecurityLists.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<SecurityList>("SecurityLists", addedHuman);
                    allMessageBuilder.Append($"{addMessage}\n");
                }
                foreach (var updatedHuman in UpdatedCollection.Where(x => x.IdSecurity != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("SecurityLists", updatedHuman, updatedHuman.IdSecurity.Value);
                    allMessageBuilder.Append($"{updateMessage}\n");
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
            foreach (var item in SecurityLists)
                exportList.Add($"{item.IdSecurity}, {item.TimeStart}, {item.TimeEnd}, {item.IdHuman}, " +
                               $"{item.IdCheckpoint}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "SecurityLists");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(6);
            try
            {
                foreach (var items in imported)
                {
                    var item = new SecurityList
                    {
                        IdSecurity = null,
                        TimeStart = Convert.ToDateTime(items[1]),
                        TimeEnd = Convert.ToDateTime(items[2]),
                        IdHuman = Convert.ToInt32(items[3]),
                        IdCheckpoint = Convert.ToInt32(items[4]),
                        IsDeleted = Convert.ToBoolean(items[5])
                    };
                    AddedCollection.Add(item);
                    SecurityLists.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        public string ValidationErrorMessage()
        {
            if (SecurityList == null) return String.Empty;

            if (SecurityList.TimeStart > DateTime.Now) return "Поле \"Дата начала\" не может быть в будущем";
            if (SecurityList.TimeEnd > DateTime.Now) return "Поле \"Дата конца\" не может быть в будущем";
            if (SecurityList.TimeStart.Year < 2010) return "Минимальное значение поля \"Дата начала\" - 2010 год";
            if (SecurityList.TimeEnd.HasValue && SecurityList.TimeEnd.Value.Year < 2010) return "Минимальное значение поля \"Дата конца\" - 2010 год";
            if (!Humans.Select(x => x.IdHuman).Contains(SecurityList.IdHuman)) return "Поле \"Охранник\" не выбрано";
            if (!Checkpoints.Select(x => x.IdCheckpoint).Contains(SecurityList.IdCheckpoint)) return "Поле \"КПП\" не выбрано";

            return String.Empty;
        }

        #endregion
    }
}
