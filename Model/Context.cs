using System.IO;
using Microsoft.EntityFrameworkCore;

namespace AudioScheduler.Model
{
    public sealed class Context : DbContext
    {
        public Context()
        {
            if (File.Exists(App.DatabaseFile)) return;
            App.InfoMessage("Startup Warning",
                $"Database file could not be found. It will be created at {App.DatabaseFile}. " +
                "The database file may have been deleted or misplaced or this is the first time using the program on this computer.");
            Database.EnsureCreated();
        }

        public DbSet<Sound> Sounds { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite("Data Source=" + App.DatabaseFile);
        }
    }
}