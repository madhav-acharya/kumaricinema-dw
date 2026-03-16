using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class TicketRepository : IRepository<Ticket>
    {
        private readonly string _connectionString;

        public TicketRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ticket_id, seat_id, show_id, ticket_price, ticket_status, created_at FROM ticket ORDER BY ticket_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new Ticket
                                {
                                    TicketId = reader["ticket_id"].ToString(),
                                    SeatId = reader["seat_id"].ToString(),
                                    ShowId = reader["show_id"].ToString(),
                                    TicketPrice = Convert.ToDecimal(reader["ticket_price"]),
                                    TicketStatus = reader["ticket_status"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tickets: " + ex.Message);
            }
            return tickets;
        }

        public Ticket GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ticket_id, seat_id, show_id, ticket_price, ticket_status, created_at FROM ticket WHERE ticket_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Ticket
                                {
                                    TicketId = reader["ticket_id"].ToString(),
                                    SeatId = reader["seat_id"].ToString(),
                                    ShowId = reader["show_id"].ToString(),
                                    TicketPrice = Convert.ToDecimal(reader["ticket_price"]),
                                    TicketStatus = reader["ticket_status"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving ticket: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Ticket entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO ticket (seat_id, show_id, ticket_price, ticket_status) VALUES (:seatId, :showId, :price, :status)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":seatId", entity.SeatId);
                        command.Parameters.AddWithValue(":showId", entity.ShowId);
                        command.Parameters.AddWithValue(":price", entity.TicketPrice);
                        command.Parameters.AddWithValue(":status", entity.TicketStatus);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting ticket: " + ex.Message);
            }
        }

        public bool Update(Ticket entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE ticket SET seat_id = :seatId, show_id = :showId, ticket_price = :price, ticket_status = :status WHERE ticket_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.TicketId);
                        command.Parameters.AddWithValue(":seatId", entity.SeatId);
                        command.Parameters.AddWithValue(":showId", entity.ShowId);
                        command.Parameters.AddWithValue(":price", entity.TicketPrice);
                        command.Parameters.AddWithValue(":status", entity.TicketStatus);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ticket: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM ticket WHERE ticket_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting ticket: " + ex.Message);
            }
        }

        public List<Ticket> GetByShowId(string showId)
        {
            var tickets = new List<Ticket>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ticket_id, seat_id, show_id, ticket_price, ticket_status, created_at FROM ticket WHERE show_id = :showId ORDER BY ticket_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":showId", showId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new Ticket
                                {
                                    TicketId = reader["ticket_id"].ToString(),
                                    SeatId = reader["seat_id"].ToString(),
                                    ShowId = reader["show_id"].ToString(),
                                    TicketPrice = Convert.ToDecimal(reader["ticket_price"]),
                                    TicketStatus = reader["ticket_status"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tickets by show: " + ex.Message);
            }
            return tickets;
        }
    }
}
