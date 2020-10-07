using System;
using System.Threading.Tasks;
using System.Device.I2c;
using Iot.Device.GrovePiDevice.Models;
using Iot.Device.GrovePiDevice;
using Iot.Device.GrovePiDevice.Sensors;
using Iot.Device.CharacterLcd;

namespace SmartHome.Services
{
    public class GrovePiService
    {
        public GrovePi GrovePi { get; set; }


        public Led KitchenLight { get; set; }

        public Led LivingRoomLight { get; set; }

        public Led BathroomLight { get; set; }


        public DhtSensor TempHumid { get; set; }

        public string Temperature { get; set; }

        public string Humidity { get; set; }


        public LcdRgb1602 Display { get; set; }

        public GrovePiService()
        {
            I2cConnectionSettings i2CConnectionSettings = new I2cConnectionSettings(1, GrovePi.DefaultI2cAddress);
            GrovePi = new GrovePi(I2cDevice.Create(i2CConnectionSettings));

            // Lights
            KitchenLight = new Led(GrovePi, GrovePort.DigitalPin2);
            LivingRoomLight = new Led(GrovePi, GrovePort.DigitalPin3);
            BathroomLight = new Led(GrovePi, GrovePort.DigitalPin4);

            // Sensors
            TempHumid = new DhtSensor(GrovePi, GrovePort.DigitalPin8, DhtType.Dht11);

            // Display
            var i2cLcdDevice = I2cDevice.Create(new I2cConnectionSettings(busId: 1, deviceAddress: 0x3E));
            var i2cRgbDevice = I2cDevice.Create(new I2cConnectionSettings(busId: 1, deviceAddress: 0x62));
            Display = new LcdRgb1602(i2cLcdDevice, i2cRgbDevice);
        }

        public async Task ReadDHTValues()
        {
            Console.WriteLine("Getting NonNaN values from DHT sensor:");
            TempHumid.Read();
            while (Double.IsNaN(TempHumid.LastRelativeHumidity) && Double.IsNaN(TempHumid.LastTemperature))
            {
                TempHumid.Read();
                Console.WriteLine($"Humidity: {TempHumid.LastRelativeHumidity}");
                Console.WriteLine($"Temperature: {TempHumid.LastTemperature}");

            }
            Temperature = TempHumid.LastTemperature.ToString();
            Humidity = TempHumid.LastRelativeHumidity.ToString();
        }
    }
}
