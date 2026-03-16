using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class TheaterRepository : IRepository<Theater>
    {
        private readonly string _connectionString;

        public TheaterRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Theater> GetAll()
        {
            var theaters = new List<Theater>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT theater_id, name, location, created_at FROM theater ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                theaters.Add(new Theater
                                {
                                    TheaterId = reader["theater_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Location = reader["location"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving theaters: " + ex.Message);
            }
            return theaters;
        }

        public Theater GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT theater_id, name, location, created_at FROM theater WHERE theater_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Theater
                                {
                                    TheaterId = reader["theater_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Location = reader["location"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving theater: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Theater entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO theater (name, location) VALUES (:name, :location)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":location", entity.Location);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting theater: " + ex.Message);
            }
        }

        public bool Update(Theater entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE theater SET name = :name, location = :location WHERE theater_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.TheaterId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":location", entity.Location);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating theater: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM theater WHERE theater_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting theater: " + ex.Message);
            }
        }
    }
}
