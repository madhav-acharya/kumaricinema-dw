namespace KumariCinema.Models
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string BookingId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public Booking Booking { get; set; }
    }
}
