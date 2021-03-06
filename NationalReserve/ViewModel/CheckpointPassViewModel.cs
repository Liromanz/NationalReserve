using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class CheckpointPassViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand DiagrammCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<CheckpointPass> UpdatedCollection { get; set; }
        public ObservableCollection<CheckpointPass> AddedCollection { get; set; }

        private ObservableCollection<CheckpointPass> _deletedCollection;
        public ObservableCollection<CheckpointPass> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private CheckpointPass _deleted;
        public CheckpointPass Deleted
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

        private ObservableCollection<CheckpointPass> _checkpointPasses;
        public ObservableCollection<CheckpointPass> CheckpointPasses
        {
            get => _checkpointPasses;
            set
            {
                _checkpointPasses = value;
                OnPropertyChanged();
            }
        }

        private CheckpointPass _checkpointPass;
        public CheckpointPass CheckpointPass
        {
            get => _checkpointPass;
            set
            {
                if (value != null)
                {
                    if (Humans != null && Humans.Any())
                        Human = Humans.FirstOrDefault(x => x.IdHuman == value.IdHuman);
                    if (Checkpoints != null && Checkpoints.Any())
                        Checkpoint = Checkpoints.FirstOrDefault(x => x.IdCheckpoint == value.IdCheckpoint);
                    UpdatedCollection.Add(value);
                }
                _checkpointPass = value;
                OnPropertyChanged();
            }
        }

        private CheckpointPass _selected;
        public CheckpointPass Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    CheckpointPass = value;
                }
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
                CheckpointPass.IdHuman = value?.IdHuman ?? CheckpointPass.IdHuman;
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
                CheckpointPass.IdCheckpoint = value?.IdCheckpoint ?? CheckpointPass.IdCheckpoint;
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

        public CheckpointPassViewModel()
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
            DiagrammCommand = new RelayCommand(o => { ShowDiagramm(); });

            Humans = await ApiConnector.GetAll<Human>("Humen");
            Checkpoints = await ApiConnector.GetAll<Checkpoint>("Checkpoints");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<CheckpointPass>();
            UpdatedCollection = new ObservableCollection<CheckpointPass>();
            DeletedCollection = new ObservableCollection<CheckpointPass>();

            CheckpointPass = new CheckpointPass();
            var fullTableList = await ApiConnector.GetAll<CheckpointPass>("CheckpointPasses");
            CheckpointPasses = new ObservableCollection<CheckpointPass>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<CheckpointPass>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            CheckpointPass.IdCheckpointPass = null;
            CheckpointPasses.Add((CheckpointPass)CheckpointPass.Clone());
            AddedCollection.Add((CheckpointPass)CheckpointPass.Clone());

            Selected = new CheckpointPass();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("CheckpointPasses", Deleted.IdCheckpointPass.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }

        public void LogicalDelete()
        {
            if (Selected?.IdCheckpointPass != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                CheckpointPasses.Remove(Selected);

                CheckpointPass = new CheckpointPass();
            }
        }
        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                CheckpointPasses.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<CheckpointPass>("CheckpointPasses", added);
                    allMessageBuilder.Append($"{addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdCheckpointPass != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("CheckpointPasses", updated, updated.IdCheckpointPass.Value);
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
            foreach (var item in CheckpointPasses)
                exportList.Add($"{item.IdCheckpointPass}, {item.IdHuman}, {item.IdCheckpoint}, {item.PassType}, {item.PassTime}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "CheckpointPasses");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(6);
            try
            {
                foreach (var items in imported)
                {
                    var item = new CheckpointPass
                    {
                        IdCheckpointPass = null,
                        IdHuman = Convert.ToInt32(items[1]),
                        IdCheckpoint = Convert.ToInt32(items[2]),
                        PassType = items[3],
                        PassTime = Convert.ToDateTime(items[4]),
                        IsDeleted = Convert.ToBoolean(items[5]),
                    };
                    AddedCollection.Add(item);
                    CheckpointPasses.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }

        private async void ShowDiagramm()
        {
            DiagrammWindow diagramm = new DiagrammWindow();
            diagramm.ViewModel.DiagrammName = "Количество проходов по дням в месяце";
            diagramm.ViewModel.AxisXName = "Дни";
            diagramm.ViewModel.AxisYName = "Количество";

            var fullTableList = await ApiConnector.GetAll<CheckpointPass>("CheckpointPasses");
            diagramm.ViewModel.MakeDiagramm(
                fullTableList.Select(x => x.PassTime.Day).ToHashSet().OrderBy(x => x),
                fullTableList.Select(x => x.PassTime.Day));

            diagramm.Show();
        }

        public string ValidationErrorMessage()
        {
            if (CheckpointPass == null) return String.Empty;

            if (!Humans.Select(x => x.IdHuman).Contains(CheckpointPass.IdHuman)) return "Поле \"Человек\" не выбрано";
            if (!Checkpoints.Select(x => x.IdCheckpoint).Contains(CheckpointPass.IdCheckpoint)) return "Поле \"КПП\" не выбрано";
            if (CheckpointPass.PassTime > DateTime.Now) return "Поле \"Время прохода\" не может быть в будущем";
            if (CheckpointPass.PassTime.Year < 2010) return "Минимальное значение поля \"Время прохода\" - 2010 год";

            return String.Empty;
        }

        #endregion
    }
}
