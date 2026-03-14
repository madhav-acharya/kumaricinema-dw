using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class LanguageRepository : IRepository<Language>
    {
        private readonly string _connectionString;

        public LanguageRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Language> GetAll()
        {
            var languages = new List<Language>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT language_id, name, code FROM language ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(new Language
                                {
                                    LanguageId = reader["language_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving languages: " + ex.Message);
            }
            return languages;
        }

        public Language GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT language_id, name, code FROM language WHERE language_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Language
                                {
                                    LanguageId = reader["language_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving language: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Language entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO language (language_id, name, code) VALUES (:id, :name, :code)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.LanguageId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":code", entity.Code);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting language: " + ex.Message);
            }
        }

        public bool Update(Language entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE language SET name = :name, code = :code WHERE language_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.LanguageId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":code", entity.Code);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating language: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM language WHERE language_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting language: " + ex.Message);
            }
        }
    }
}
