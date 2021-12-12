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
    public class CheckpointViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Checkpoint> UpdatedCollection { get; set; }
        public ObservableCollection<Checkpoint> AddedCollection { get; set; }

        private ObservableCollection<Checkpoint> _deletedCollection;
        public ObservableCollection<Checkpoint> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Checkpoint _deleted;
        public Checkpoint Deleted
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
                if (value != null)
                    UpdatedCollection.Add(value);

                _checkpoint = value;
                OnPropertyChanged();
            }
        }

        private Checkpoint _selected;
        public Checkpoint Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Checkpoint = value;
                }
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

        public CheckpointViewModel()
        {
            IsBusy = true;
            InitAsync();
            ReadAsync();
        }

        private void InitAsync()
        {
            AddCommand = new RelayCommand(o => { AddObject(); });
            SaveCommand = new RelayCommand(o => { SaveAsync(); });
            PhysicalDeleteCommand = new RelayCommand(o => { PhysicalDelete(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Checkpoint>();
            UpdatedCollection = new ObservableCollection<Checkpoint>();
            DeletedCollection = new ObservableCollection<Checkpoint>();

            Checkpoint = new Checkpoint();

            var fullTableList = await ApiConnector.GetAll<Checkpoint>("Checkpoints");
            Checkpoints = new ObservableCollection<Checkpoint>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<Checkpoint>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Checkpoint.IdCheckpoint = null;
            Checkpoints.Add((Checkpoint)Checkpoint.Clone());
            AddedCollection.Add((Checkpoint)Checkpoint.Clone());

            Selected = new Checkpoint();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("Checkpoints", Deleted.IdCheckpoint.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (Selected?.IdCheckpoint != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                Checkpoints.Remove(Selected);

                Selected = new Checkpoint();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                Checkpoints.Add(Deleted);
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
                foreach (var added in AddedCollection)
                {
                    var addMessage = await ApiConnector.AddData<Checkpoint>("Checkpoints", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdCheckpoint != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Checkpoints", updated, updated.IdCheckpoint.Value);
                    allMessageBuilder.Append($"{updated.Name}: {updateMessage}\n");
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
            if (Checkpoint == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(Checkpoint.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
