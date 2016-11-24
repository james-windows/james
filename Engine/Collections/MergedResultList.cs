using System;
using System.Collections.Generic;
using Engine.Entity.SearchTree;
using Engine.Helper;

namespace Engine.Collections
{
    public class MergedResultList
    {
        public int Count => Items.Count;

        public MergedResultList(IEnumerable<Item> items)
        {
            Items = new List<Item>(items);
        }

        public MergedResultList()
        {
            Items = new List<Item>();
        }

        public List<Item> Items { get; }

        public int Add(Item item)
        {
            int index = Items.SortedInsert(item);
            if (Items.Count > StaticVariables.ResultListLength)
            {
                Items.RemoveAt(StaticVariables.ResultListLength);
            }
            return index;
        }

        public int MergeIntoList(MergedResultList items)
        {
            int result = Items.MergeIntoList(items.Items);
            Items.RemoveDuplicatedItems();
            if (Items.Count > StaticVariables.ResultListLength)
            {
                Items.RemoveRange(StaticVariables.ResultListLength, Items.Count - StaticVariables.ResultListLength);
            }
            return result;
        }

        public bool Remove(Predicate<Item> predicate)
        {
            return Items.RemoveAll(predicate) > 0;
        }
    }
}
