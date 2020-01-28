using System;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using Orbit.Util;

using UnityEngine;

public class Initialize: MonoBehaviour
{
    private IServiceProvider ServiceProvider => OrbitServiceProvider.Instance;

    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private void Start()
    {
        OrbitServiceProvider.OnRegisteringServices += this.ConfigureServices;

        ServiceProvider.GetRequiredService<IDataGenerator>().Start();
    }

    private void ConfigureServices(object sender, IServiceCollection services)
    {
        services.AddSingleton(SynchronizationContext.Current);
    }

}