using System.Collections.Generic;
using Engine.Entity.SearchTree;

namespace Engine.Helper
{
    public static class ExtensionMethods
    {
        public static TraversalOptions GetLongestCommonPrefix(this string label, string search, out int longestCommonPrefix, int pos = 0)
        {
            int i;
            for (i = 0; i < label.Length; i++)
            {
                if (pos + i >= search.Length || label[i] != search[pos + i])
                {
                    if (i == 0)
                    {
                        longestCommonPrefix = 0;
                        return TraversalOptions.MoveNext;
                    }
                    longestCommonPrefix = i;
                    return TraversalOptions.Split;
                }
            }
            longestCommonPrefix = label.Length;
            if (label.Length + pos == search.Length && search.EndsWith(label))
            {
                return TraversalOptions.Found;
            }
            return TraversalOptions.MoveDown;
        }

        /// <summary>
        /// Inserts an element to the list and returns the items position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int SortedInsert<T>(this List<T> items, T item)
        {
            int index = items.BinarySearch(item);
            if (index < 0)
            {
                index = ~index;
                items.Insert(index, item);
            }
            return index;
        }

        /// <summary>
        /// Merges second into the first list and returnes the count of changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int MergeIntoList<T>(this List<T> first, List<T> second)
        {
            foreach (var item in second)
            {
                first.SortedInsert(item);
            }
            return second.Count;
        }

        /// <summary>
        /// Removes duplicated items if priorities and paths are similar
        /// </summary>
        /// <param name="items"></param>
        public static void RemoveDuplicatedItems(this List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count && items[i].Priority == items[j].Priority; j++)
                {
                    if (items[i].CompareTo(items[j]) == 0)
                    {
                        items.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}
