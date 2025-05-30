using System.Configuration;
using System.Data;
using System.Windows;

namespace ITAssets
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string connectionString = "server=localhost;database=itassets;user=root;password=;";

        // public static DatabaseService datasvc = new DatabaseService(connectionString);



    }

}
