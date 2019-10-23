using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Orbit.Util
{
    public class OrbitServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _provider;

        private OrbitServiceProvider()
        {
            this._provider = this.Build();
        }

        private static readonly Lazy<OrbitServiceProvider> _instance = new Lazy<OrbitServiceProvider>(() => new OrbitServiceProvider());

        public static IServiceProvider Instance => _instance.Value;

        private IServiceProvider Build()
        {
            var services = new ServiceCollection();
            this.RegisterServices(services);
            ServiceProvider prov = services.BuildServiceProvider();
            EventMonitor.Instance.Start();
            return prov;
        }

        private void AddValueProviders(IServiceCollection services)
        {
            var providerTypes = Assembly.GetExecutingAssembly().ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(IMonitoredComponent)));
            foreach (Type prov in providerTypes)
            {
                services.AddScoped(prov);
                EventMonitor.Instance.Register(prov);
            }
        }

        private void RegisterServices(IServiceCollection services)
        {
            this.AddValueProviders(services);
            services.AddSingleton(_ => EventMonitor.Instance);

            //TODO: Replace the following with actual database implementation when ready
            services.AddDbContext<OrbitDbContext>(o => o.UseInMemoryDatabase("OrbitDb"));
        }

        object? IServiceProvider.GetService(Type serviceType) => this._provider.GetService(serviceType);
    }
}