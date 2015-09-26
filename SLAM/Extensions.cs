using System.Collections.Generic;

namespace SLAM
{
    public static class Extensions
    {
        public static void AddRangeLast<T>(this LinkedList<T> list, IEnumerable<T> elems)
        {
            foreach (var elem in elems)
                list.AddLast(elem);
        }
    }
}
