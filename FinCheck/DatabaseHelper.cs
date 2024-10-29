using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCheck
{
    using Microsoft.Data.Sqlite;
    using System.Data;
    using System.Windows;

    public class DatabaseHelper
    {
        private readonly string _connectionString = "Data Source=fincheck.db";

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var deleteTables = @"DROP TABLE IF EXISTS Users;
                                    DROP TABLE IF EXISTS Wallets;
                                    DROP TABLE IF EXISTS Transactions;";

                var createUsersTable = @"CREATE TABLE IF NOT EXISTS Users (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Username TEXT NOT NULL UNIQUE,
                                        Password TEXT NOT NULL);";

                var createWalletsTable = @"CREATE TABLE IF NOT EXISTS Wallets (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserID INTEGER NOT NULL,
                                        WalletName TEXT NOT NULL,
                                        Currency TEXT NOT NULL,
                                        FOREIGN KEY(UserID) REFERENCES Users(ID));";

                var createTransactionsTable = @"CREATE TABLE IF NOT EXISTS Transactions (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        WalletID INTEGER NOT NULL,
                                        Type TEXT NOT NULL,
                                        Category TEXT NOT NULL,
                                        Amount REAL NOT NULL,
                                        Description TEXT,
                                        DateCreated DATETIME NOT NULL,
                                        FOREIGN KEY(WalletID) REFERENCES Wallets(ID));";


                //using (var command = new SqliteCommand(deleteTables, connection))
                //{
                //    command.ExecuteNonQuery();
                //}

                using (var command = new SqliteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createWalletsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createTransactionsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool AddUser(string username, string password)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var insertUserCommand = @"INSERT INTO Users (Username, Password) 
                                          VALUES (@Username, @Password);";

                    using (var command = new SqliteCommand(insertUserCommand, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}");
                return false;
            }
        }

        public User GetUser(string username, string password)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var selectUserCommand = @"SELECT ID, Username, Password FROM Users 
                                      WHERE Username = @Username AND Password = @Password;";

                using (var command = new SqliteCommand(selectUserCommand, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userID = reader.GetInt32(0);
                            MessageBox.Show(userID.ToString());
                            string retrievedUsername = reader.GetString(1);
                            MessageBox.Show(retrievedUsername.ToString());
                            string retrievedPassword = reader.GetString(2);
                            MessageBox.Show(retrievedPassword.ToString());
                            User user = new User(userID, retrievedUsername, retrievedPassword);

                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public void AddWallet(int userId, string walletName, string currency)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO Wallets (UserID, WalletName, Currency) VALUES (@UserId, @WalletName, @Currency)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@WalletName", walletName);
                    command.Parameters.AddWithValue("@Currency", currency);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateWallet(int walletID, int userId, string walletName, string currency)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var updateCommand = @"UPDATE Wallets
                                      SET UserID = @UserID, WalletName = @WalletName, Currency = @Currency
                                      WHERE ID = @WalletID;";
                using(var command = new SqliteCommand(updateCommand, connection))
                {
                    command.Parameters.AddWithValue("@WalletID", walletID);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@WalletName", walletName);
                    command.Parameters.AddWithValue("@Currency", currency);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Wallet> GetAllWallets()
        {
            var wallets = new List<Wallet>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ID, UserID, WalletName, Currency FROM Wallets";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var wallet = new Wallet(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetString(2),
                            reader.GetString(3)
                        );

                        wallets.Add(wallet);
                    }
                }
            }

            return wallets;
        }

        public List<Wallet> GetWalletsForUser(int userId)
        {
            List<Wallet> wallets = new List<Wallet>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT ID, WalletName, Currency FROM Wallets WHERE UserID = @UserId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string walletName = reader.GetString(1);
                            string currency = reader.GetString(2);

                            wallets.Add(new Wallet(id, userId, walletName, currency));
                        }
                    }
                }
            }

            return wallets;
        }

        public void DeleteWallet(int walletId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = "DELETE FROM Wallets WHERE ID = @WalletId";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@WalletId", walletId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddTransaction(int walletID, string type, string category, decimal amount, string description, string dateTime)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var insertCommand = @"INSERT INTO Transactions (WalletID, Type, Category, Amount, Description, DateCreated)
                                      VALUES (@WalletID, @Type, @Category, @Amount, @Description, @DateCreated);";

                using (var command = new SqliteCommand(insertCommand, connection))
                {
                    command.Parameters.AddWithValue("@WalletID", walletID);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateCreated", dateTime);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTransaction(int transactionID)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var deleteCommand = @"DELETE FROM Transactions WHERE ID = @TransactionID;";

                using (var command = new SqliteCommand(deleteCommand, connection))
                {
                    command.Parameters.AddWithValue("@TransactionID", transactionID);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTransaction(int transactionID, int walletID, string type, string category, decimal amount, string description, string dateTime)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var updateCommand = @"UPDATE Transactions 
                                      SET WalletID = @WalletID, Type = @Type, Category = @Category, Amount = @Amount,
                                          Description = @Description, DateCreated = @DateCreated
                                      WHERE ID = @TransactionID;";

                using (var command = new SqliteCommand(updateCommand, connection))
                {
                    command.Parameters.AddWithValue("@TransactionID", transactionID);
                    command.Parameters.AddWithValue("@WalletID", walletID);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateCreated", dateTime);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Transaction> GetTransactionsByWalletID(int walletID)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var selectCommand = @"SELECT ID, WalletID, Type, Category, Amount, Description, DateCreated
                              FROM Transactions
                              WHERE WalletID = @WalletID;";

                using (var command = new SqliteCommand(selectCommand, connection))
                {
                    command.Parameters.AddWithValue("@WalletID", walletID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int retrievedWalletID = reader.GetInt32(1);
                            string type = reader.GetString(2);
                            string category = reader.GetString(3);
                            decimal amount = reader.GetDecimal(4);
                            string description = reader.IsDBNull(5) ? null : reader.GetString(5);
                            string dateCreated = reader.GetString(6);

                            Transaction transaction = new Transaction(id, retrievedWalletID, type, category, amount, description, dateCreated);
                            transactions.Add(transaction);
                        }
                    }
                }
            }

            return transactions;
        }

        public decimal GetBalance(int walletID)
        {
            List<Transaction> transactions = new List<Transaction>();

            decimal balance = 0m;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var selectCommand = @"SELECT ID, WalletID, Type, Category, Amount, Description, DateCreated
                              FROM Transactions
                              WHERE WalletID = @WalletID;";

                using (var command = new SqliteCommand(selectCommand, connection))
                {
                    command.Parameters.AddWithValue("@WalletID", walletID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = reader.GetString(2);
                            decimal amount = reader.GetDecimal(4);

                            if (type == "Расход")
                            {
                                balance -= amount;
                            }
                            else if(type == "Доход")
                            {
                                balance += amount;
                            }
                        }
                    }
                }
            }

            return balance;
        }

        public List<Transaction> GetTransactionsForUser(int userId)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = @"SELECT DateCreated, Type, Category, Amount, Description 
                      FROM Transactions 
                      JOIN Wallets ON Transactions.WalletID = Wallets.ID 
                      WHERE Wallets.UserID = @UserID";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new Transaction
                            (
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetDecimal(4),
                                reader.GetString(5),
                                reader.GetDateTime(6).ToString("yyyy:MM:dd HH:mm:ss")
                            ));
                        }
                    }
                }
            }

            return transactions;
        }
    }
}
