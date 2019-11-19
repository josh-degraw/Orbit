using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Orbit.Util;
using Orbit.Data;

namespace Orbit.Models
{
    class InternalCoolantController
    {
        /// <summary>
        /// desired cabin temperature, changeable by crew
        /// </summary>
        public double CabinSetTemp { get; set; }

        /// <summary>
        /// current sensor read cabin temperature
        /// </summary>
        public double CabinActualTemp { get; private set; }
        public string PumpOnOff { get; set; }

        /// <summary>
        /// pressure of coolant in the system
        /// </summary>
        public int FluidPressure { get; private set; }

        /// <summary>
        /// temp of coolant returning from internal/external heat exchanger
        /// </summary>
        public int InflowFluidTemp { get; private set; }

        /// <summary>
        /// temp of coolant from cabin going to internal/external heat exchanger
        /// </summary>
        public int OutflowFluidTemp { get; private set; }

        /// <summary>
        /// emulate sensors and firmware controller
        /// generate sensor data (rand in range), change hardware state if indicated (TODO), upload 'momentary' state to database
        /// </summary>
        public void GenerateData()
        {

            Random random = new Random();
            {
                double temp = random.Next(210, 289);
                CabinActualTemp = temp/10 + temp%10;
                FluidPressure = random.Next(460, 512) / 10;
                InflowFluidTemp = random.Next(120, 150);
                OutflowFluidTemp = random.Next(180, 200);
            }
            
            IServiceProvider provider = OrbitServiceProvider.Instance;

            using (var db = provider.GetService<OrbitDbContext>())
            {
                db.Add(new InternalCoolantLoop()
                {
                    ID = new Guid(),
                    DateTime = DateTimeOffset.Now,
                    PumpStatus = "On",
                    Pressure = FluidPressure,
                    OutgoingFluidTemp = OutflowFluidTemp,
                    ReturnFluidTemp = InflowFluidTemp,
                    ReserveTankLevel = 70
                });

                db.SaveChanges();
            }
        }
    }
}
