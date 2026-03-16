using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class AppUserRepository : IRepository<AppUser>
    {
        private readonly string _connectionString;

        public AppUserRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<AppUser> GetAll()
        {
            var users = new List<AppUser>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, name, email, password, role, theater_id, created_at FROM app_user ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new AppUser
                                {
                                    UserId = reader["user_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    TheaterId = reader["theater_id"] != DBNull.Value ? reader["theater_id"].ToString() : null,
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users: " + ex.Message);
            }
            return users;
        }

        public AppUser GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, name, email, password, role, theater_id, created_at FROM app_user WHERE user_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new AppUser
                                {
                                    UserId = reader["user_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    TheaterId = reader["theater_id"] != DBNull.Value ? reader["theater_id"].ToString() : null,
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user: " + ex.Message);
            }
            return null;
        }

        public bool Insert(AppUser entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO app_user (name, email, password, role, theater_id) VALUES (:name, :email, :password, :role, :theaterId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":email", entity.Email);
                        command.Parameters.AddWithValue(":password", entity.Password);
                        command.Parameters.AddWithValue(":role", entity.Role);
                        command.Parameters.AddWithValue(":theaterId", entity.TheaterId ?? (object)DBNull.Value);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting user: " + ex.Message);
            }
        }

        public bool Update(AppUser entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE app_user SET name = :name, email = :email, password = :password, role = :role, theater_id = :theaterId WHERE user_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.UserId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":email", entity.Email);
                        command.Parameters.AddWithValue(":password", entity.Password);
                        command.Parameters.AddWithValue(":role", entity.Role);
                        command.Parameters.AddWithValue(":theaterId", entity.TheaterId ?? (object)DBNull.Value);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM app_user WHERE user_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user: " + ex.Message);
            }
        }

        public AppUser GetByEmail(string email)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, name, email, password, role, theater_id, created_at FROM app_user WHERE email = :email";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":email", email);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new AppUser
                                {
                                    UserId = reader["user_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    TheaterId = reader["theater_id"] != DBNull.Value ? reader["theater_id"].ToString() : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by email: " + ex.Message);
            }
            return null;
        }

        public List<AppUser> GetByTheaterId(string theaterId)
        {
            var users = new List<AppUser>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, name, email, password, role, theater_id, created_at FROM app_user WHERE theater_id = :theaterId ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":theaterId", theaterId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new AppUser
                                {
                                    UserId = reader["user_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    TheaterId = reader["theater_id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users by theater: " + ex.Message);
            }
            return users;
        }
    }
}
