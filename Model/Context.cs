using System.IO;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler.Model
{
    public class Context : DbContext
    {
        public Context()
        {
            if (File.Exists(App.DatabaseFile)) return;
            App.InfoMessage("Startup Warning",
                $"Database file ({App.DatabaseFile}) could not be found. It will be created in the same directory as this executable. The database file may have been deleted or misplaced.");
            Database.EnsureCreated();
        }

        public DbSet<Sound> Sounds { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Day> Days { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite("Data Source=" + App.DatabaseFile);
        }
    }
}