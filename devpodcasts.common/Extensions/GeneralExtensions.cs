using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devpodcasts.common.Extensions
{
    public static class GeneralExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            ArgumentNullException.ThrowIfNull(destination, nameof(destination));
            var list = destination as List<T>;
            if (list.Any())
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    destination.Add(item);
                }
            }
        }

        public static bool In<T>(this T source, params T[] events)
        {
            if (source == null) return false;
            return events.Any(x => x.Equals(source));
        }
    }
}
