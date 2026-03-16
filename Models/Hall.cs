namespace KumariCinema.Models
{
    public class Hall
    {
        public string HallId { get; set; }
        public string HallName { get; set; }
        public int Capacity { get; set; }
        public string ScreenType { get; set; }
        public string TheaterId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Theater Theater { get; set; }
    }
}
