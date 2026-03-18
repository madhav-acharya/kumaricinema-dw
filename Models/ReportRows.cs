namespace KumariCinema.Models
{
    public class UserTicketReportRow
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string BookingId { get; set; }
        public string TicketId { get; set; }
        public string SeatNumber { get; set; }
        public string MovieName { get; set; }
        public string HallName { get; set; }
        public string TheaterName { get; set; }
        public string TheaterCity { get; set; }
        public System.DateTime ShowTime { get; set; }
        public decimal TicketPrice { get; set; }
        public string PaymentStatus { get; set; }
        public System.DateTime BookingDate { get; set; }
    }

    public class TheaterCityHallMovieRow
    {
        public string TheaterId { get; set; }
        public string TheaterName { get; set; }
        public string TheaterCity { get; set; }
        public string HallId { get; set; }
        public string HallName { get; set; }
        public string MovieId { get; set; }
        public string MovieName { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string ShowCategory { get; set; }
        public decimal BaseTicketPrice { get; set; }
    }

    public class MovieTheaterCityHallOccupancyRow
    {
        public string MovieId { get; set; }
        public string MovieName { get; set; }
        public string TheaterId { get; set; }
        public string TheaterName { get; set; }
        public string TheaterCity { get; set; }
        public string HallId { get; set; }
        public string HallName { get; set; }
        public int HallCapacity { get; set; }
        public int ShowCount { get; set; }
        public int PaidTickets { get; set; }
        public decimal OccupancyPercentage { get; set; }
    }
}
