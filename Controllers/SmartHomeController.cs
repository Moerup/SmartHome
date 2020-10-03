using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Iot.Device.GrovePiDevice.Sensors;
using SmartHome.Services;
using SmartHome.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SmartHomeController : ControllerBase
    {
        private static GrovePiService _grovePi;

        private static TempHumidContext _context;

        public SmartHomeController(GrovePiService grovePiService, TempHumidContext context)
        {
            _grovePi = grovePiService;
            _context = context;
        }

        [HttpGet]
        [Route("lights/kitchenlight")]
        public string ToggleKitchenLight()
        {
            return ToggleLight(_grovePi.KitchenLight, "Kitchen");
        }

        [HttpGet]
        [Route("lights/livingroom")]
        public string ToggleLivingRoomLight()
        {
            return ToggleLight(_grovePi.LivingRoomLight, "Living Room");
        }

        [HttpGet]
        [Route("lights/bathroom")]
        public string ToggleBathroomLight()
        {
            return ToggleLight(_grovePi.BathroomLight, "Bathroom");
        }

        [HttpGet]
        [Route("sensors/temphumid/pi")]
        public async Task<string> GetPiTemperatureHumidSensor()
        {
            await _grovePi.ReadDHTValues();
            var humidity = _grovePi.TempHumid.LastRelativeHumidity.ToString();
            var temperature = _grovePi.TempHumid.LastTemperature.ToString();

            _grovePi.Display.Clear();
            _grovePi.Display.Write($"Temp: {temperature}C");
            _grovePi.Display.SetCursorPosition(0, 1);
            _grovePi.Display.Write($"Humid: {humidity}%");

            return $"Temp: {temperature}C  Humid: {humidity}%";
        }

        [HttpPost]
        [Route("sensors/temphumid")]
        public async Task<string> PostTemperatureHumidSensor(TempHumidSensor tempHumidSensor)
        {
            Console.WriteLine("POST Request - TempHumid reading");

            try
            {
                Console.WriteLine("Creating TempHumid reading");

                _context.Add(new TempHumidSensor
                {
                    Location = tempHumidSensor.Location,
                    DateTime = DateTime.Now,
                    Humidity = tempHumidSensor.Humidity,
                    Temperature = tempHumidSensor.Temperature
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Succes!");
            return "Succesfully created new temphumid reading";
        }

        [HttpGet]
        [Route("sensors/temphumid")]
        public async Task<List<TempHumidSensor>> GetAllTemperatureHumidSensorReadings()
        {
            Console.WriteLine("GET Request - TempHumid readings");

            var readings = new List<TempHumidSensor>();

            try
            {
                Console.WriteLine("Getting all TempHumid readings");

                readings = await _context.TempHumidSensors.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Succes!");
            return readings;
        }

        private string ToggleLight(Led led, string name)
        {
            if (led.Value == false)
            {
                led.Value = true;
                _grovePi.Display.Clear();
                _grovePi.Display.Write(name);
                _grovePi.Display.SetCursorPosition(0, 1);
                _grovePi.Display.Write("Enabled");
                return $"{nameof(led)} are now enabled";
            }
            if (led.Value == true)
            {
                led.Value = false;
                _grovePi.Display.Clear();
                _grovePi.Display.Write(name);
                _grovePi.Display.SetCursorPosition(0, 1);
                _grovePi.Display.Write("Disabled");
                return $"{nameof(led)} are now disabled";
            }

            _grovePi.Display.Clear();
            _grovePi.Display.Write("Error - LED");
            _grovePi.Display.SetCursorPosition(0, 1);
            _grovePi.Display.Write(name);
            return "Couldn't reach LED";
        }
    }
}
