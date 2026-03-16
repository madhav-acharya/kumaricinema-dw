using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class GenreRepository : IRepository<Genre>
    {
        private readonly string _connectionString;

        public GenreRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Genre> GetAll()
        {
            var genres = new List<Genre>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT genre_id, name, description, created_at FROM genre ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                genres.Add(new Genre
                                {
                                    GenreId = reader["genre_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving genres: " + ex.Message);
            }
            return genres;
        }

        public Genre GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT genre_id, name, description, created_at FROM genre WHERE genre_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Genre
                                {
                                    GenreId = reader["genre_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving genre: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Genre entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO genre (name, description) VALUES (:name, :description)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":description", entity.Description ?? string.Empty);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting genre: " + ex.Message);
            }
        }

        public bool Update(Genre entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE genre SET name = :name, description = :description WHERE genre_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.GenreId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":description", entity.Description ?? "");
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating genre: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM genre WHERE genre_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting genre: " + ex.Message);
            }
        }
    }
}
