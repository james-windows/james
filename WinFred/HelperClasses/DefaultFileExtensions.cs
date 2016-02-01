using System.Collections.Generic;
using James.Search;

namespace James.HelperClasses
{
    public class DefaultFileExtensions
    {
        public static IEnumerable<FileExtension> GetDefault()
        {
            return new List<FileExtension>
            {
                //executables:
                new FileExtension("exe", 100),
                new FileExtension("msi", 80),
                new FileExtension("jar", 70),

                //images:
                new FileExtension("png", 20),
                new FileExtension("jpg", 20),
                new FileExtension("ico", 19),
                new FileExtension("gif", 19),
                new FileExtension("jpeg", 18),

                //videos
                new FileExtension("wav", 22),
                new FileExtension("wma", 22),
                new FileExtension("flv", 24),
                new FileExtension("mp4", 24),
                new FileExtension("mkv", 26),
                new FileExtension("mpg", 24),
                new FileExtension("wmv", 22),

                //documents:
                new FileExtension("pdf", 50),
                new FileExtension("doc", 44),
                new FileExtension("docx", 45),
                new FileExtension("csv", 39),
                new FileExtension("txt", 40),
                new FileExtension("rtf", 40),
                new FileExtension("psd", 40),
                new FileExtension("xls", 40),
                new FileExtension("xlsx", 41),
                new FileExtension("pps", 35),
                new FileExtension("ppt", 40),
                new FileExtension("pptx", 42),

                //coding:
                new FileExtension("c", 11),
                new FileExtension("cpp", 11),
                new FileExtension("html", 15),
                new FileExtension("js", 13),
                new FileExtension("cs", 13),
                new FileExtension("cshtml", 10),
                new FileExtension("java", 14),
                new FileExtension("sln", 60),
                new FileExtension("log", 8),
                new FileExtension("xml", 9),
                new FileExtension("h", 10),
                new FileExtension("sql", 12),
                new FileExtension("bat", 12),
                new FileExtension("css", 12),
                new FileExtension("php", 12),
                new FileExtension("svg", 12),
                new FileExtension("dll", 11),

                //archive:
                new FileExtension("zip", 51),
                new FileExtension("iso", 50),
                new FileExtension("7z", 49),
                new FileExtension("bin", 30),

                //other stuff:
                new FileExtension("xmcd", 39),
                new FileExtension("mcdx", 40),
                new FileExtension("rss", 10),
                new FileExtension("torrent", 30),
            };
        }
    }
}
