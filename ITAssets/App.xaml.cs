using ITAssets;
using System.Configuration;
using System.Data;
using System.Windows;
using MySqlConnector;
using System.Diagnostics;

namespace ITAssets
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string connectionString = "server=localhost;database=itassets;user=root;password=;";


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
                


            {
                new DatabaseService(App.connectionString).GetConnection();
                //MessageBox.Show("Sikeres adatbázis kapcsolat!", "Kapcsolat teszt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Adatbázis kapcsolat hiba:\n" + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


    }

}
