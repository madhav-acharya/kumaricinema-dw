namespace KumariCinema.Models
{
    public class Booking
    {
        public string BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }
        public string ShowId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public AppUser User { get; set; }
        public MovieShow Show { get; set; }
    }
}
