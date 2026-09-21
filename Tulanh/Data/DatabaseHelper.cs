using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Tulanh.Models;

namespace Tulanh.Data
{
    public class DatabaseHelper
    {
        private static string dbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tulanh.db");

        private static string connectionString =
            $"Data Source={dbPath}";


        // Tạo database và bảng
        public static void CreateDatabase()
        {
            using (SqliteConnection connection =
                   new SqliteConnection(connectionString))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Foods
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Category TEXT,
                        Compartment TEXT,
                        Quantity INTEGER,
                        Weight REAL,
                        ImportDate TEXT,
                        FreshDays INTEGER
                    )";

                using (SqliteCommand command =
                       new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        // Thêm thực phẩm
        public static void AddFood(Food food)
        {
            using (SqliteConnection connection =
                   new SqliteConnection(connectionString))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Foods
                    (
                        Name,
                        Category,
                        Compartment,
                        Quantity,
                        Weight,
                        ImportDate,
                        FreshDays
                    )
                    VALUES
                    (
                        @Name,
                        @Category,
                        @Compartment,
                        @Quantity,
                        @Weight,
                        @ImportDate,
                        @FreshDays
                    )";

                using (SqliteCommand command =
                       new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Name", food.Name);
                    command.Parameters.AddWithValue("@Category", food.Category);
                    command.Parameters.AddWithValue("@Compartment", food.Compartment);
                    command.Parameters.AddWithValue("@Quantity", food.Quantity);
                    command.Parameters.AddWithValue("@Weight", food.Weight);
                    command.Parameters.AddWithValue(
                        "@ImportDate",
                        food.ImportDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@FreshDays", food.FreshDays);

                    command.ExecuteNonQuery();
                }
            }
        }


        // Lấy danh sách thực phẩm
        public static List<Food> GetFoods()
        {
            List<Food> foods = new List<Food>();

            using (SqliteConnection connection =
                   new SqliteConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Foods";

                using (SqliteCommand command =
                       new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Food food = new Food();

                            food.Id = Convert.ToInt32(reader["Id"]);
                            food.Name = reader["Name"].ToString();
                            food.Category = reader["Category"].ToString();
                            food.Compartment = reader["Compartment"].ToString();
                            food.Quantity = Convert.ToInt32(reader["Quantity"]);
                            food.Weight = Convert.ToDouble(reader["Weight"]);

                            food.ImportDate =
                                DateTime.Parse(reader["ImportDate"].ToString());

                            food.FreshDays =
                                Convert.ToInt32(reader["FreshDays"]);

                            foods.Add(food);
                        }
                    }
                }
            }

            return foods;
        }


        // Xóa thực phẩm
        public static void DeleteFood(int id)
        {
            using (SqliteConnection connection =
                   new SqliteConnection(connectionString))
            {
                connection.Open();

                string sql = "DELETE FROM Foods WHERE Id = @Id";

                using (SqliteCommand command =
                       new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}