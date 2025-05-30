using MySqlConnector;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ITAssets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            DataContext = new PurchasesViewModel();

            try
            {
                new DatabaseService(App.connectionString).GetConnection();
                MessageBox.Show("Sikeres adatbázis kapcsolat!", "Kapcsolat teszt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Adatbázis kapcsolat hiba:\n" + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }


        private void DBConnectionTest()
        {
            string connectionString = "server=localhost;database=itassets;user=root;password=;";

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();
                MessageBox.Show("Sikeres adatbázis kapcsolat!", "Kapcsolat teszt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Adatbázis kapcsolat hiba:\n" + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                new DatabaseService(App.connectionString).GetConnection();
                MessageBox.Show("Sikeres adatbázis kapcsolat!", "Kapcsolat teszt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Adatbázis kapcsolat hiba:\n" + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}