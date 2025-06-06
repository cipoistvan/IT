using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace ITAssets
{
    public class DatabaseService
    {
        private readonly string connectionString;
        public DatabaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public MySqlConnection GetConnection()
        {
            try
            {
                var conn = new MySqlConnection(connectionString);
                conn.Open();
                return conn;
            }

            catch (MySqlException ex)
            {
                throw;
            }

        }
        public MySqlDataReader ExecuteQuery(MySqlConnection conn, string query)
        {
            var cmd = new MySqlCommand(query, conn);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
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
        public List<Part> GetParts()
        {
            var list = new List<Part>();

            var query = @"
            SELECT 
                p.id AS ID,
                p.name AS Name,
                p.categoryid as CategoryId,
                c.name AS Category
            FROM parts p
            JOIN categories c ON p.categoryid = c.id
            ORDER BY p.id";

            using var conn = GetConnection();

            using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new Part
                {
                    ID = reader.GetInt32("ID"),
                    Name = reader.GetString("Name"),
                    CategoryId = reader.GetInt32("CategoryId"),
                    CategoryName = reader.GetString("Category"),
                });
            }

            return list;
        }
        public List<Category> GetCategories()
        {
            var list = new List<Category>();

            var query = @"
            SELECT 
                c.id AS ID,
                c.name AS Name
                
            FROM categories c
            ORDER BY c.name";

            using var conn = GetConnection();

            using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new Category
                {
                    ID = reader.GetInt32("ID"),
                    Name = reader.GetString("Name"),
                    
                });
            }

            return list;
        }
        public List<ITAssembly> GetITAssemblies()
        {
            var list = new List<ITAssembly>();

            var query = @"
            SELECT 
                a.id AS ID,
                a.name AS Name,
                a.date AS Date,
                u.username AS UserName
            FROM assemblies a
            JOIN users u ON a.userid = u.id
            ORDER BY a.id";

            using var conn = GetConnection();

            using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new ITAssembly
                {
                    ID = reader.GetInt32("ID"),
                    Name = reader.GetString("Name"),
                    Date = reader.GetDateTime("Date"),
                    UserName = reader.GetString("UserName")
                });
            }

            return list;
        }
        public List<ASPart> GetASParts()
        {
            var list = new List<ASPart>();

            var query = @"
                SELECT 
                a.id AS ID,
                p.name AS PartName,
                ass.Name as ASName,
                c.name AS Category,
                a.quantity as Quantity
       
                FROM assemblyparts a
                JOIN assemblies ass ON a.AssemblyId = ass.id
                JOIN parts p ON a.partid = p.id
                JOIN categories c ON p.categoryid = c.id
                ORDER BY p.id";

            using var conn = GetConnection();

            using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new ASPart
                {
                    ID = reader.GetInt32("ID"),
                    PartName = reader.GetString("PartName"),
                    AssemblyName = reader.GetString("ASName"),
                    CategoryName = reader.GetString("Category"),
                    Quantity = reader.GetInt32("Quantity"),

                });
            }

            return list;
        }
        public List<User> GetUsers()
        {
            var list = new List<User>();

            var query = @"
                SELECT 
                u.id AS ID,
                u.username AS UserName,
                u.passwordhash as Password,
                u.email AS Email
       
                FROM users u
                ORDER BY u.id";

            using var conn = GetConnection();

            using var reader = ExecuteQuery(conn, query);

            while (reader.Read())
            {
                list.Add(new User
                {
                    ID = reader.GetInt32("ID"),
                    UserName = reader.GetString("UserName"),
                    Password = reader.GetString("Password"),
                    Email= reader.GetString("Email")

                });
            }

            return list;
        }
        public DeleteResult DeleteUser(User user)
        {
            if (user is not null && user.ID > 0 )
            {

                try
                {
                    var query = "DELETE FROM users WHERE Id = @id";
                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", user.ID);
                    cmd.ExecuteNonQuery();
                    return DeleteResult.Success;

                }
                catch (MySqlException ex) when (ex.Number == 1451)
                {
                    return DeleteResult.ForeignKeyConstraint;
                }
                catch
                {
                    return DeleteResult.Error;
                }
            }
            return DeleteResult.NothingToDelete;
        }
        public UpdateResult ModifyUser(User user)
        {
            if (user is not null)
            {

                try
                {
                    var query = @"UPDATE users 
                                SET username = @username,
                                    passwordhash = @password,
                                    email = @email
                                WHERE Id = @id";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", user.ID);
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@email", user.Email);

                    cmd.ExecuteNonQuery();
                    return UpdateResult.Success;

                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    return UpdateResult.Duplicate;
                }
                catch
                {
                    return UpdateResult.Error;
                }
            }
            return UpdateResult.NothingToUpdate;
        }
        public UpdateResult AddUser(User user)
        {
            if (user is not null)
            {

                try
                {
                    var query = @"INSERT INTO users (username, passwordhash, email)
                                VALUES (@username, @password, @email)";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@email", user.Email);

                    cmd.ExecuteNonQuery();
                    return UpdateResult.Success;

                }

                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    return UpdateResult.Duplicate;
                }
                catch
                {
                    return UpdateResult.Error;
                }
            }
            return UpdateResult.NothingToUpdate;


        }
    }
    public enum DeleteResult
    {
        Success,
        ForeignKeyConstraint,
        Error,
        NothingToDelete
    }
    public enum UpdateResult
    {
        Success,
        Duplicate,
        Error,
        NothingToUpdate
    }
}

