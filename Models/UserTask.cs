namespace TaskScheduler.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TaskName { get; set; }
        public DateTime ExecutionTime  { get; set; }
        public string Status { get; set; } //pending, running, completed, cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AppUser User { get; set; }
    }
}
