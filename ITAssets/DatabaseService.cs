using MySqlConnector;
using System.Data;

namespace ITAssets
{

    public class DatabaseService
    {
        private readonly string connectionString;

        public DatabaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Külön metódus a kapcsolat létrehozására
        public MySqlConnection GetConnection()
        {
            try 
            {
                var conn = new MySqlConnection(connectionString);
                conn.Open();
                return conn;
            }

            catch (MySqlException)
            {
                throw;
            }

        }

        // Általános lekérdező (csak olvasásra)
        public MySqlDataReader ExecuteQuery(MySqlConnection conn, string query)
        {
             var cmd = new MySqlCommand(query, conn);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection); // automatikus close
        }

        // Speciális lekérdezés: beszerzések megjelenítéséhez
        public List<Purchase> GetPurchases()
        {
            var list = new List<Purchase>();

            var query = @"
            SELECT 
                p.id AS ID,
                p.date AS Date,
                u.username AS User,
                pr.name AS ItemName,
                c.name AS Type,
                p.quantity AS Quantity,
                p.unitprice AS UnitPrice
            FROM purchases p
            JOIN users u ON p.userid = u.id
            JOIN parts pr ON p.partid = pr.id
            JOIN categories c ON pr.categoryid = c.id
            ORDER BY p.date DESC";

             using var conn = GetConnection();

             using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new Purchase
                {
                    ID = reader.GetInt32("ID"),
                    Date = reader.GetDateTime("Date"),
                    User = reader.GetString("User"),
                    ItemName = reader.GetString("ItemName"),
                    Type = reader.GetString("Type"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice")
                });
            }

            return list;
        }
    }
}

//public int ID { get; set; }
//public DateTime Date { get; set; }
//public string User { get; set; }
//public string ItemName { get; set; }
//public string Type { get; set; }
//public int Quantity { get; set; }
//public decimal UnitPrice { get; set; }
//public decimal Total => Quantity * UnitPrice;