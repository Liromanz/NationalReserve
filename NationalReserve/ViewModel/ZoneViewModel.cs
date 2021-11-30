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
    public class ZoneViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Zone> UpdatedCollection { get; set; }
        public ObservableCollection<Zone> AddedCollection { get; set; }

        private ObservableCollection<Zone> _deletedCollection;
        public ObservableCollection<Zone> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Zone _deleted;
        public Zone Deleted
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
                if (value != null)
                {
                    if (Checkpoints != null && Checkpoints.Any())
                        Checkpoint = Checkpoints.FirstOrDefault(x => x.IdCheckpoint == value.IdCheckpoint);
                    else
                        UpdatedCollection.Add(value);
                }
                _zone = value;
                OnPropertyChanged();
            }
        }

        private Zone _selected;
        public Zone Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Zone = value;
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
                Zone.IdCheckpoint = value?.IdCheckpoint ?? Zone.IdCheckpoint;
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

        public ZoneViewModel()
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
        }

        #region DataHandler

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Zone>();
            UpdatedCollection = new ObservableCollection<Zone>();
            DeletedCollection = new ObservableCollection<Zone>();

            Zone = new Zone();
            Zones = await ApiConnector.GetAll<Zone>("Zones");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Zone.IdZone = null;
            Zones.Add((Zone)Zone.Clone());
            AddedCollection.Add((Zone)Zone.Clone());

            Selected = new Zone();
        }

        public void LogicalDelete()
        {
            if (Selected?.IdZone != null)
            {
                DeletedCollection.Add(Selected);
                Zones.Remove(Selected);

                Selected = new Zone();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Zones.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Zone>("Zones", addedHuman);
                    allMessageBuilder.Append($"{addedHuman.Name}: {addMessage}\n");
                }
                foreach (var updatedHuman in UpdatedCollection.Where(x => x.IdZone != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Zones", updatedHuman, updatedHuman.IdZone.Value);
                    allMessageBuilder.Append($"{updatedHuman.Name}: {updateMessage}\n");
                }
                foreach (var deletedHuman in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("Zones", deletedHuman.IdZone.Value);
                    allMessageBuilder.Append($"{deletedHuman.Name}: {deleteMessage}\n");
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
            if (Zone == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Zone.Name)) return "Поле \"Наименование\" не заполнено";
            if (string.IsNullOrWhiteSpace(Zone.Description)) return "Поле \"Описание\" не заполнено";
            if (Zone.Square <= 0) return "Поле \"Площадь\" не может быть меньше или равно нулю";
            if (Zone.IsForStaff < 0 || Zone.IsForStaff > 1) return "Поле \"Спец. зона\" не выбрано";
            if (Zone.IsForWatch < 0 || Zone.IsForWatch > 1) return "Поле \"Зона для просмотра\" не выбрано";
            if (!Checkpoints.Select(x => x.IdCheckpoint).Contains(Zone.IdCheckpoint)) return "Поле \"КПП\" не выбрано";

            return String.Empty;
        }

        #endregion
    }
}
