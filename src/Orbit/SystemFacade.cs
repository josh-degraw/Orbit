using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using Orbit.Models;
using Orbit.Data;
using Orbit.Util;

namespace Orbit
{
    public class SystemFacade
    {
        private IServiceProvider ServiceProvider => Orbit.Util.OrbitServiceProvider.Instance;
        private IServiceCollection serviceCollection;

        //Will be replaced with Unity Binding
        //public event PropertyChangedEventHandler? PropertyChanged;

        private WasteWaterStorageTankData? _wasteWater;
        private UrineSystemData? _urineSystemData;
        private WaterProcessorData? _waterProcessorData;

        public WasteWaterStorageTankData? WasteWater {
            get => this._wasteWater;

            set {

                // This stuff here is required to tell the UI to update anything that references this property
                this._wasteWater = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WasteWater)));
            }
        }

        public UrineSystemData? UrineTank {
            get => this._urineSystemData;

            set {
                this._urineSystemData = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UrineTank)));
            }
        }

        public WaterProcessorData? PotableWaterTank {
            get => this._waterProcessorData;

            set {
                this._waterProcessorData = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PotableWaterTank)));
            }
        }

        public UrineSystemData? UrineSystemStatus {
            get => this._urineSystemData;

            set {
                this._urineSystemData = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UrineSystemStatus)));
            }
        }

        public WaterProcessorData? WaterProcessorStatus {
            get => this._waterProcessorData;

            set {
                this._waterProcessorData = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WaterProcessorStatus)));
            }
        }

        public SystemFacade()
        {
              
        }

        public async void StartService()
        {
            DataGenerator.Instance.Start();   
            using var scope = ServiceProvider.CreateScope();
            var provider = scope.ServiceProvider;
            var db = provider.GetRequiredService<OrbitDbContext>();
            db.InsertSeedData();

            var comp = provider.GetService<IMonitoredComponent<WasteWaterStorageTankData>>();
            this.WasteWater = await comp.GetLatestReportAsync().ConfigureAwait(true);

            var urineComp = provider.GetService<IMonitoredComponent<UrineSystemData>>();
            this.UrineTank = await urineComp.GetLatestReportAsync().ConfigureAwait(true);

            var potableWaterComp = provider.GetService<IMonitoredComponent<WaterProcessorData>>();
            this.PotableWaterTank = await potableWaterComp.GetLatestReportAsync().ConfigureAwait(true);

            var urineStatusComp = provider.GetService<IMonitoredComponent<UrineSystemData>>();
            this.UrineSystemStatus = await urineStatusComp.GetLatestReportAsync().ConfigureAwait(true);

            var waterProcessorComp = provider.GetService<IMonitoredComponent<WaterProcessorData>>();
            this.WaterProcessorStatus = await waterProcessorComp.GetLatestReportAsync().ConfigureAwait(true);

        }
    }
}
