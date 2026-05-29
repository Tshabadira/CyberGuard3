using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CyberGuard
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    internal class TaskManager
    {
        private readonly string _connectionString;

        public TaskManager()
        {
            _connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["CyberGuardDB"].ConnectionString;
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            string query = @"INSERT INTO tasks (title, description, reminder_date) 
                             VALUES (@title, @desc, @reminder);
                             SELECT LAST_INSERT_ID();";
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@reminder", (object)reminderDate ?? DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<TaskItem> GetTasks(bool includeCompleted = false)
        {
            string query = "SELECT * FROM tasks WHERE is_completed = @completed ORDER BY reminder_date ASC, created_at DESC";
            var list = new List<TaskItem>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@completed", includeCompleted ? 0 : 0); // actually need to filter
                    // Better: pass parameter for completed status
                }
            }

            // Re-write with proper filter:
            string query2 = includeCompleted ?
                "SELECT * FROM tasks ORDER BY is_completed ASC, reminder_date ASC" :
                "SELECT * FROM tasks WHERE is_completed = FALSE ORDER BY reminder_date ASC";
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query2, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TaskItem
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                                ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? (DateTime?)null : reader.GetDateTime("reminder_date"),
                                IsCompleted = reader.GetBoolean("is_completed"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool DeleteTask(int id)
        {
            string query = "DELETE FROM tasks WHERE id = @id";
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool MarkComplete(int id)
        {
            string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}