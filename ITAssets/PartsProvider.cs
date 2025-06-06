using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public DatabaseService DBConnection;
        public bool _IsAddMode = false;
        public ObservableCollection<Part> Parts{ get; }
        public ObservableCollection<Category> Categories{ get; }

        public ICommand AddPartCmd { get; }
        public ICommand ModifyPartCmd { get; }
        public ICommand CancelPartCmd { get; }
        public ICommand DeletePartCmd { get; }
        public ICommand SavePartCmd { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PartsViewModel()
        {
            DBConnection = new DatabaseService(App.connectionString);
            Parts = new ObservableCollection<Part>(DBConnection.GetParts());
            Categories = new ObservableCollection<Category>(DBConnection.GetCategories());

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

                    },
                    canExecute: _ => SelectedPart != null && !IsEditMode

                );

            SavePartCmd = new RelayCommand
                (
                    execute: parameter =>
                    {
                        IsEditMode = false;
                        _IsAddMode = false;
                    },
                    canExecute: _ => IsEditMode == true
                );



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

        private Part _selectedPart;

        public Part SelectedPart
        {
            get => _selectedPart;
            set
            {
                if (_selectedPart != value)
                {
                    _selectedPart = value;

                    if (value is null)
                        EditPart = null;
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

                    OnPropertyChanged(nameof(SelectedCategory));

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
                    _buffer1.CategoryName = "";
                    target = _buffer1;
                }
                else
                {
                    target = ReferenceEquals(EditPart, _buffer1) ? _buffer2 : _buffer1;

                    target.ID = value.ID;
                    target.Name = value.Name;
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


