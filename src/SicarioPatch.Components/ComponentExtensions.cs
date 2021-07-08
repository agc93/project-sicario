using System.Collections.Generic;

namespace SicarioPatch.Components
{
    public static class ComponentExtensions
    {
        internal static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1 && newIndex > -1 && newIndex < list.Count)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > list.Count) newIndex = list.Count;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }

        }
    }
}