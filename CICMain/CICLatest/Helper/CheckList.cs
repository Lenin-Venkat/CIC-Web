using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CICLatest.Helper
{
    public static class CheckList
    {
        public static bool AnyOrNotNull<T>(this IEnumerable<T> source)
        {
            if (source.Any())
                return true;
            else
                return false;

        }
    }
}
