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
using NationalReserve.View;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class SponsorshipViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<Sponsorship> UpdatedCollection { get; set; }
        public ObservableCollection<Sponsorship> AddedCollection { get; set; }

        private ObservableCollection<Sponsorship> _deletedCollection;
        public ObservableCollection<Sponsorship> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Sponsorship _deleted;
        public Sponsorship Deleted
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

        private ObservableCollection<Sponsorship> _sponsorships;
        public ObservableCollection<Sponsorship> Sponsorships
        {
            get => _sponsorships;
            set
            {
                _sponsorships = value;
                OnPropertyChanged();
            }
        }

        private Sponsorship _sponsorship;
        public Sponsorship Sponsorship
        {
            get => _sponsorship;
            set
            {
                if (value != null)
                {
                    if (PaymentTypes != null && PaymentTypes.Any())
                        PaymentType = PaymentTypes.FirstOrDefault(x => x.Id == value.IdHuman);
                    if (Humans != null && Humans.Any())
                        Human = Humans.FirstOrDefault(x => x.IdHuman == value.IdHuman);
                    UpdatedCollection.Add(value);
                }
                _sponsorship = value;
                OnPropertyChanged();
            }
        }

        private Sponsorship _selected;
        public Sponsorship Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Sponsorship = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PaymentType> _paymentTypes;
        public ObservableCollection<PaymentType> PaymentTypes
        {
            get => _paymentTypes;
            set
            {
                _paymentTypes = value;
                OnPropertyChanged();
            }
        }

        private PaymentType _paymentType;
        public PaymentType PaymentType
        {
            get => _paymentType;
            set
            {
                _paymentType = value;
                Sponsorship.IdType = value?.Id ?? Sponsorship.IdType;
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
                Sponsorship.IdHuman = value?.IdHuman ?? Sponsorship.IdHuman;
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

        public SponsorshipViewModel()
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
            PaymentTypes = await ApiConnector.GetAll<PaymentType>("PaymentTypes");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Sponsorship>();
            UpdatedCollection = new ObservableCollection<Sponsorship>();
            DeletedCollection = new ObservableCollection<Sponsorship>();

            Sponsorship = new Sponsorship();
            var fullTableList = await ApiConnector.GetAll<Sponsorship>("Sponsorships");
            Sponsorships = new ObservableCollection<Sponsorship>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<Sponsorship>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Sponsorship.IdPayment = null;
            Sponsorship.PaymentDate = DateTime.Now;
            Sponsorships.Add((Sponsorship)Sponsorship.Clone());
            AddedCollection.Add((Sponsorship)Sponsorship.Clone());

            Selected = new Sponsorship();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("Sponsorships", Deleted.IdPayment.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (Selected?.IdPayment != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                Sponsorships.Remove(Selected);

                Selected = new Sponsorship();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                Sponsorships.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Sponsorship>("Sponsorships", added);
                    allMessageBuilder.Append($"Сумма:{added.Amount}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdPayment != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Sponsorships", updated, updated.IdPayment.Value);
                    allMessageBuilder.Append($"Сумма:{updated.Amount}: {updateMessage}\n");
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
            foreach (var item in Sponsorships)
                exportList.Add($"{item.IdPayment}, {item.IdHuman}, {item.IdType}, {item.Amount}, " +
                               $"{item.PaymentDate}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "Sponsorships");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(6);
            try
            {
                foreach (var items in imported)
                {
                    var item = new Sponsorship
                    {
                        IdPayment = null,
                        IdHuman = Convert.ToInt32(items[1]),
                        IdType = Convert.ToInt32(items[2]),
                        Amount = Convert.ToInt32(items[3]),
                        PaymentDate = Convert.ToDateTime(items[4]),
                        IsDeleted = Convert.ToBoolean(items[5])
                    };
                    AddedCollection.Add(item);
                    Sponsorships.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        private async void ShowDiagramm()
        {
            DiagrammWindow diagramm = new DiagrammWindow();
            diagramm.ViewModel.DiagrammName = "Деньги спонсоров";
            diagramm.ViewModel.AxisXName = "Номер спонсора";
            diagramm.ViewModel.AxisYName = "Деньги";

            var fullTableList = await ApiConnector.GetAll<Sponsorship>("Sponsorships");
            diagramm.ViewModel.MakeDiagramm(
                fullTableList.Select(x => x.IdHuman).ToHashSet().OrderBy(x => x),
                fullTableList.GroupBy(y => y.IdHuman).Select(y => y.Sum(z => (int)z.Amount)));

            diagramm.Show();
        }
        public string ValidationErrorMessage()
        {
            if (Sponsorship == null) return String.Empty;

            if (!Humans.Select(x => x.IdHuman).Contains(Sponsorship.IdHuman)) return "Поле \"Спонсор\" не выбрано";
            if (!PaymentTypes.Select(x => x.Id).Contains(Sponsorship.IdType)) return "Поле \"Тип оплаты\" не выбрано";
            if (Sponsorship.Amount <= 0) return "Поле \"Сумма\" не может быть отрицательным или равным нулю";

            return String.Empty;
        }

        #endregion
    }
}
