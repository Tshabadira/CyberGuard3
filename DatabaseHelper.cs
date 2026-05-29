using System;
using MySql.Data.MySqlClient;

namespace CyberGuard
{
    internal static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["CyberGuardDB"].ConnectionString;

        public static void EnsureTableCreated()
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(255) NOT NULL,
                    description TEXT,
                    reminder_date DATETIME NULL,
                    is_completed BOOLEAN DEFAULT FALSE,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or show error – you can integrate with ActivityLog later
                System.Windows.Forms.MessageBox.Show("Database error: " + ex.Message);
            }
        }
    }
}