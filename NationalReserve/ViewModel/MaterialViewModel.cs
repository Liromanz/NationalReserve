using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class MaterialViewModel : ObservableObject
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Material> UpdatedCollection { get; set; }
        public ObservableCollection<Material> AddedCollection { get; set; }

        private ObservableCollection<Material> _deletedCollection;
        public ObservableCollection<Material> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Material _deleted;
        public Material Deleted
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

        private ObservableCollection<Material> _materials;
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged();
            }
        }

        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                if (value != null)
                {
                    if (MaterialTypes != null && MaterialTypes.Any())
                        MaterialType = MaterialTypes.FirstOrDefault(x => x.Id == value.IdType);
                    else
                        UpdatedCollection.Add(value);
                }
                _material = value;
                OnPropertyChanged();
            }
        }

        private Material _selected;
        public Material Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Material = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MaterialType> _materialTypes;
        public ObservableCollection<MaterialType> MaterialTypes
        {
            get => _materialTypes;
            set
            {
                _materialTypes = value;
                OnPropertyChanged();
            }
        }

        private MaterialType _materialType;
        public MaterialType MaterialType
        {
            get => _materialType;
            set
            {
                _materialType = value;
                Material.IdType = value?.Id ?? Material.IdType;
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

        public MaterialViewModel()
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

            MaterialTypes = await ApiConnector.GetAll<MaterialType>("MaterialTypes");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Material>();
            UpdatedCollection = new ObservableCollection<Material>();
            DeletedCollection = new ObservableCollection<Material>();

            Material = new Material();
            Materials = await ApiConnector.GetAll<Material>("Materials");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Material.IdMaterial = null;
            Materials.Add(Material);
            AddedCollection.Add(Material);

            Selected = new Material();
        }

        public void LogicalDelete()
        {
            if (Selected?.IdType != null)
            {
                DeletedCollection.Add(Selected);
                Materials.Remove(Selected);

                Selected = new Material();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Materials.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Material>("Materials", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdMaterial != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Materials", updated, updated.IdMaterial.Value);
                    allMessageBuilder.Append($"{updated.Name}: {updateMessage}\n");
                }
                foreach (var deleted in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("Materials", deleted.IdMaterial.Value);
                    allMessageBuilder.Append($"{deleted.Name}: {deleteMessage}\n");
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
            if (Material == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Material.Name)) return "Поле \"Наименование\" незаполнено";
            if (!MaterialTypes.Select(x => x.Id).Contains(Material.IdType)) return "Поле \"Тип материала\" не выбрано";
            if (Material.CostPerOne <= 0) return "Поле \"Цена за единицу\" не может быть отрицательным или равным нулю";

            return String.Empty;
        }

        #endregion
    }
}
