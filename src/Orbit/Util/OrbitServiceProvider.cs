using Microsoft.Extensions.DependencyInjection;

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
            this._provider = Build();
        }

        private static readonly Lazy<OrbitServiceProvider> _instance = new Lazy<OrbitServiceProvider>(() => new OrbitServiceProvider());

        public static IServiceProvider Instance => _instance.Value;

        private IServiceProvider Build()
        {
            var services = new ServiceCollection();
            RegisterServices(services);
            return services.BuildServiceProvider();
        }

        private void RegisterServices(IServiceCollection services)
        {
            foreach (var prov in Assembly.GetExecutingAssembly().ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(IValueProvider))))
            {
                services.AddScoped(prov);
            }
        }

        object IServiceProvider.GetService(Type serviceType) => _provider.GetService(serviceType);
    }
}