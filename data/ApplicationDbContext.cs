using Microsoft.EntityFrameworkCore;
using RailwayReservationMVC.Models;

namespace RailwayReservationMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Quota> Quota { get; set; }  // ✅ Correctly named as "Quotas"
        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Passenger>()
            .HasKey(p => p.PassengerID);  

            modelBuilder.Entity<Train>()
                .Property(t => t.Fare)
                .HasColumnType("decimal(10,2)"); 

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalFare)
                .HasColumnType("decimal(10,2)");

 
            modelBuilder.Entity<Quota>()
                .HasOne(q => q.Train)
                .WithMany(t => t.Quotas) 
                .HasForeignKey(q => q.TrainID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Train)
                .WithMany(t => t.Reservations)  
                .HasForeignKey(r => r.TrainID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Quota)
                .WithMany(q => q.Reservations) 
                .HasForeignKey(r => r.QuotaID)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Reservation)
                .WithMany(r => r.Passengers)  
                .HasForeignKey(p => p.PNRNo)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
