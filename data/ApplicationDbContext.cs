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
            .HasKey(p => p.PassengerID);  // ✅ Explicitly defining the PK

            // ✅ Define precision for Fare in Train table
            modelBuilder.Entity<Train>()
                .Property(t => t.Fare)
                .HasColumnType("decimal(10,2)"); // Precision: 10 digits, 2 decimal places

            // ✅ Define precision for TotalFare in Reservation table
            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalFare)
                .HasColumnType("decimal(10,2)"); // Prevents truncation issues

            // ✅ Define Quota relationship with Train
            modelBuilder.Entity<Quota>()
                .HasOne(q => q.Train)
                .WithMany(t => t.Quotas)  // ✅ Train can have multiple quotas
                .HasForeignKey(q => q.TrainID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Define Reservation relationship with Train
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Train)
                .WithMany(t => t.Reservations)  // ✅ Train can have multiple reservations
                .HasForeignKey(r => r.TrainID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Define Reservation relationship with Quota
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Quota)
                .WithMany(q => q.Reservations)  // ✅ Quota can be used in multiple reservations
                .HasForeignKey(r => r.QuotaID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Define Passenger relationship with Reservation
            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Reservation)
                .WithMany(r => r.Passengers)  // ✅ Reservation can have multiple passengers
                .HasForeignKey(p => p.PNRNo)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
