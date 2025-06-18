using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAssets
{
    public class DesignPurchasesViewModel
    {
        public List<Purchase> Purchases { get; set; }

        public DesignPurchasesViewModel()
        {
            Purchases = new List<Purchase>
            {
                new Purchase { ID = 1, Date = DateTime.Today, UserName = "admin", PartName = "SSD 1TB", CategoryName = "SSD", Quantity = 2, UnitPrice = 25000 },
                new Purchase { ID = 2, Date = DateTime.Today, UserName = "admin", PartName = "Intel i5", CategoryName = "CPU", Quantity = 1, UnitPrice = 60000 },
                new Purchase { ID = 3, Date = DateTime.Today, UserName = "admin", PartName = "DDR4 RAM 16GB", CategoryName = "RAM", Quantity = 4, UnitPrice = 15000 }
            };

        }

    }

    public class PurchasesViewModel:INotifyPropertyChanged
    {
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IPartRepository partRepository;
        
        public bool _IsAddMode = false;
        public PurchasesFilter Filter { get; set; } = new PurchasesFilter();
        
        public ObservableCollection<Purchase> Purchases { get; set; }
        public List<Purchase> AllPurchases { get; set; }
        public ObservableCollection<Part> Parts { get; }
        public ObservableCollection<Category> Categories { get; }

        public ICommand AddPurchaseCmd { get; }
        public ICommand ModifyPurchaseCmd { get; }
        public ICommand DeletePurchaseCmd { get; }
        
        public ICommand SavePurchaseCmd { get; }
        public ICommand CancelPurchaseCmd { get; }

        public ICommand ApplyFilterCmd { get; }
        public ICommand ClearFilterCmd { get; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private MainViewModel mainviewmodel;
        public PurchasesViewModel(MainViewModel mainViewModel, IPurchaseRepository purchaseRepository, IPartRepository partRepository)
        {
            mainviewmodel = mainViewModel;
            this.purchaseRepository = purchaseRepository;
            this.partRepository = partRepository;

            Purchases = new ObservableCollection<Purchase>(purchaseRepository.GetPurchases());
            AllPurchases = Purchases.ToList();
            Parts = new ObservableCollection<Part>(partRepository.GetParts());
            Categories = new ObservableCollection<Category>(partRepository.GetCategories());

            AddPurchaseCmd = new RelayCommand
               (
                   execute: _ =>
                   {
                       IsEditMode = true;
                       _IsAddMode = true;
                       EditPurchase = null;
                       SelectedPart = null;
                   },
                   canExecute: _ => !IsEditMode
               );

            ModifyPurchaseCmd = new RelayCommand
                (
                    execute: _ => IsEditMode = true,
                    canExecute: _ => SelectedPurchase != null && !IsEditMode
                );

            DeletePurchaseCmd = new RelayCommand
                (
                   execute: _ =>
                   {

                       var delOk =
                       MessageBox.Show("Biztosan törölni szeretné?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

                       if (delOk == MessageBoxResult.Yes)
                       {

                           var result = purchaseRepository.DeletePurchase(SelectedPurchase);

                           switch (result)
                           {
                               case DeleteResult.Success:
                                   SelectedPurchase = null;
                                   Purchases.Clear();
                                   AllPurchases = purchaseRepository.GetPurchases();

                                   foreach (var purchase in AllPurchases)
                                   {
                                       Purchases.Add(purchase);
                                   }
                                   break;

                               case DeleteResult.ForeignKeyConstraint:
                                   MessageBox.Show("Nem törölhető a vásárlás, mert más rekord hivatkozik rá.");
                                   break;

                               case DeleteResult.Error:
                                   MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                                   break;

                           }
                       }

                   },
                    canExecute: _ => SelectedPurchase != null && !IsEditMode

                );

            SavePurchaseCmd = new RelayCommand(ExecuteSave, CanExecuteSave);

            CancelPurchaseCmd = new RelayCommand
                (
                    execute: parameter =>
                    {
                        IsEditMode = false;
                        _IsAddMode = false;
                        EditPurchase = SelectedPurchase;

                    },
                    canExecute: _ => IsEditMode == true

                );

            ApplyFilterCmd = new RelayCommand(ApplyFilter);

            ClearFilterCmd = new RelayCommand(
                execute: _=>
                {
                    Filter.FromDate = null;
                    Filter.ToDate = null;
                    Filter.PartName = string.Empty;
                    Filter.PartType = string.Empty;

                    AllPurchases = purchaseRepository.GetPurchases();

                    Purchases = new ObservableCollection<Purchase>(AllPurchases.ToList());
                    
                    OnPropertyChanged(nameof(Purchases));

                });
        }

        private void ExecuteSave(object parameter)
        {
            UpdateResult result;


            EditPurchase.UserId = mainviewmodel.LoginVM.LoginUser.ID;
            if (_IsAddMode)
            {
                result = purchaseRepository.AddPurchase(EditPurchase);
            }
            else
            {
                result = purchaseRepository.ModifyPurchase(EditPurchase);
            }

            switch (result)
            {
                case UpdateResult.Success:
                    SelectedPurchase = null;
                    Purchases.Clear();
                    AllPurchases = purchaseRepository.GetPurchases();
                    foreach (var purchase in AllPurchases)
                    {
                        Purchases.Add(purchase);
                    }

                    _IsAddMode = false;
                    IsEditMode = false;

                    break;

                case UpdateResult.Duplicate:
                    MessageBox.Show("Ilyen alkatrész már létezik !");
                    break;

                case UpdateResult.Error:
                    MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                    break;
            }


        }
        private bool CanExecuteSave(object parameter) => IsEditMode;

        private void ApplyFilter(object parameter)
        {
            var strategies = new List<IFilterStrategy<Purchase>>();

            if (!string.IsNullOrWhiteSpace(Filter.PartName))
                strategies.Add(new PartNameFilter(Filter.PartName));

            if (!string.IsNullOrWhiteSpace(Filter.PartType))
                strategies.Add(new PartTypeFilter(Filter.PartType));

            if (Filter.FromDate.HasValue || Filter.ToDate.HasValue)
                strategies.Add(new DateRangeFilter(Filter.FromDate, Filter.ToDate));

            var combined = new AndFilter<Purchase>(strategies);
            var result = AllPurchases.Where(p => combined.Matches(p)).ToList();

            Purchases = new ObservableCollection<Purchase>(result);
            OnPropertyChanged(nameof(Purchases));

        }

        private Purchase _selectedPurchase;

        public Purchase SelectedPurchase
        {
            get => _selectedPurchase;
            set
            {
                if (_selectedPurchase != value)
                {
                    _selectedPurchase = value;

                    if (value is null) { 
                        EditPurchase = null;
                        SelectedPart = null;
                    }
                    else
                    {

                        EditPurchase = SelectedPurchase;
                        SelectedPart = Parts.FirstOrDefault(c => c.ID == SelectedPurchase.PartId);
                    }

                }
            }
        }

        private Part _selectedPart;

        public Part SelectedPart
        {
            get => _selectedPart;
            set
            {
                if (_selectedPart != value)
                {
                    _selectedPart = value;

                    if (value is not null)
                        EditPurchase.PartId = _selectedPart.ID;
                    OnPropertyChanged(nameof(SelectedPart));
                    OnPropertyChanged(nameof(EditPurchase));

                }
            }
        }

        private Purchase _editPurchase;
        private Purchase _buffer1 = new Purchase();
        private Purchase _buffer2 = new Purchase();
        private Purchase target;

        public Purchase EditPurchase
        {
            get => _editPurchase;
            set
            {

                if (value == null)
                {
                    _buffer1.ID = 0;
                    _buffer1.UserId = 0;
                    _buffer1.UserName = "";
                    _buffer1.PartId = 0;
                    _buffer1.PartName = "";
                    _buffer1.CategoryId = 0;
                    _buffer1.CategoryName = "";
                    _buffer1.Quantity = 0;
                    _buffer1.UnitPrice = 0;
                    _buffer1.Date = null;
                    target = _buffer1;
                }
                else
                {
                    target = ReferenceEquals(EditPurchase, _buffer1) ? _buffer2 : _buffer1;

                    target.ID = value.ID;
                    target.UserId = value.UserId;
                    target.UserName = value.UserName;
                    target.PartId = value.PartId;
                    target.PartName = value.PartName;
                    target.CategoryId = value.CategoryId;
                    target.CategoryName = value.CategoryName;
                    target.Quantity = value.Quantity;
                    target.UnitPrice = value.UnitPrice;
                    target.Date = value.Date;

                }

                _editPurchase = target;
                OnPropertyChanged(nameof(EditPurchase));

            }
        }



        private bool _IsEditMode = false;
        public bool IsEditMode
        {
            get => _IsEditMode;
            set
            {
                if (_IsEditMode != value)
                {
                    _IsEditMode = value;
                    OnPropertyChanged(nameof(IsEditMode));
                    OnPropertyChanged(nameof(IsGridEnabled));
                }
            }
        }

        public bool IsGridEnabled => !IsEditMode;
    }

}

