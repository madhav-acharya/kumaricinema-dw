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
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, language_id, genre_id FROM movie_show ORDER BY start_time DESC";
                    using (var command = new OracleCommand(query, connection))
                    {
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
                                    LanguageId = reader["language_id"] != DBNull.Value ? reader["language_id"].ToString() : null,
                                    GenreId = reader["genre_id"] != DBNull.Value ? reader["genre_id"].ToString() : null
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
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, language_id, genre_id FROM movie_show WHERE show_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
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
                                    LanguageId = reader["language_id"] != DBNull.Value ? reader["language_id"].ToString() : null,
                                    GenreId = reader["genre_id"] != DBNull.Value ? reader["genre_id"].ToString() : null
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
                    string query = "INSERT INTO movie_show (show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, language_id, genre_id) VALUES (:id, :movieId, :hallId, :startTime, :endTime, :category, :price, :languageId, :genreId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.ShowId);
                        command.Parameters.AddWithValue(":movieId", entity.MovieId);
                        command.Parameters.AddWithValue(":hallId", entity.HallId);
                        command.Parameters.AddWithValue(":startTime", entity.StartTime);
                        command.Parameters.AddWithValue(":endTime", entity.EndTime);
                        command.Parameters.AddWithValue(":category", entity.ShowCategory);
                        command.Parameters.AddWithValue(":price", entity.BaseTicketPrice);
                        command.Parameters.AddWithValue(":languageId", entity.LanguageId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue(":genreId", entity.GenreId ?? (object)DBNull.Value);
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
                    string query = "UPDATE movie_show SET movie_id = :movieId, hall_id = :hallId, start_time = :startTime, end_time = :endTime, show_category = :category, base_ticket_price = :price, language_id = :languageId, genre_id = :genreId WHERE show_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.ShowId);
                        command.Parameters.AddWithValue(":movieId", entity.MovieId);
                        command.Parameters.AddWithValue(":hallId", entity.HallId);
                        command.Parameters.AddWithValue(":startTime", entity.StartTime);
                        command.Parameters.AddWithValue(":endTime", entity.EndTime);
                        command.Parameters.AddWithValue(":category", entity.ShowCategory);
                        command.Parameters.AddWithValue(":price", entity.BaseTicketPrice);
                        command.Parameters.AddWithValue(":languageId", entity.LanguageId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue(":genreId", entity.GenreId ?? (object)DBNull.Value);
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
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, language_id, genre_id FROM movie_show WHERE movie_id = :movieId ORDER BY start_time";
                    using (var command = new OracleCommand(query, connection))
                    {
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
                                    LanguageId = reader["language_id"] != DBNull.Value ? reader["language_id"].ToString() : null,
                                    GenreId = reader["genre_id"] != DBNull.Value ? reader["genre_id"].ToString() : null
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
                    string query = "SELECT show_id, movie_id, hall_id, start_time, end_time, show_category, base_ticket_price, language_id, genre_id FROM movie_show WHERE hall_id = :hallId ORDER BY start_time";
                    using (var command = new OracleCommand(query, connection))
                    {
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
                                    LanguageId = reader["language_id"] != DBNull.Value ? reader["language_id"].ToString() : null,
                                    GenreId = reader["genre_id"] != DBNull.Value ? reader["genre_id"].ToString() : null
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
    }
}
