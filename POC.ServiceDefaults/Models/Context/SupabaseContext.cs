using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using POC.ServiceDefaults.Models.Tables;
using Supabase.Postgrest.Models;
using System.Reflection;

namespace POC.ServiceDefaults.Models.Context
{
    public class SupabaseContext : DbContext
    {
        public SupabaseContext(DbContextOptions<SupabaseContext> options) : base(options) { }

        public DbSet<DeviceDTO> Devices => Set<DeviceDTO>();
        public DbSet<SensorDTO> Sensors => Set<SensorDTO>();
        public DbSet<MeasurementDTO> Measurements => Set<MeasurementDTO>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Writer/Reader for WTK Geography Points: POINT(x y)
            var wktReader = new WKTReader();
            var wktWriter = new WKTWriter();
            // Register DTOs
            modelBuilder.Entity<DeviceDTO>(entity =>
            {
                //entity.ToTable("devices");
                entity.HasKey(d => d.DeviceID);
                entity.Property(d => d.DeviceID).ValueGeneratedOnAdd();
                entity.HasMany(d => d.Sensors);
                // Setup for GPS Point conversion
                entity.Property(d => d.DeviceGPS)
                    .HasConversion(
                        // From string -> Point (Going to DB)
                        v => string.IsNullOrEmpty(v) ? new Point(0,0) : (Point)wktReader.Read(v),
                        // From Point -> string (From DB)
                        v => v == null ? "POINT(0 0)" : wktWriter.Write(v)
                    ).HasColumnType("geography");
            });

            modelBuilder.Entity<SensorDTO>(entity =>
            {
                entity.HasKey(s => s.SensorID);
                entity.Property(s => s.SensorID).ValueGeneratedOnAdd();
                entity.HasOne(s => s.Device);
                //entity.HasMany(s => s.Measurements);
            });

            modelBuilder.Entity<MeasurementDTO>(entity =>
            {
                entity.HasKey(m => m.MeasurementID);
                entity.Property(m => m.MeasurementID).ValueGeneratedOnAdd();
                entity.HasOne(m => m.Sensor);
            });


            // Automatically ignore base model properties on all entities that inherit Supabase BaseModel
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    var baseProps = typeof(BaseModel).GetProperties();
                    foreach (var baseProp in baseProps)
                    {
                        modelBuilder.Entity(entityType.ClrType).Ignore(baseProp.Name);
                    }
                }
            }
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
