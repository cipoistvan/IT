using Microsoft.Extensions.Logging;
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
                MessageBox.Show("Adatbázis kapcsolat hiba:\n" + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                App.logger.LogCritical(ex, "Adatbázis kapcsolat nem jött létre: ");
                App.Current.Shutdown();
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
                p.id AS id,
                p.userid as userid,
                u.username AS username,
                p.partid as partid,
                pr.name AS partname,
                c.id as categoryid,
                c.name AS categoryname,
                p.quantity AS quantity,
                p.unitprice AS unitprice,
                p.date AS date
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
                    UserId = reader.GetInt32("userid"),
                    UserName = reader.GetString("username"),
                    PartId = reader.GetInt32("partid"),
                    PartName = reader.GetString("partname"),
                    CategoryId = reader.GetInt32("categoryid"),
                    CategoryName = reader.GetString("categoryname"),
                    Quantity = reader.GetInt32("quantity"),
                    UnitPrice = reader.GetDecimal("unitprice"),
                    Date = reader.GetDateTime("date"),
                });
            }

            return list;
        }
        public DeleteResult DeletePurchase(Purchase purchase)
        {
            if (purchase is not null && purchase.ID > 0)
            {

                try
                {
                    var query = "DELETE FROM purchases WHERE Id = @id";
                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", purchase.ID);
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
        public UpdateResult ModifyPurchase(Purchase purchase)
        {
            if (purchase is not null)
            {

                try
                {
                    var query = @"UPDATE purchases 
                        set userid = @userid,
                            partid = @partid,
                            quantity = @quantity,
                            unitprice = @unitprice,
                            date = @date
                            WHERE id = @id";

                    using var conn = GetConnection();
                    var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", purchase.ID);
                    cmd.Parameters.AddWithValue("@userid", purchase.UserId);
                    cmd.Parameters.AddWithValue("@partid", purchase.PartId);
                    cmd.Parameters.AddWithValue("@quantity", purchase.Quantity);
                    cmd.Parameters.AddWithValue("@unitprice", purchase.UnitPrice);
                    cmd.Parameters.AddWithValue("@date", purchase.Date);

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
        public UpdateResult AddPurchase(Purchase purchase)
        {
            if (purchase is not null)
            {

                try
                {
                    var query = @"INSERT INTO purchases (userid, partid, quantity, unitprice, date)
                                VALUES (@userid, @partid, @quantity, @unitprice, @date)";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@userid", purchase.UserId);
                    cmd.Parameters.AddWithValue("@partid", purchase.PartId);
                    cmd.Parameters.AddWithValue("@quantity", purchase.Quantity);
                    cmd.Parameters.AddWithValue("@unitprice", purchase.UnitPrice);
                    cmd.Parameters.AddWithValue("@date", purchase.Date);
                    

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
        public DeleteResult DeletePart(Part part)
        {
            if (part is not null && part.ID > 0)
            {

                try
                {
                    var query = "DELETE FROM parts WHERE Id = @id";
                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", part.ID);
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
        public UpdateResult ModifyPart(Part part)
        {
            if (part is not null)
            {

                try
                {
                    var query = @"UPDATE parts 
                                SET name = @name,
                                    categoryid = @categoryid
                                WHERE id = @id";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", part.ID);
                    cmd.Parameters.AddWithValue("@name", part.Name);
                    cmd.Parameters.AddWithValue("@categoryid", part.CategoryId);

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
        public UpdateResult AddPart(Part part)
        {
            if (part is not null)
            {

                try
                {
                    var query = @"INSERT INTO parts (name, categoryid)
                                VALUES (@name, @categoryid)";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@name", part.Name);
                    cmd.Parameters.AddWithValue("@categoryid", part.CategoryId);

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
        public DeleteResult DeleteITAssembly(ITAssembly itassembly)
        {
            if (itassembly is not null && itassembly.ID > 0)
            {

                try
                {
                    var query = "DELETE FROM assemblies WHERE Id = @id";
                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", itassembly.ID);
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
        public UpdateResult AddITAssembly(ITAssembly itassembly)
        {
            if (itassembly is not null)
            {

                try
                {
                    var query = @"INSERT INTO assemblies (userid, name, date)
                                VALUES (@userid, @name, @date)";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@userid", itassembly.UserId);
                    cmd.Parameters.AddWithValue("@name", itassembly.Name);
                    cmd.Parameters.AddWithValue("@date", itassembly.Date);

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
        public UpdateResult ModifyITAssembly(ITAssembly itassembly)
        {
            if (itassembly is not null)
            {

                try
                {
                    var query = @"UPDATE assemblies 
                                SET userid = @userid,
                                    name = @name,
                                    date = @date
                                WHERE id = @id";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", itassembly.ID);
                    cmd.Parameters.AddWithValue("@userid", itassembly.UserId);
                    cmd.Parameters.AddWithValue("@name", itassembly.Name);
                    cmd.Parameters.AddWithValue("@date", itassembly.Date);

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


        public List<ASPart> GetASParts(int? assemblyid)
        {
            var list = new List<ASPart>();

            var query = @"
                SELECT 
                a.id AS ID,
                a.assemblyid as AssemblyID,
                ass.Name as AssemblyName,
                a.partid as PartID,
                p.name AS PartName,
                c.id As CategoryID,
                c.name AS CategoryName,
                a.quantity as Quantity
       
                FROM assemblyparts a
                JOIN assemblies ass ON a.AssemblyId = ass.id
                JOIN parts p ON a.partid = p.id
                JOIN categories c ON p.categoryid = c.id
                WHERE (@FAssemblyID is null or a.assemblyid = @FAssemblyID)
                ORDER BY p.id";


            using var conn = GetConnection();
            var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@FAssemblyID", assemblyid);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new ASPart
                {
                    ID = reader.GetInt32("ID"),
                    AssemblyID = reader.GetInt32("AssemblyID"),
                    AssemblyName = reader.GetString("AssemblyName"),
                    PartID = reader.GetInt32("PartID"),
                    PartName = reader.GetString("PartName"),
                    CategoryID = reader.GetInt32("CategoryID"),
                    CategoryName = reader.GetString("CategoryName"),
                    Quantity = reader.GetInt32("Quantity"),
                });
            }

            return list;
        }
        public List<ASPart> GetASParts()
        {
            return GetASParts(null);
        }

        public UpdateResult AddASPart(ASPart aspart)
        {
            if (aspart is not null)
            {

                try
                {
                    var query = @"INSERT INTO assemblyparts (assemblyid, partid, quantity)
                                VALUES (@assemblyid, @partid, @quantity)";

                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    
                    cmd.Parameters.AddWithValue("@assemblyid", aspart.AssemblyID);
                    cmd.Parameters.AddWithValue("@partid", aspart.PartID);
                    cmd.Parameters.AddWithValue("@quantity", aspart.Quantity);

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
        public DeleteResult DeleteASPart(ASPart aspart)
        {
            if (aspart is not null && aspart.ID > 0)
            {

                try
                {
                    var query = "DELETE FROM assemblyparts WHERE Id = @id";
                    using var conn = GetConnection();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", aspart.ID);
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
        public User GetUser(User user)
        {

            var query = @"
                SELECT 
                u.id AS ID,
                u.username AS UserName,
                u.passwordhash as Password,
                u.email AS Email
       
                FROM users u
                WHERE u.email = @email
                ";

            using var conn = GetConnection();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", user.Email);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    ID = reader.GetInt32("ID"),
                    UserName = reader.GetString("UserName"),
                    Password = reader.GetString("Password"),
                    Email = reader.GetString("Email")
                };


                return user;
            }

            return null;
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

