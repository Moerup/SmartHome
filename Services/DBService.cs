using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Services
{
    public class DBService
    {
        private readonly TempHumidContext _context;
        public DBService(TempHumidContext context)
        {
            _context = context;
        }

        public void CreateTempHumidEntry(TempHumidSensor tempHumidSensor)
        {
            _context.Add(new TempHumidSensor
            {
                Location = tempHumidSensor.Location,
                DateTime = DateTime.Now,
                Humidity = tempHumidSensor.Humidity,
                Temperature = tempHumidSensor.Temperature
            });
        }

        public List<TempHumidSensor> GetTempHumidReadings(DateTime? from = null, DateTime? to = null)
        {
            // Finds based on timeinterval
            if (from != null && to != null)
            {
                var tempHumidSensors1 = _context.TempHumidSensors
                    .Where(x => x.DateTime > from && x.DateTime < to)
                    .OrderByDescending(x => x.DateTime);

                return tempHumidSensors1.ToList();
            }

            // Finds based on from and to the newest
            if (from == null && to != null)
            {
                var tempHumidSensors2 = _context.TempHumidSensors
                    .Where(x => x.DateTime > from)
                    .OrderByDescending(x => x.DateTime);

                return tempHumidSensors2.ToList();
            }

            //Find all if no timeframe sent
            var tempHumidSensors = _context.TempHumidSensors.OrderByDescending(x => x.DateTime);

            return tempHumidSensors.ToList();
        }
    }
}
