using ITAssets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAssets
{
    public class DesignITAssembyViewModel
    {
        public List<ITAssembly> ITAssemblies { get; set; }

        public DesignITAssembyViewModel()
        {
            ITAssemblies = new List<ITAssembly>
            {
                new ITAssembly { ID = 1, Name = "PC1", UserName = "admin", Date = DateTime.Today},
                new ITAssembly { ID = 2, Name = "PC2", UserName = "admin", Date = DateTime.Today},
                new ITAssembly { ID = 3, Name = "PC3", UserName = "admin", Date = DateTime.Today}
            };

        }
    }



    public class ITAssemblyViewModel:INotifyPropertyChanged
    {
        private IITAssemblyRepository ITAssemblyRepository;
        private IASPartRepository ASPartRepository;
        public bool _IsAddMode = false;
        public ObservableCollection<ITAssembly> ITAssemblies { get; }

        public ICommand AddITAssemblyCmd { get; }
        public ICommand ModifyITAssemblyCmd { get; }
        public ICommand DeleteITAssemblyCmd { get; }
        public ICommand SaveITAssemblyCmd { get; }
        public ICommand CancelITAssemblyCmd { get; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private MainViewModel mainviewmodel;
        public ITAssemblyViewModel(MainViewModel mainViewModel, IITAssemblyRepository ITAssemblyRepository, IASPartRepository ASPartRepository)
        {
            mainviewmodel = mainViewModel;

            this.ITAssemblyRepository = ITAssemblyRepository;
            this.ASPartRepository = ASPartRepository;
            ITAssemblies = new ObservableCollection<ITAssembly>(ITAssemblyRepository.GetITAssemblies());

            AddITAssemblyCmd = new RelayCommand
               (
                   execute: _ =>
                   {
                       IsEditMode = true;
                       _IsAddMode = true;
                       EditITAssembly = null;
                   },
                   canExecute: _ => !IsEditMode
               );

            ModifyITAssemblyCmd = new RelayCommand
                (
                    execute: _ => IsEditMode = true,
                    canExecute: _ => SelectedITAssembly != null && !IsEditMode
                );

            DeleteITAssemblyCmd = new RelayCommand
                (
                    execute: _ =>
                    {

                        var delOk =
                        MessageBox.Show("Biztosan törölni szeretné?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (delOk == MessageBoxResult.Yes)
                        {

                            var result = ITAssemblyRepository.DeleteITAssembly(SelectedITAssembly);

                            switch (result)
                            {
                                case DeleteResult.Success:
                                    SelectedITAssembly = null;
                                    ITAssemblies.Clear();
                                    foreach (var ITAssembly in ITAssemblyRepository.GetITAssemblies())
                                    {
                                        ITAssemblies.Add(ITAssembly);
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
                    canExecute: _ => SelectedITAssembly != null && !IsEditMode

                );

            SaveITAssemblyCmd = new RelayCommand(ExecuteSave, CanExecuteSave);

            CancelITAssemblyCmd = new RelayCommand
                (
                    execute: parameter =>
                    {
                        IsEditMode = false;
                        _IsAddMode = false;
                        EditITAssembly = SelectedITAssembly;

                    },
                    canExecute: _ => IsEditMode == true

                );

        }

        private void ExecuteSave(object parameter)
        {

            UpdateResult result;

            EditITAssembly.UserId = mainviewmodel.LoginVM.LoginUser.ID;

            if (_IsAddMode)
            {
                result = ITAssemblyRepository.AddITAssembly(EditITAssembly);
            }
            else
            {
                result = ITAssemblyRepository.ModifyITAssembly(EditITAssembly);
            }
            
            switch (result)
            {
                case UpdateResult.Success:
                    SelectedITAssembly = null;
                    ITAssemblies.Clear();
                    foreach (var ITAssembly in ITAssemblyRepository.GetITAssemblies())
                    {
                        ITAssemblies.Add(ITAssembly);
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



        private ITAssembly _selectedITAssembly;

        public ITAssembly SelectedITAssembly
        {
            get => _selectedITAssembly;
            set
            {
                if (_selectedITAssembly != value)
                {
                    _selectedITAssembly = value;

                    if (value is null)
                    {
                        EditITAssembly = null;
                        mainviewmodel.ASPartVM.ASParts = null;
                    }
                    else
                    {

                        EditITAssembly = SelectedITAssembly;
                        mainviewmodel.ASPartVM.ASParts = new ObservableCollection<ASPart>(ASPartRepository.GetASParts(SelectedITAssembly.ID));
                        
                    }

                }
            }
        }




        private ITAssembly _editITAssembly;
        private ITAssembly _buffer1 = new ITAssembly();
        private ITAssembly _buffer2 = new ITAssembly();
        private ITAssembly target;

        public ITAssembly EditITAssembly
        {
            get => _editITAssembly;
            set
            {

                if (value == null)
                {
                    _buffer1.ID = 0;
                    _buffer1.UserId = 0;
                    _buffer1.UserName = "";
                    _buffer1.Name = "";
                    _buffer1.Date = null;
                    target = _buffer1;
                }
                else
                {
                    target = ReferenceEquals(EditITAssembly, _buffer1) ? _buffer2 : _buffer1;

                    target.ID = value.ID;
                    target.UserId = value.UserId;
                    target.UserName = value.UserName;
                    target.Name = value.Name;
                    target.Date = value.Date;
                }

                _editITAssembly = target;
                OnPropertyChanged(nameof(EditITAssembly));

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



