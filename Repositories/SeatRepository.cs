using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class SeatRepository : IRepository<Seat>
    {
        private readonly string _connectionString;

        public SeatRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Seat> GetAll()
        {
            var seats = new List<Seat>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT seat_id, seat_number, status, seat_type_id, created_at FROM seat ORDER BY seat_number";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                seats.Add(new Seat
                                {
                                    SeatId = reader["seat_id"].ToString(),
                                    SeatNumber = reader["seat_number"].ToString(),
                                    Status = reader["status"].ToString(),
                                    SeatTypeId = reader["seat_type_id"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving seats: " + ex.Message);
            }
            return seats;
        }

        public Seat GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT seat_id, seat_number, status, seat_type_id, created_at FROM seat WHERE seat_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Seat
                                {
                                    SeatId = reader["seat_id"].ToString(),
                                    SeatNumber = reader["seat_number"].ToString(),
                                    Status = reader["status"].ToString(),
                                    SeatTypeId = reader["seat_type_id"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving seat: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Seat entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO seat (seat_number, status, seat_type_id) VALUES (:number, :status, :seatTypeId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":number", entity.SeatNumber);
                        command.Parameters.AddWithValue(":status", entity.Status);
                        command.Parameters.AddWithValue(":seatTypeId", entity.SeatTypeId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting seat: " + ex.Message);
            }
        }

        public bool Update(Seat entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE seat SET seat_number = :number, status = :status, seat_type_id = :seatTypeId WHERE seat_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.SeatId);
                        command.Parameters.AddWithValue(":number", entity.SeatNumber);
                        command.Parameters.AddWithValue(":status", entity.Status);
                        command.Parameters.AddWithValue(":seatTypeId", entity.SeatTypeId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating seat: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM seat WHERE seat_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting seat: " + ex.Message);
            }
        }
    }
}
