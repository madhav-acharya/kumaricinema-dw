namespace KumariCinema.Models
{
    public class Ticket
    {
        public string TicketId { get; set; }
        public string SeatId { get; set; }
        public string ShowId { get; set; }
        public decimal TicketPrice { get; set; }
        public string TicketStatus { get; set; }
        public Seat Seat { get; set; }
        public MovieShow Show { get; set; }
    }
}
