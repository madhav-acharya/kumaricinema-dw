using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class PaymentRepository : IRepository<Payment>
    {
        private readonly string _connectionString;

        public PaymentRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Payment> GetAll()
        {
            var payments = new List<Payment>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT payment_id, booking_id, amount_paid, payment_method, payment_status FROM payment ORDER BY payment_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new Payment
                                {
                                    PaymentId = reader["payment_id"].ToString(),
                                    BookingId = reader["booking_id"].ToString(),
                                    AmountPaid = Convert.ToDecimal(reader["amount_paid"]),
                                    PaymentMethod = reader["payment_method"].ToString(),
                                    PaymentStatus = reader["payment_status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving payments: " + ex.Message);
            }
            return payments;
        }

        public Payment GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT payment_id, booking_id, amount_paid, payment_method, payment_status FROM payment WHERE payment_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Payment
                                {
                                    PaymentId = reader["payment_id"].ToString(),
                                    BookingId = reader["booking_id"].ToString(),
                                    AmountPaid = Convert.ToDecimal(reader["amount_paid"]),
                                    PaymentMethod = reader["payment_method"].ToString(),
                                    PaymentStatus = reader["payment_status"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving payment: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Payment entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO payment (payment_id, booking_id, amount_paid, payment_method, payment_status) VALUES (:id, :bookingId, :amount, :method, :status)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.PaymentId);
                        command.Parameters.AddWithValue(":bookingId", entity.BookingId);
                        command.Parameters.AddWithValue(":amount", entity.AmountPaid);
                        command.Parameters.AddWithValue(":method", entity.PaymentMethod);
                        command.Parameters.AddWithValue(":status", entity.PaymentStatus);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting payment: " + ex.Message);
            }
        }

        public bool Update(Payment entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE payment SET booking_id = :bookingId, amount_paid = :amount, payment_method = :method, payment_status = :status WHERE payment_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.PaymentId);
                        command.Parameters.AddWithValue(":bookingId", entity.BookingId);
                        command.Parameters.AddWithValue(":amount", entity.AmountPaid);
                        command.Parameters.AddWithValue(":method", entity.PaymentMethod);
                        command.Parameters.AddWithValue(":status", entity.PaymentStatus);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating payment: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM payment WHERE payment_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting payment: " + ex.Message);
            }
        }

        public List<Payment> GetByBookingId(string bookingId)
        {
            var payments = new List<Payment>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT payment_id, booking_id, amount_paid, payment_method, payment_status FROM payment WHERE booking_id = :bookingId ORDER BY payment_id DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":bookingId", bookingId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new Payment
                                {
                                    PaymentId = reader["payment_id"].ToString(),
                                    BookingId = reader["booking_id"].ToString(),
                                    AmountPaid = Convert.ToDecimal(reader["amount_paid"]),
                                    PaymentMethod = reader["payment_method"].ToString(),
                                    PaymentStatus = reader["payment_status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving payments by booking: " + ex.Message);
            }
            return payments;
        }
    }
}
