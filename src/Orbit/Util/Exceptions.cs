using System;
using System.Globalization;
using System.Resources;

namespace Orbit.Util
{
    internal static class Exceptions
    {
        private static ResourceManager ResourceManager => OrbitServiceProvider.ResourceManager;

        // This is an example of how way we can design with multiple cultures/languages in mind
        public static Exception NoDataFound() => new InvalidOperationException(ResourceManager.GetString("ERR_NO_DATA_RETRIEVED", CultureInfo.CurrentCulture));
    }
}