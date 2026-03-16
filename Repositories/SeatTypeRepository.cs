using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class SeatTypeRepository : IRepository<SeatType>
    {
        private readonly string _connectionString;

        public SeatTypeRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<SeatType> GetAll()
        {
            var seatTypes = new List<SeatType>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT seat_type_id, name, description, price_multiplier, created_at FROM seat_type ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                seatTypes.Add(new SeatType
                                {
                                    SeatTypeId = reader["seat_type_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"].ToString(),
                                    PriceMultiplier = Convert.ToDecimal(reader["price_multiplier"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving seat types: " + ex.Message);
            }
            return seatTypes;
        }

        public SeatType GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT seat_type_id, name, description, price_multiplier, created_at FROM seat_type WHERE seat_type_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SeatType
                                {
                                    SeatTypeId = reader["seat_type_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"].ToString(),
                                    PriceMultiplier = Convert.ToDecimal(reader["price_multiplier"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving seat type: " + ex.Message);
            }
            return null;
        }

        public bool Insert(SeatType entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO seat_type (name, description, price_multiplier) VALUES (:name, :description, :multiplier)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":description", entity.Description ?? "");
                        command.Parameters.AddWithValue(":multiplier", entity.PriceMultiplier);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting seat type: " + ex.Message);
            }
        }

        public bool Update(SeatType entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE seat_type SET name = :name, description = :description, price_multiplier = :multiplier WHERE seat_type_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.SeatTypeId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":description", entity.Description ?? "");
                        command.Parameters.AddWithValue(":multiplier", entity.PriceMultiplier);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating seat type: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM seat_type WHERE seat_type_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting seat type: " + ex.Message);
            }
        }
    }
}
