namespace TaskScheduler.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TokenValue { get; set; }
        public string Type { get; set; } // otp or email
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15);
    }
}
