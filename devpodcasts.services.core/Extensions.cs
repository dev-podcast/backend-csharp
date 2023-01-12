using System;
using System.Collections.Generic;
using System.Linq;

namespace devpodcasts.Services.Core
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static void AddRange<T>(this ICollection<T> dest, IEnumerable<T> source)
        {
            var list = dest as List<T>;
            if (list.Any())
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    dest.Add(item);
                }
            }
        }
    }
}