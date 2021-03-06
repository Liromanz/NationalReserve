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
    public class StaffDocumentViewModel : ObservableObject, IOneToOneHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

        #endregion

        #region Данные с верстки

        private ObservableCollection<StaffDocument> _staffDocuments;
        public ObservableCollection<StaffDocument> StaffDocuments
        {
            get => _staffDocuments;
            set
            {
                _staffDocuments = value;
                OnPropertyChanged();
            }
        }

        private StaffDocument _staffDocument;
        public StaffDocument StaffDocument
        {
            get => _staffDocument;
            set
            {
                _staffDocument = value;
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
                ReadOneAsync(value?.IdHuman ?? 0);
                _human = value;
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

        public StaffDocumentViewModel()
        {
            IsBusy = true;
            InitAsync();
        }

        private async void InitAsync()
        {
            AddCommand = new RelayCommand(o => { CreateAsync(); });
            EditCommand = new RelayCommand(o => { UpdateAsync(); });
            DeleteCommand = new RelayCommand(o => { DeleteAsync(); });
            ExportCommand = new RelayCommand(o => { ExportTable(); });
            ImportCommand = new RelayCommand(o => { ImportTable(); });

            Humans = await ApiConnector.GetAll<Human>("Humen");
            IsBusy = false;
        }

        #region CRUDw

        public async void ReadOneAsync(int id)
        {
            IsBusy = true;
            StaffDocument = await ApiConnector.GetOne<StaffDocument>("StaffDocuments", id);
            if (StaffDocument == null)
                StaffDocument = new StaffDocument();
            else
                StaffDocument.Id = id;
            IsBusy = false;
        }

        public async void CreateAsync()
        {
            try
            {
                IsBusy = true;
                StaffDocument.Id = Human.IdHuman ?? StaffDocument.Id;
                StaffDocuments.Add(StaffDocument);
                if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message);
                    return;
                }

                var addMessage = await ApiConnector.AddData<StaffDocument>("StaffDocuments", StaffDocument);
                MessageBox.Show($"Сотрудник №{StaffDocument.Id}: {addMessage}");
                ReadOneAsync(StaffDocument.Id);
            }
            catch (Exception e)
            {
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void UpdateAsync()
        {
            try
            {
                IsBusy = true;
                if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message);
                    return;
                }
                var updateMessage = await ApiConnector.UpdateData("StaffDocuments", StaffDocument, StaffDocument.Id);
                MessageBox.Show($"Сотрудник №{StaffDocument.Id}: {updateMessage}");
                ReadOneAsync(StaffDocument.Id);
            }
            catch (Exception e)
            {
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void DeleteAsync()
        {

            MessageBoxResult dialogResult = MessageBox.Show(GlobalConstants.DeleteQuestionMessage, "Удаление", MessageBoxButton.YesNo);
            if (dialogResult != MessageBoxResult.Yes) return;

            try
            {
                IsBusy = true;
                if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message);
                    return;
                }

                var deleteMessage = await ApiConnector.DeleteData("StaffDocuments", StaffDocument.Id);
                MessageBox.Show($"Сотрудник №{StaffDocument.Id}: {deleteMessage}");
                ReadOneAsync(StaffDocument.Id);
            }
            catch (Exception e)
            {
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        public void ExportTable()
        {
            List<string> exportList = new List<string>();
            foreach (var item in StaffDocuments)
                exportList.Add($"{item.Id}, {item.SerialPass}, {item.NumberPass}, {item.BankNumber}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "StaffDocuments");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(5).FirstOrDefault();
            try
            {
                var item = new StaffDocument
                {
                    Id = Convert.ToInt32(imported[1]),
                    SerialPass = Convert.ToInt32(imported[1]),
                    NumberPass = Convert.ToInt32(imported[2]),
                    BankNumber = Convert.ToInt32(imported[3]),
                    IsDeleted = Convert.ToBoolean(imported[4])
                };
                StaffDocument = item;
                CreateAsync();
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        public string ValidationErrorMessage()
        {
            if (StaffDocument == null) return String.Empty;

            if (StaffDocuments.Count > 1)
            {
                StaffDocuments.RemoveAt(1);
                return "У сотрудника уже есть документы, добавить новую запись нельзя";
            }
            if (!Humans.Select(x => x.IdHuman).Contains(StaffDocument.Id)) return "Поле \"Сотрудник\" не выбрано";
            if (StaffDocument.SerialPass.ToString().Length != 4) return "Поле \"Серия паспорта\" не заполнено. В поле должно быть 4 числа";
            if (StaffDocument.NumberPass.ToString().Length != 6) return "Поле \"Номер паспорта\" не заполнено. В поле должно быть 6 чисел";
            if (StaffDocument.BankNumber.ToString().Length != 16) return "Поле \"Банковкий счет\" не заполнен. В поле должно быть 16 чисел";

            return String.Empty;
        }

        #endregion
    }
}
