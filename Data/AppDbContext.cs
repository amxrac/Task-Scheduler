using TaskScheduler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace TaskScheduler.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<Token> Tokens { get; set; }

    }
}
