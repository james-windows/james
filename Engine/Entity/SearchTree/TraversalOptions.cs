using Engine.Helper;

namespace Engine.Entity.SearchTree
{
    public struct TraversalOption
    {
        public readonly int commonPrefixLength;
        public TraversalOptions option;
        public TraversalOption(string label, string search, int pos)
        {
            option = label.GetLongestCommonPrefix(search, out commonPrefixLength, pos);
        }

        public override string ToString()
        {
            return option.ToString();
        }
    }

    public enum TraversalOptions
    {
        MoveNext,
        MoveDown,
        Split,
        Found
    }
}
