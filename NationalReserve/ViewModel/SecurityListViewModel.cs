using System;
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
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

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
                SecurityList.IdCheckpoint = value?.IdCheckpoint ?? 1;
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
                SecurityList.IdHuman = value?.IdHuman ?? 1;
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
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });

            Checkpoints = await ApiConnector.GetAll<Checkpoint>("Checkpoints");
            var listHumans = await ApiConnector.GetAll<Human>("Humen");
            Humans = new ObservableCollection<Human>(listHumans.Where(x=> x.IsStaff == 1));
        }

        #region DataHandler

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<SecurityList>();
            UpdatedCollection = new ObservableCollection<SecurityList>();
            DeletedCollection = new ObservableCollection<SecurityList>();

            SecurityList = new SecurityList();
            SecurityLists = await ApiConnector.GetAll<SecurityList>("SecurityLists");

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
            SecurityLists.Add(SecurityList);
            AddedCollection.Add(SecurityList);

            Selected = new SecurityList();
        }

        public void LogicalDelete()
        {
            if (Selected?.IdSecurity != null)
            {
                DeletedCollection.Add(Selected);
                SecurityLists.Remove(Selected);

                Selected = new SecurityList();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
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
                foreach (var deletedHuman in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("SecurityLists", deletedHuman.IdSecurity.Value);
                    allMessageBuilder.Append($"{deleteMessage}\n");
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

        public string ValidationErrorMessage()
        {
            if (SecurityList == null) return String.Empty;

            if (SecurityList.TimeStart > DateTime.Now) return "Поле \"Дата начала\" не может быть в будущем";
            if (SecurityList.TimeEnd > DateTime.Now) return "Поле \"Дата конца\" не может быть в будущем";
            if (!Humans.Select(x => x.IdHuman).Contains(SecurityList.IdHuman)) return "Поле \"Охранник\" не выбрано";
            if (!Checkpoints.Select(x => x.IdCheckpoint).Contains(SecurityList.IdSecurity)) return "Поле \"КПП\" не выбрано";

            return String.Empty;
        }

        #endregion
    }
}
