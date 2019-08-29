using System;
using System.Collections.Generic;

namespace CsvEditor
{
    static class Extends
    {
        public static T GetAt<T>(this List<T> list, int index)
        {
            if (list == null) return default(T);
            if (index >= list.Count) return default(T);
            return list[index];
        }
    }
}
