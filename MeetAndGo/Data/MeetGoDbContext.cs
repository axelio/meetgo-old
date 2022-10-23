using System.Collections.Generic;
using MeetAndGo.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Data
{
    public class MeetGoDbContext : IdentityDbContext<User>
    {
        public MeetGoDbContext(DbContextOptions<MeetGoDbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<DeletedEntity> DeletedEntities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HistoricalVisit> HistoricalVisits { get; set; }
        public DbSet<HistoricalBooking> HistoricalBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Address>(entity =>
            {
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.Number).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Street).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Website).HasMaxLength(100);
            });

            builder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSDATETIMEOFFSET()");
                entity.Property(e => e.UserId).HasMaxLength(450);
                entity.Property(e => e.Code).HasMaxLength(4);

                entity
                    .HasOne(b => b.Visit)
                    .WithMany(v => v.Bookings)
                    .HasForeignKey(b => b.VisitId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasData(new List<Category>
                {
                    new() {Id = 1, Name = "Sport"},
                    new() {Id = 2, Name = "Art"},
                    new() {Id = 3, Name = "Fun"},
                    new() {Id = 4, Name = "Gastro"},
                    new() {Id = 5, Name = "Relax"},
                    new() {Id = 6, Name = "Tourism"},
                    new() {Id = 7, Name = "Kids"}
                });
            });

            builder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasData(new List<City>
                {
                    new() { Id = 1, Name = "Gdańsk" },
                    new() { Id = 2, Name = "Gdynia" },
                    new() { Id = 3, Name = "Sopot" },
                    new() { Id = 4, Name = "Kraków" }
                });
            });

            builder.Entity<CompanySettings>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.MaxDailyVisits).IsRequired();
            });

            builder.Entity<Event>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSDATETIMEOFFSET()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.PictureUrl).HasMaxLength(500);
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity
                    .HasOne(u => u.CompanySettings)
                    .WithOne(u => u.Company)
                    .HasForeignKey<CompanySettings>(cs => cs.UserId);

                entity
                    .HasMany(u => u.Bookings)
                    .WithOne(b => b.Customer)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PhoneNumber);
            });

            builder.Entity<Visit>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSDATETIMEOFFSET()");
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.IsBooked).IsConcurrencyToken();
            });

            builder.Entity<HistoricalVisit>(entity =>
            {
                entity.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            builder.Entity<HistoricalBooking>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            builder.Entity<DeletedEntity>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSDATETIMEOFFSET()");
                entity.Property(e => e.JsonEntity).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            });
        }
    }
}
