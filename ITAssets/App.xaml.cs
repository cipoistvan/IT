using ITAssets;
using System.Configuration;
using System.Data;
using System.Windows;
using MySqlConnector;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ITAssets
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string connectionString = "server=localhost;database=itassets;user=root;password=;";
        public static ILogger logger;
        public static string mainlogtext;
        public static MainViewModel MainVM;
        public static string logFilePath = "ITLogFile.txt";


protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using ILoggerFactory factory = LoggerFactory.Create(builder => {
                builder.AddProvider(new ITLoggerProvider());
                builder.AddProvider(new ITFileLoggerProvider(logFilePath));
            });
            logger = factory.CreateLogger("IT nyilvántartó");
            logger.LogInformation("Logolás elindult !");

            try
            {
                new DatabaseService(App.connectionString).GetConnection();
            }
            catch (MySqlException ex)
            {
                Shutdown();
                return;
            }

            MainVM = new MainViewModel();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


    }

}
