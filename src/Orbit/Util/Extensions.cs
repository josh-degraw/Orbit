using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
#if !NETSTANDARD_21
using Microsoft.EntityFrameworkCore.Extensions.Internal;
#endif

namespace Orbit.Util
{
    public static class Extensions
    {
#if !NETSTANDARD_21
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IQueryable<T> source) => source.AsAsyncEnumerable();
#endif
    }
}
