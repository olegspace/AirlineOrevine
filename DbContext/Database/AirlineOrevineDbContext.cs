using System;
using DbContext.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DbContext.Database
{
    public class AirlineOrevineDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Passenger> Passengers { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Liner> Liners { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Crew> Crews { get; set; }

        public DbSet<Departure> Departures { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            bool isLocal = true;

            //Локальная Db
            if (isLocal)
            {                
                optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=AirlineOrevine;Username=postgres;Password=letmein2");
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            }
            else
            {
                //Удалённая Db
                optionsBuilder.UseNpgsql(
                    "Host=176.57.215.91;" +
                    "Port=5432;" +
                    "Database=airlineorevinedb;" +
                    "Username=test;" +  
                    "Password=P1234esdfkljlk234_fsdsd21!;" +
                    "Pooling = false;" +
                    "CommandTimeout = 300;" +
                    "Timeout = 300;" +
                    "Connection Idle Lifetime = 300;" +
                    "Tcp Keepalive = true;");
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            }
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>()
                .HasMany(x => x.WayPoints)
                .WithMany(x => x.Routes)
                .UsingEntity(j => j.ToTable("WayPoints"));

            modelBuilder.Entity<Route>()
                .HasOne(x => x.StartingPoint)
                .WithMany();

            modelBuilder.Entity<Route>()
                .HasOne(x => x.EndingPoint)
                .WithMany();

            modelBuilder.Entity<Crew>()
                .HasMany(x => x.Employees)
                .WithMany(x => x.Crews)
                .UsingEntity(j => j.ToTable("FlightCrew"));

            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.Departure)
                .WithMany();

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcValueConverter>();
            configurationBuilder.Properties<DateTime?>().HaveConversion<UtcValueConverter>();
            base.ConfigureConventions(configurationBuilder);
        }

        private class UtcValueConverter : ValueConverter<DateTime, DateTime>
        {
            public UtcValueConverter()
                : base(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            {
            }
        }
    }
}