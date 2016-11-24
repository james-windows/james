using System.Collections.Generic;
using System.Linq;

namespace Engine.Divider
{
    public class Divider : IDivider
    {
        public IEnumerable<string> SplitPath(string path)
        {
            string filename = path.Split('\\').Last();
            List<string> options = new List<string>(SplitBySpecialCase(filename));
            options.AddRange(SplitByNumber(filename));
            options.AddRange(SplitByUppercase(filename));
            options.Add(filename);

            return options.Select(s => s.Trim()).Where(s => s != "").Distinct().ToList();
        }

        private IEnumerable<string> SplitByUppercase(string path)
        {
            for (int i = 1; i < path.Length - 1; i++)
            {
                if (char.IsUpper(path[i]) && char.IsLower(path[i - 1]))
                {
                    yield return path.Substring(i);
                }
            }
        }

        private IEnumerable<string> SplitByNumber(string path)
        {
            for (int i = 1; i < path.Length - 1; i++)
            {
                if ((char.IsDigit(path[i]) && char.IsLetter(path[i - 1])) || (char.IsDigit(path[i - 1]) && char.IsLetter(path[i])))
                {
                    yield return path.Substring(i);
                }
            }
        }

        private static readonly List<char> _specialChars = new List<char>() { ' ', '.', ':', ',', '_', '-', ',', ';', '|', '(', ')', '+', '-', '&', '@' };

        private IEnumerable<string> SplitBySpecialCase(string path)
        {
            for (int i = 1; i < path.Length; i++)
            {
                if (_specialChars.Any(c => path[i - 1] == c))
                {
                    yield return path.Substring(i);
                }
            }
            int index = path.LastIndexOf('.');
            if (index != -1 && index != path.Length - 1)
            {
                yield return path.Substring(index);
            }
        }
    }
}
