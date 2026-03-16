namespace KumariCinema.Models
{
    public class Seat
    {
        public string SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }
        public string SeatTypeId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public SeatType SeatType { get; set; }
    }
}
