using KumariCinema.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class ReportsRepository
    {
        private readonly string _connectionString;

        public ReportsRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<UserTicketReportRow> GetUserTicketsForSixMonths(string userId, DateTime periodStart, string theaterId = null)
        {
            var rows = new List<UserTicketReportRow>();
            var periodEnd = periodStart.AddMonths(6);

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
SELECT 
    u.user_id,
    u.name AS user_name,
    u.email,
    b.booking_id,
    t.ticket_id,
    s.seat_number,
    m.name AS movie_name,
    h.hall_name,
    th.name AS theater_name,
    th.location AS theater_city,
    ms.start_time,
    t.ticket_price,
    p.payment_status,
    b.created_at AS booking_date
FROM booking b
INNER JOIN app_user u ON u.user_id = b.user_id
INNER JOIN booking_seat bs ON bs.booking_id = b.booking_id
INNER JOIN ticket t ON t.show_id = b.show_id AND t.seat_id = bs.seat_id
INNER JOIN seat s ON s.seat_id = t.seat_id
INNER JOIN movie_show ms ON ms.show_id = b.show_id
INNER JOIN movie m ON m.movie_id = ms.movie_id
INNER JOIN hall h ON h.hall_id = ms.hall_id
INNER JOIN theater th ON th.theater_id = h.theater_id
LEFT JOIN payment p ON p.booking_id = b.booking_id
WHERE b.user_id = :userId
  AND b.created_at >= :periodStart
  AND b.created_at < :periodEnd";

                    if (!string.IsNullOrEmpty(theaterId))
                    {
                        query += " AND th.theater_id = :theaterId";
                    }

                    query += " ORDER BY b.created_at DESC, t.ticket_id DESC";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":userId", userId);
                        command.Parameters.AddWithValue(":periodStart", periodStart);
                        command.Parameters.AddWithValue(":periodEnd", periodEnd);
                        if (!string.IsNullOrEmpty(theaterId))
                        {
                            command.Parameters.AddWithValue(":theaterId", theaterId);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rows.Add(new UserTicketReportRow
                                {
                                    UserId = reader["user_id"].ToString(),
                                    UserName = reader["user_name"].ToString(),
                                    UserEmail = reader["email"].ToString(),
                                    BookingId = reader["booking_id"].ToString(),
                                    TicketId = reader["ticket_id"].ToString(),
                                    SeatNumber = reader["seat_number"].ToString(),
                                    MovieName = reader["movie_name"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    TheaterName = reader["theater_name"].ToString(),
                                    TheaterCity = reader["theater_city"].ToString(),
                                    ShowTime = Convert.ToDateTime(reader["start_time"]),
                                    TicketPrice = Convert.ToDecimal(reader["ticket_price"]),
                                    PaymentStatus = reader["payment_status"] == DBNull.Value ? "unpaid" : reader["payment_status"].ToString(),
                                    BookingDate = Convert.ToDateTime(reader["booking_date"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user ticket report: " + ex.Message);
            }

            return rows;
        }

        public List<TheaterCityHallMovieRow> GetTheaterCityHallMovies(string theaterId, string hallId)
        {
            var rows = new List<TheaterCityHallMovieRow>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
SELECT
    th.theater_id,
    th.name AS theater_name,
    th.location AS theater_city,
    h.hall_id,
    h.hall_name,
    m.movie_id,
    m.name AS movie_name,
    ms.start_time,
    ms.end_time,
    ms.show_category,
    ms.base_ticket_price
FROM movie_show ms
INNER JOIN movie m ON m.movie_id = ms.movie_id
INNER JOIN hall h ON h.hall_id = ms.hall_id
INNER JOIN theater th ON th.theater_id = h.theater_id
WHERE th.theater_id = :theaterId";

                    if (!string.IsNullOrEmpty(hallId))
                    {
                        query += " AND h.hall_id = :hallId";
                    }

                    query += " ORDER BY ms.start_time DESC";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":theaterId", theaterId);
                        if (!string.IsNullOrEmpty(hallId))
                        {
                            command.Parameters.AddWithValue(":hallId", hallId);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rows.Add(new TheaterCityHallMovieRow
                                {
                                    TheaterId = reader["theater_id"].ToString(),
                                    TheaterName = reader["theater_name"].ToString(),
                                    TheaterCity = reader["theater_city"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    MovieName = reader["movie_name"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving theater city hall movie report: " + ex.Message);
            }

            return rows;
        }

        public List<MovieTheaterCityHallOccupancyRow> GetTopOccupancyPerformers(string movieId, string theaterId = null)
        {
            var rows = new List<MovieTheaterCityHallOccupancyRow>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
SELECT * FROM (
    SELECT
        m.movie_id,
        m.name AS movie_name,
        th.theater_id,
        th.name AS theater_name,
        th.location AS theater_city,
        h.hall_id,
        h.hall_name,
        h.capacity AS hall_capacity,
        COUNT(DISTINCT ms.show_id) AS show_count,
        COUNT(DISTINCT t.ticket_id) AS paid_tickets,
        ROUND(
            CASE
                WHEN h.capacity * COUNT(DISTINCT ms.show_id) = 0 THEN 0
                ELSE (COUNT(DISTINCT t.ticket_id) * 100.0) / (h.capacity * COUNT(DISTINCT ms.show_id))
            END,
            2
        ) AS occupancy_percentage
    FROM movie_show ms
    INNER JOIN movie m ON m.movie_id = ms.movie_id
    INNER JOIN hall h ON h.hall_id = ms.hall_id
    INNER JOIN theater th ON th.theater_id = h.theater_id
    LEFT JOIN booking b ON b.show_id = ms.show_id
    LEFT JOIN payment p ON p.booking_id = b.booking_id AND LOWER(p.payment_status) = 'paid'
    LEFT JOIN booking_seat bs ON bs.booking_id = b.booking_id
    LEFT JOIN ticket t ON t.show_id = ms.show_id AND t.seat_id = bs.seat_id AND p.payment_id IS NOT NULL
    WHERE ms.movie_id = :movieId";

                    if (!string.IsNullOrEmpty(theaterId))
                    {
                        query += " AND th.theater_id = :theaterId";
                    }

                    query += @"
    GROUP BY
        m.movie_id,
        m.name,
        th.theater_id,
        th.name,
        th.location,
        h.hall_id,
        h.hall_name,
        h.capacity
    ORDER BY occupancy_percentage DESC, paid_tickets DESC
)
WHERE ROWNUM <= 3";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":movieId", movieId);
                        if (!string.IsNullOrEmpty(theaterId))
                        {
                            command.Parameters.AddWithValue(":theaterId", theaterId);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rows.Add(new MovieTheaterCityHallOccupancyRow
                                {
                                    MovieId = reader["movie_id"].ToString(),
                                    MovieName = reader["movie_name"].ToString(),
                                    TheaterId = reader["theater_id"].ToString(),
                                    TheaterName = reader["theater_name"].ToString(),
                                    TheaterCity = reader["theater_city"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    HallName = reader["hall_name"].ToString(),
                                    HallCapacity = Convert.ToInt32(reader["hall_capacity"]),
                                    ShowCount = Convert.ToInt32(reader["show_count"]),
                                    PaidTickets = Convert.ToInt32(reader["paid_tickets"]),
                                    OccupancyPercentage = Convert.ToDecimal(reader["occupancy_percentage"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving occupancy performer report: " + ex.Message);
            }

            return rows;
        }
    }
}
