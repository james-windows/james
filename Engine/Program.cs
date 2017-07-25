using Engine.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class Program
    {
        static void Main()
        {
            SearchEngine engine = new SearchEngine();
            string path = "D:\\WinFred\\Project\\james\\Engine.Test\\bin\\Debug\\files.txt";
            var lines = File.ReadAllLines(path).Select(x => x.Split(';')).Select(x => new Tuple<string, int, string>(x[0], int.Parse(x[1]), x[0].Split('\\').Last()));
            Console.WriteLine($"total path count: {lines.Count()}\navg. length of a path: {(new FileInfo(path).Length - lines.Count()) / lines.Count()} chars");
            DateTime start = DateTime.Now;
            foreach (var line in lines)
            {
                engine.Insert(line.Item1, line.Item2);
            }
            Console.WriteLine($"Indexing {lines.Count()} paths took {(DateTime.Now - start).TotalMilliseconds:N}ms");
            Console.WriteLine($"current memory consumption of the index {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024.0:N}MB");
            start = DateTime.Now;
            int n = 100;
            for (int i = 0; i < n; i++)
            {
                foreach (var line in lines)
                {
                    engine.Find(line.Item3);
                }
            }
            Console.WriteLine($"Searching {lines.Count() * n:N} times took {(DateTime.Now - start).TotalMilliseconds:N}");
            Console.ReadLine();
        }
    }
}
