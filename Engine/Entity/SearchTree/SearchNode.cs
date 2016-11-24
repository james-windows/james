using Engine.Collections;
using Engine.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Entity.SearchTree
{
    public class SearchNode
    {
        private SearchNode NextNode { get; set; }

        private SearchNode ChildNode { get; set; }

        public string Label { get; set; }

        public MergedResultList Results { get; set; } = new MergedResultList();

        public List<Item> ResultItems => Results.Items;

        public List<Item> Items { get; set; } = new List<Item>();

        public SearchNode Find(string search, int pos = 0)
        {
            TraversalOption nextStep = new TraversalOption(Label, search, pos);
            switch (nextStep.option)
            {
                case TraversalOptions.Split:
                    if (nextStep.commonPrefixLength < Label.Length)
                    {
                        return this;
                    }
                    return null;
                case TraversalOptions.Found:
                    return this;
                case TraversalOptions.MoveNext:
                    return NextNode?.Find(search, pos);
                case TraversalOptions.MoveDown:
                    return ChildNode?.Find(search, pos + nextStep.commonPrefixLength);
                default:
                    return null;
            }
        }

        public void Insert(string search, Item item, ref MergeResult result, int pos = 0)
        {
            TraversalOption nextStep = new TraversalOption(Label, search, pos);
            switch (nextStep.option)
            {
                case TraversalOptions.Found:
                    if (!Items.Any(item1 => item1.ComparePath(item)))
                    {
                        Items.Add(item);
                        Results.Add(item);
                        result.ToBeMergedInto = Results;
                        result.NeedsMerge = true;
                    }
                    break;
                case TraversalOptions.MoveNext:
                    if (NextNode == null)
                    {
                        NextNode = new SearchNode { Label = search.Substring(pos) };
                        NextNode.Items.Add(item);
                        NextNode.Results.Add(item);
                        result.ToBeMergedInto = NextNode.Results;
                        result.NeedsMerge = true;
                    }
                    else
                    {
                        NextNode.Insert(search, item, ref result, pos);
                    }
                    break;
                case TraversalOptions.MoveDown:
                    if (ChildNode == null)
                    {
                        ChildNode = new SearchNode()
                        {
                            Label = search.Substring(pos + nextStep.commonPrefixLength),
                            Items = new List<Item>(),

                        };
                        ChildNode.Items.Add(item);
                        ChildNode.Results.Add(item);
                        result.ToBeMergedInto = ChildNode.Results;
                        result.NeedsMerge = true;
                        if (result.NeedsMerge)
                        {
                            result.NeedsMerge = Results.MergeIntoList(result.ToBeMergedInto) > 0;
                            result.ToBeMergedInto = Results;
                        }
                    }
                    else
                    {
                        ChildNode.Insert(search, item, ref result, pos + nextStep.commonPrefixLength);
                        if (result.NeedsMerge)
                        {
                            result.NeedsMerge = Results.MergeIntoList(result.ToBeMergedInto) > 0;
                            result.ToBeMergedInto = Results;
                        }
                    }
                    break;
                case TraversalOptions.Split:
                    SearchNode newNode = new SearchNode
                    {
                        Label = Label.Substring(nextStep.commonPrefixLength),
                        Items = Items,
                        Results = Results,
                        ChildNode = ChildNode
                    };

                    Items = new List<Item>();
                    Results = new MergedResultList(newNode.ResultItems);
                    Label = Label.Substring(0, nextStep.commonPrefixLength);
                    ChildNode = newNode;
                    if (search.Length == nextStep.commonPrefixLength + pos)
                    {
                        Items.Add(item);
                        Results.Add(item);
                    }
                    else
                    {
                        SearchNode toInsert = new SearchNode
                        {
                            Label = search.Substring(nextStep.commonPrefixLength + pos),
                            Items = new List<Item> { item },
                            Results = new MergedResultList(new List<Item> { item })
                        };
                        Results.MergeIntoList(toInsert.Results);
                        newNode.NextNode = toInsert;
                    }

                    result.ToBeMergedInto = Results;
                    result.NeedsMerge = true;
                    break;
            }
        }

        public void Delete(string search, string path, int pos = 0, string debugging = "")
        {
            TraversalOption nextStep = new TraversalOption(Label, search, pos);
            switch (nextStep.option)
            {
                case TraversalOptions.MoveNext:
                    if (NextNode == null)
                    {
                        return;
                    }
                    NextNode.Delete(search, path, pos);
                    break;
                case TraversalOptions.MoveDown:
                    if (ChildNode != null)
                    {
                        ChildNode.Delete(search, path, pos + nextStep.commonPrefixLength, debugging + Label);
                        Results.Remove(item => item.ComparePath(path));
                        if (Results.Count == StaticVariables.ResultListLength - 1)
                        {
                            RefillResults();
                        }
                        if (Results.Count == 0 && NextNode != null)
                        {
                            Items = NextNode.Items;
                            Results = NextNode.Results;
                            Label = NextNode.Label;
                            ChildNode = NextNode.ChildNode;
                            NextNode = NextNode.NextNode;
                        }
                    }
                    break;
                case TraversalOptions.Found:
                    Items.Where(item => item.ComparePath(path)).ToList().ForEach(item => item.Delete());
                    Items.RemoveAll(item => item.ComparePath(path));
                    ResultItems.RemoveAll(item => item.ComparePath(path));
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Refills the ResultList by merging a new item from the childs
        /// </summary>
        private void RefillResults()
        {
            SearchNode currentNode = ChildNode;
            Item newItemToInsert = null;
            int currentPriority = -1;
            while (currentNode?.NextNode != null)
            {
                var tmp = currentNode.ResultItems.FirstOrDefault(item => !ResultItems.Contains(item));
                if (tmp != null && tmp.Priority > currentPriority)
                {
                    newItemToInsert = tmp;
                    currentPriority = tmp.Priority;
                }
                currentNode = currentNode.NextNode;
            }
            if (newItemToInsert != null)
            {
                Results.Add(newItemToInsert);
            }
        }

        public override string ToString() => $"Label:{Label}, Items:{Items.Count}, ResultItems:{ResultItems.Count}";
    }
}
