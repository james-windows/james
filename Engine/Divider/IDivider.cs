using System.Collections.Generic;

namespace Engine.Divider
{
    public interface IDivider
    {
        IEnumerable<string> SplitPath(string path);
    }
}
