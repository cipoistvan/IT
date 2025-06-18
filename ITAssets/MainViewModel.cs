using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public PurchasesViewModel PurchaseVM { get; set; }
        public PartsViewModel PartVM { get; set; }
        public ITAssemblyViewModel ITAssemblyVM { get; set; }
        public ASPartsViewModel ASPartVM { get; set; }
        public UsersViewModel UserVM { get; set; }
        public LoginViewModel LoginVM { get; set; }

        private DatabaseService MySQlConnection;
        public MainViewModel()
        {

            MySQlConnection = new DatabaseService(App.connectionString);
            PurchaseVM = new PurchasesViewModel(this, MySQlConnection, MySQlConnection);
            PartVM = new PartsViewModel(MySQlConnection);
            ITAssemblyVM = new ITAssemblyViewModel(this, MySQlConnection, MySQlConnection);
            ASPartVM = new ASPartsViewModel(this, MySQlConnection);
            UserVM = new UsersViewModel(MySQlConnection);
            LoginVM = new LoginViewModel(this,MySQlConnection);
        }


        private bool _isLoggedIn = false;
        
        public bool IsLoggedIn
        {
            get
            {
                //LoginVM.LoginUser = new User { ID = 1, UserName = "admin" };
                

                return _isLoggedIn;

            }
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }

        private string _logtext;
        public string LogText
        {
            get {
                return _logtext;
            }
            set
            {
                _logtext = value;
                OnPropertyChanged(nameof(LogText));
            }

        }
    }
}
