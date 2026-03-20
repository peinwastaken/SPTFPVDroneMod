using System.Collections.Generic;

namespace FPVDroneModClient.Helpers
{
    public static class ListHelper
    {
        public static List<T> ToList<T>(this IEnumerable<T> enumerable)
        {
            List<T> list = [];

            foreach (T item in enumerable)
            {
                list.Add(item);
            }

            return list;
        }
    }
}
