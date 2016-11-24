using Engine.Collections;

namespace Engine.Entity.SearchTree
{
    public class MergeResult
    {
        public bool NeedsMerge { get; set; }

        public MergedResultList ToBeMergedInto { get; set; }
    }
}
