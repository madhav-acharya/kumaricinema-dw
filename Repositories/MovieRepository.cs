using Oracle.ManagedDataAccess.Client;
using KumariCinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KumariCinema.Repositories
{
    public class MovieRepository : IRepository<Movie>
    {
        private readonly string _connectionString;

        public MovieRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;
        }

        public List<Movie> GetAll()
        {
            var movies = new List<Movie>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT movie_id, name, duration_minutes, viewing_format FROM movie ORDER BY name";
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                movies.Add(new Movie
                                {
                                    MovieId = reader["movie_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    DurationMinutes = Convert.ToInt32(reader["duration_minutes"]),
                                    ViewingFormat = reader["viewing_format"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving movies: " + ex.Message);
            }
            return movies;
        }

        public Movie GetById(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT movie_id, name, duration_minutes, viewing_format FROM movie WHERE movie_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Movie
                                {
                                    MovieId = reader["movie_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    DurationMinutes = Convert.ToInt32(reader["duration_minutes"]),
                                    ViewingFormat = reader["viewing_format"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving movie: " + ex.Message);
            }
            return null;
        }

        public bool Insert(Movie entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO movie (movie_id, name, duration_minutes, viewing_format) VALUES (:id, :name, :duration, :format)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.MovieId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":duration", entity.DurationMinutes);
                        command.Parameters.AddWithValue(":format", entity.ViewingFormat);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting movie: " + ex.Message);
            }
        }

        public bool Update(Movie entity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE movie SET name = :name, duration_minutes = :duration, viewing_format = :format WHERE movie_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", entity.MovieId);
                        command.Parameters.AddWithValue(":name", entity.Name);
                        command.Parameters.AddWithValue(":duration", entity.DurationMinutes);
                        command.Parameters.AddWithValue(":format", entity.ViewingFormat);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating movie: " + ex.Message);
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM movie WHERE movie_id = :id";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting movie: " + ex.Message);
            }
        }

        public List<string> GetLanguagesByMovieId(string movieId)
        {
            var languages = new List<string>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT language_id FROM movie_language WHERE movie_id = :movieId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader["language_id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving movie languages: " + ex.Message);
            }
            return languages;
        }

        public bool AddLanguageToMovie(string movieId, string languageId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO movie_language (movie_id, language_id) VALUES (:movieId, :languageId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        command.Parameters.AddWithValue(":languageId", languageId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding language to movie: " + ex.Message);
            }
        }

        public bool RemoveLanguageFromMovie(string movieId, string languageId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM movie_language WHERE movie_id = :movieId AND language_id = :languageId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        command.Parameters.AddWithValue(":languageId", languageId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing language from movie: " + ex.Message);
            }
        }

        public List<string> GetGenresByMovieId(string movieId)
        {
            var genres = new List<string>();
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT genre_id FROM movie_genre WHERE movie_id = :movieId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                genres.Add(reader["genre_id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving movie genres: " + ex.Message);
            }
            return genres;
        }

        public bool AddGenreToMovie(string movieId, string genreId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO movie_genre (movie_id, genre_id) VALUES (:movieId, :genreId)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        command.Parameters.AddWithValue(":genreId", genreId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding genre to movie: " + ex.Message);
            }
        }

        public bool RemoveGenreFromMovie(string movieId, string genreId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM movie_genre WHERE movie_id = :movieId AND genre_id = :genreId";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":movieId", movieId);
                        command.Parameters.AddWithValue(":genreId", genreId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing genre from movie: " + ex.Message);
            }
        }
    }
}
