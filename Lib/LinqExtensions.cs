using System;
using System.Collections.Generic;

namespace Lib
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }
}
