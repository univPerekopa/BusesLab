using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusesWebApplication
{
    public partial class DBBusesContext : DbContext
    {
        public DBBusesContext()
        {
        }

        public DBBusesContext(DbContextOptions<DBBusesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BusModel> BusModels { get; set; } = null!;
        public virtual DbSet<BusStatus> BusStatuses { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Driver> Drivers { get; set; } = null!;
        public virtual DbSet<DriversCategory> DriversCategories { get; set; } = null!;
        public virtual DbSet<Route> Routes { get; set; } = null!;
        public virtual DbSet<RoutesStation> RoutesStations { get; set; } = null!;
        public virtual DbSet<Station> Stations { get; set; } = null!;
        public virtual DbSet<Timetable> Timetables { get; set; } = null!;
        public virtual DbSet<TripStatus> TripStatuses { get; set; } = null!;
        public virtual DbSet<Bus> Buses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-GKQOHQ5;\nDatabase=DBBuses; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusModel>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<BusStatus>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Cities_Countries");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.FullName).HasMaxLength(100);
            });

            modelBuilder.Entity<DriversCategory>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.DriversCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_DriversCategories_Categories");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.DriversCategories)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_DriversCategories_Drivers");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<RoutesStation>(entity =>
            {
                entity.HasOne(d => d.Route)
                    .WithMany(p => p.RoutesStations)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_RoutesStations_Routes");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.RoutesStations)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_RoutesStations_Stations");
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Stations)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Stations_Cities");
            });

            modelBuilder.Entity<Timetable>(entity =>
            {
                entity.ToTable("Timetable");

                entity.Property(e => e.BusId).HasMaxLength(20);

                entity.HasOne(d => d.Bus)
                    .WithMany(p => p.Timetables)
                    .HasForeignKey(d => d.BusId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Timetable_Buses");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Timetables)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Timetable_Drivers");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Timetables)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Timetable_Routes");

                entity.HasOne(d => d.TripStatus)
                    .WithMany(p => p.Timetables)
                    .HasForeignKey(d => d.TripStatusId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Timetable_TripStatuses");
            });

            modelBuilder.Entity<TripStatus>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Bus>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(20);

                entity.HasOne(d => d.CategoryNeededNavigation)
                    .WithMany(p => p.Buses)
                    .HasForeignKey(d => d.CategoryNeeded)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Buses_Categories");

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Buses)
                    .HasForeignKey(d => d.ModelId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Buses_BusModels");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Buses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_Buses_BusStatuses");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
