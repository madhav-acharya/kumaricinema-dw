namespace KumariCinema.Models
{
    public class AppUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string TheaterId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Theater Theater { get; set; }
    }
}
