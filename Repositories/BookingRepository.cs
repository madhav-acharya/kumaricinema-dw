using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class BookingRepository : IRepository<Booking>
    {
        private readonly string _connectionString;

        public BookingRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Booking> GetAll()
        {
            var bookings = new List<Booking>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT booking_id, total_amount, user_id, show_id FROM booking ORDER BY booking_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookings.Add(new Booking
                                {
                                    BookingId = reader["booking_id"].ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                                    UserId = reader["user_id"].ToString(),
                                    ShowId = reader["show_id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings: " + ex.Message);
            }
            return bookings;
        }

        public Booking GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT booking_id, total_amount, user_id, show_id FROM booking WHERE booking_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Booking
                                {
                                    BookingId = reader["booking_id"].ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                                    UserId = reader["user_id"].ToString(),
                                    ShowId = reader["show_id"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Booking entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO booking (booking_id, total_amount, user_id, show_id) VALUES (:id, :amount, :userId, :showId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.BookingId);
                        command.Parameters.AddWithValue(":amount", entity.TotalAmount);
                        command.Parameters.AddWithValue(":userId", entity.UserId);
                        command.Parameters.AddWithValue(":showId", entity.ShowId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting booking: " + ex.Message);
            }
        }

        public bool Update(Booking entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE booking SET total_amount = :amount, user_id = :userId, show_id = :showId WHERE booking_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.BookingId);
                        command.Parameters.AddWithValue(":amount", entity.TotalAmount);
                        command.Parameters.AddWithValue(":userId", entity.UserId);
                        command.Parameters.AddWithValue(":showId", entity.ShowId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM booking WHERE booking_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting booking: " + ex.Message);
            }
        }

        public List<Booking> GetByUserId(string userId)
        {
            var bookings = new List<Booking>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT booking_id, total_amount, user_id, show_id FROM booking WHERE user_id = :userId ORDER BY booking_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookings.Add(new Booking
                                {
                                    BookingId = reader["booking_id"].ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                                    UserId = reader["user_id"].ToString(),
                                    ShowId = reader["show_id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings by user: " + ex.Message);
            }
            return bookings;
        }

        public List<string> GetSeatsByBookingId(string bookingId)
        {
            var seats = new List<string>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT seat_id FROM booking_seat WHERE booking_id = :bookingId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":bookingId", bookingId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                seats.Add(reader["seat_id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving seats for booking: " + ex.Message);
            }
            return seats;
        }

        public bool AddSeatToBooking(string bookingId, string seatId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO booking_seat (booking_id, seat_id) VALUES (:bookingId, :seatId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":bookingId", bookingId);
                        command.Parameters.AddWithValue(":seatId", seatId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding seat to booking: " + ex.Message);
            }
        }

        public bool RemoveSeatFromBooking(string bookingId, string seatId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM booking_seat WHERE booking_id = :bookingId AND seat_id = :seatId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":bookingId", bookingId);
                        command.Parameters.AddWithValue(":seatId", seatId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing seat from booking: " + ex.Message);
            }
        }
    }
}
