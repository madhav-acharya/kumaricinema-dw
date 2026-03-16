using System;

namespace KumariCinema.Models
{
    public class MovieShow
    {
        public string ShowId { get; set; }
        public string MovieId { get; set; }
        public string HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ShowCategory { get; set; }
        public decimal BaseTicketPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LanguageId { get; set; }
        public string GenreId { get; set; }
        public Movie Movie { get; set; }
        public Hall Hall { get; set; }
        public Language Language { get; set; }
        public Genre Genre { get; set; }
    }
}
