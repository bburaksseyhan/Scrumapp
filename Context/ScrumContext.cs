using Microsoft.EntityFrameworkCore;
using scrum_ui.Models;

namespace scrum_ui.Context
{
    public class ScrumContext : DbContext
    {
        public ScrumContext(DbContextOptions options) : base(options)
        {

            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Participants> Participants { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<SprintLog> SprintLogs { get; set; }
    }
}