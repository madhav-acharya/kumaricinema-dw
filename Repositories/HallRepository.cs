using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class HallRepository : IRepository<Hall>
    {
        private readonly string _connectionString;

        public HallRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Hall> GetAll()
        {
            var halls = new List<Hall>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT hall_id, hall_name, capacity, screen_type, theater_id FROM hall ORDER BY hall_name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                halls.Add(new Hall
                                {
                                    HallId = reader["hall_id"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    Capacity = Convert.ToInt32(reader["capacity"]),
                                    ScreenType = reader["screen_type"].ToString(),
                                    TheaterId = reader["theater_id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving halls: " + ex.Message);
            }
            return halls;
        }

        public Hall GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT hall_id, hall_name, capacity, screen_type, theater_id FROM hall WHERE hall_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Hall
                                {
                                    HallId = reader["hall_id"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    Capacity = Convert.ToInt32(reader["capacity"]),
                                    ScreenType = reader["screen_type"].ToString(),
                                    TheaterId = reader["theater_id"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving hall: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Hall entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO hall (hall_id, hall_name, capacity, screen_type, theater_id) VALUES (:id, :name, :capacity, :screenType, :theaterId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.HallId);
                        command.Parameters.AddWithValue(":name", entity.HallName);
                        command.Parameters.AddWithValue(":capacity", entity.Capacity);
                        command.Parameters.AddWithValue(":screenType", entity.ScreenType);
                        command.Parameters.AddWithValue(":theaterId", entity.TheaterId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting hall: " + ex.Message);
            }
        }

        public bool Update(Hall entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE hall SET hall_name = :name, capacity = :capacity, screen_type = :screenType, theater_id = :theaterId WHERE hall_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.HallId);
                        command.Parameters.AddWithValue(":name", entity.HallName);
                        command.Parameters.AddWithValue(":capacity", entity.Capacity);
                        command.Parameters.AddWithValue(":screenType", entity.ScreenType);
                        command.Parameters.AddWithValue(":theaterId", entity.TheaterId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating hall: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM hall WHERE hall_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting hall: " + ex.Message);
            }
        }

        public List<Hall> GetByTheaterId(string theaterId)
        {
            var halls = new List<Hall>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT hall_id, hall_name, capacity, screen_type, theater_id FROM hall WHERE theater_id = :theaterId ORDER BY hall_name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":theaterId", theaterId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                halls.Add(new Hall
                                {
                                    HallId = reader["hall_id"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    Capacity = Convert.ToInt32(reader["capacity"]),
                                    ScreenType = reader["screen_type"].ToString(),
                                    TheaterId = reader["theater_id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving halls by theater: " + ex.Message);
            }
            return halls;
        }
    }
}
