using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;

namespace SmartHome.Models
{
    public class TempHumidContext : DbContext
    {
        public TempHumidContext(DbContextOptions<TempHumidContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite("Data Source=temphumid.db");

        public DbSet<TempHumidSensor> TempHumidSensors { get; set; }
    }

    public class TempHumidSensor
    {
        [JsonIgnore]
        public int Id { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public string Location { get; set; }

        [JsonIgnore]
        public DateTime DateTime { get; set; }
    }
}
