using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAssets
{
    public class DesignASPartsViewModel
    {
        public List<ASPart> ASParts { get; set; }


        public DesignASPartsViewModel()
        {
            ASParts = new List<ASPart>
            {
                new ASPart { ID = 1, AssemblyName="PC1", PartName = "SSD 1TB", CategoryName = "SSD", Quantity = 1 },
                new ASPart { ID = 2, AssemblyName="PC1", PartName = "Intel i5", CategoryName = "CPU", Quantity = 1},
                new ASPart { ID = 3, AssemblyName="PC1", PartName = "DDR4 RAM 16GB", CategoryName = "RAM", Quantity = 1 }
            };

        }

    }

    public class ASPartsViewModel:INotifyPropertyChanged
    {
        public DatabaseService DBConnection;

        private ObservableCollection<ASPart> _asparts;
        public ObservableCollection<ASPart> ASParts 
        
        { get {return _asparts; } 
          set {
                if (mainviewmodel.ITAssemblyVM.SelectedITAssembly == null)
                    _asparts = null;
                else
                    _asparts = value;
                OnPropertyChanged(nameof(ASParts));
            }
        }

        public ICommand AddASPartCmd { get; }
        public ICommand DeleteASPartCmd { get; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private MainViewModel mainviewmodel;
        public ASPartsViewModel(MainViewModel mainViewModel)
        {

            AddASPartCmd = new RelayCommand
            (
                execute: _ =>
                {
                    var addOk =
                        MessageBox.Show("Biztosan hozzá akarja adni?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    
                    if (addOk == MessageBoxResult.Yes)
                    {
                        ASPart addpart = new ASPart();
                        addpart.AssemblyID = mainviewmodel.ITAssemblyVM.SelectedITAssembly.ID;
                        addpart.PartID = mainviewmodel.PartVM.SelectedPart.ID;
                        addpart.Quantity = 1;
                        var result = DBConnection.AddASPart(addpart);

                        switch (result)
                        {
                            case UpdateResult.Success:
                                
                                ASParts.Clear();
                                foreach (var aspart in DBConnection.GetASParts(mainviewmodel.ITAssemblyVM.SelectedITAssembly.ID))
                                {
                                    ASParts.Add(aspart);
                                }

                                break;

                            case UpdateResult.Duplicate:
                                MessageBox.Show("Ilyen alkatrész már létezik !");
                                break;

                            case UpdateResult.Error:
                                MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                                break;
                        }



                    }

                },

                canExecute: _ => mainviewmodel.ITAssemblyVM.SelectedITAssembly!=null && mainviewmodel.PartVM.SelectedPart != null && !mainviewmodel.ITAssemblyVM.IsEditMode

            );



            DeleteASPartCmd = new RelayCommand
            (
                execute: _ =>
                {
                    var delOk =
                        MessageBox.Show("Biztosan törölni szeretné?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (delOk == MessageBoxResult.Yes)
                    {

                        var result = DBConnection.DeleteASPart(SelectedASPart);

                        switch (result)
                        {
                            case DeleteResult.Success:
                                SelectedASPart = null;
                                ASParts.Clear();
                                foreach (var aspart in DBConnection.GetASParts(mainviewmodel.ITAssemblyVM.SelectedITAssembly.ID))
                                {
                                    ASParts.Add(aspart);
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

                canExecute: _ => SelectedASPart != null && !mainviewmodel.ITAssemblyVM.IsEditMode

            );

            mainviewmodel = mainViewModel;
            DBConnection = new DatabaseService(App.connectionString);
            ASParts = new ObservableCollection<ASPart>(DBConnection.GetASParts(null));
        }

        private ASPart _selectedASPart;

        public ASPart SelectedASPart
        {
            get => _selectedASPart;
            set
            {
                if (_selectedASPart != value)
                {
                    _selectedASPart = value;
                    App.logger.LogInformation("Új tétel kiválasztva !");
                }
            }
        }


    }

}


