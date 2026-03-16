namespace KumariCinema.Models
{
    public class SeatType
    {
        public string SeatTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceMultiplier { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
