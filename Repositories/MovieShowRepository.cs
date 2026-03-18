using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class MovieShowRepository : IRepository<MovieShow>
    {
        private readonly string _connectionString;

        public MovieShowRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<MovieShow> GetAll()
        {
            var shows = new List<MovieShow>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, created_at FROM movie_show ORDER BY start_time DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shows.Add(new MovieShow
                                {
                                    ShowId = reader["show_id"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    LanguageId = null,
                                    GenreId = null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving shows: " + ex.Message);
            }
            return shows;
        }

        public MovieShow GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, created_at FROM movie_show WHERE show_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new MovieShow
                                {
                                    ShowId = reader["show_id"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    LanguageId = null,
                                    GenreId = null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving show: " + ex.Message);
            }
            return null;
        }

        public bool Insert(MovieShow entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO movie_show (movie_id, hall_id, start_time, end_time, show_category, base_ticket_price) VALUES (:movieId, :hallId, :startTime, :endTime, :category, :price)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":movieId", entity.MovieId);
                        command.Parameters.AddWithValue(":hallId", entity.HallId);
                        command.Parameters.AddWithValue(":startTime", entity.StartTime);
                        command.Parameters.AddWithValue(":endTime", entity.EndTime);
                        command.Parameters.AddWithValue(":category", entity.ShowCategory);
                        command.Parameters.AddWithValue(":price", entity.BaseTicketPrice);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting show: " + ex.Message);
            }
        }

        public bool Update(MovieShow entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE movie_show SET movie_id = :movieId, hall_id = :hallId, start_time = :startTime, end_time = :endTime, show_category = :category, base_ticket_price = :price WHERE show_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":id", entity.ShowId);
                        command.Parameters.AddWithValue(":movieId", entity.MovieId);
                        command.Parameters.AddWithValue(":hallId", entity.HallId);
                        command.Parameters.AddWithValue(":startTime", entity.StartTime);
                        command.Parameters.AddWithValue(":endTime", entity.EndTime);
                        command.Parameters.AddWithValue(":category", entity.ShowCategory);
                        command.Parameters.AddWithValue(":price", entity.BaseTicketPrice);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating show: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM movie_show WHERE show_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting show: " + ex.Message);
            }
        }

        public List<MovieShow> GetByMovieId(string movieId)
        {
            var shows = new List<MovieShow>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, created_at FROM movie_show WHERE movie_id = :movieId ORDER BY start_time";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":movieId", movieId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shows.Add(new MovieShow
                                {
                                    ShowId = reader["show_id"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    LanguageId = null,
                                    GenreId = null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving shows by movie: " + ex.Message);
            }
            return shows;
        }

        public List<MovieShow> GetByHallId(string hallId)
        {
            var shows = new List<MovieShow>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, created_at FROM movie_show WHERE hall_id = :hallId ORDER BY start_time";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":hallId", hallId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shows.Add(new MovieShow
                                {
                                    ShowId = reader["show_id"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    LanguageId = null,
                                    GenreId = null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving shows by hall: " + ex.Message);
            }
            return shows;
        }

        public List<MovieShow> GetByTheaterId(string theaterId)
        {
            var shows = new List<MovieShow>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ms.show_id, ms.movie_id, ms.hall_id, ms.start_time, ms.end_time, ms.show_category, ms.base_ticket_price, ms.created_at FROM movie_show ms INNER JOIN hall h ON ms.hall_id = h.hall_id WHERE h.theater_id = :theaterId ORDER BY ms.start_time DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddWithValue(":theaterId", theaterId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shows.Add(new MovieShow
                                {
                                    ShowId = reader["show_id"].ToString(),
                                    MovieId = reader["movie_id"].ToString(),
                                    HallId = reader["hall_id"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["start_time"]),
                                    EndTime = Convert.ToDateTime(reader["end_time"]),
                                    ShowCategory = reader["show_category"].ToString(),
                                    BaseTicketPrice = Convert.ToDecimal(reader["base_ticket_price"]),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"]),
                                    LanguageId = null,
                                    GenreId = null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving shows by theater: " + ex.Message);
            }
            return shows;
        }
    }
}
