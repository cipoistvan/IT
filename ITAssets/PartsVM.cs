using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace ITAssets
{
    public class DesignPartsViewModel
    {
        public List<Part> Parts{ get; set; }

        public DesignPartsViewModel()
        {
            Parts = new List<Part>
            {
                new Part { ID = 1, Name = "SSD 1TB", CategoryName = "SSD"},
                new Part { ID = 2, Name = "Intel i5", CategoryName = "CPU"},
                new Part { ID = 3, Name = "DDR4 RAM 16GB", CategoryName = "RAM" }
            };

        }

    }

    public class PartsViewModel:INotifyPropertyChanged
    {
        private IPartRepository partRepository;
        public bool _IsAddMode = false;
        public ObservableCollection<Part> Parts{ get; }
        public ObservableCollection<Category> Categories{ get; }

        public ICommand AddPartCmd { get; }
        public ICommand ModifyPartCmd { get; }
        public ICommand DeletePartCmd { get; }
        public ICommand SavePartCmd { get; }
        public ICommand CancelPartCmd { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PartsViewModel(IPartRepository partRepository)
        {
            this.partRepository = partRepository;
            Parts = new ObservableCollection<Part>(partRepository.GetParts());
            Categories = new ObservableCollection<Category>(partRepository.GetCategories());

            AddPartCmd = new RelayCommand
               (
                   execute: _ =>
                   {
                       IsEditMode = true;
                       _IsAddMode = true;
                       EditPart = null;
                       SelectedCategory = null;
                   },
                   canExecute: _ => !IsEditMode
               );

            ModifyPartCmd = new RelayCommand
                (
                    execute: _ => IsEditMode = true,
                    canExecute: _ => SelectedPart != null && !IsEditMode
                );

            DeletePartCmd = new RelayCommand
                (
                    execute: _ =>
                    {

                        var delOk = 
                        MessageBox.Show("Biztosan törölni szeretné?","Megerősítés",MessageBoxButton.YesNo,MessageBoxImage.Question);

                        if (delOk == MessageBoxResult.Yes)
                        {

                            var result = partRepository.DeletePart(SelectedPart);

                            switch (result)
                            {
                                case DeleteResult.Success:
                                    SelectedPart = null;
                                    Parts.Clear();
                                    foreach (var part in partRepository.GetParts())
                                    {
                                        Parts.Add(part);
                                    }
                                    break;

                                case DeleteResult.ForeignKeyConstraint:
                                    MessageBox.Show("Nem törölhető az alkatrész, mert más rekord hivatkozik rá.");
                                    break;

                                case DeleteResult.Error:
                                    MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                                    break;

                            }
                        }

                    },
                    canExecute: _ => SelectedPart != null && !IsEditMode

                );

            SavePartCmd = new RelayCommand(ExecuteSave, CanExecuteSave);

            CancelPartCmd = new RelayCommand
                (
                    execute: parameter =>
                    {
                        IsEditMode = false;
                        _IsAddMode = false;
                        EditPart = SelectedPart;

                    },
                    canExecute: _ => IsEditMode == true

                );
        }

        private void ExecuteSave(object parameter)
        {

            UpdateResult result;

            if (_IsAddMode)
            {
                result = partRepository.AddPart(EditPart);
            }
            else
            {
                result = partRepository.ModifyPart(EditPart);
            }

            switch (result)
            {
                case UpdateResult.Success:
                    SelectedPart = null;
                    Parts.Clear();
                    foreach (var part in partRepository.GetParts())
                    {
                        Parts.Add(part);
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



        private Part _selectedPart;

        public Part SelectedPart
        {
            get => _selectedPart;
            set
            {
                if (_selectedPart != value)
                {
                    _selectedPart = value;

                    if (value is null) { 
                        EditPart = null;
                        SelectedCategory = null;
                    }
                    else
                    {

                        EditPart = SelectedPart;
                        SelectedCategory = Categories.FirstOrDefault(c => c.ID == SelectedPart.CategoryId);
                    }

                }
            }
        }


        private Category _selectedCategory;

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;

                    if(value is not null)
                        EditPart.CategoryId = _selectedCategory.ID;
                    OnPropertyChanged(nameof(SelectedCategory));
                    OnPropertyChanged(nameof(EditPart));

                }
            }
        }




        private Part _editPart;
        private Part _buffer1 = new Part();
        private Part _buffer2 = new Part();
        private Part target;

        public Part EditPart
        {
            get => _editPart;
            set
            {

                if (value == null)
                {
                    _buffer1.ID = 0;
                    _buffer1.Name = "";
                    _buffer1.CategoryId = 0;
                    _buffer1.CategoryName = "";
                    target = _buffer1;
                }
                else
                {
                    target = ReferenceEquals(EditPart, _buffer1) ? _buffer2 : _buffer1;

                    target.ID = value.ID;
                    target.Name = value.Name;
                    target.CategoryId = value.CategoryId;
                    target.CategoryName = value.CategoryName;
                }

                _editPart = target;
                OnPropertyChanged(nameof(EditPart));
                
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


