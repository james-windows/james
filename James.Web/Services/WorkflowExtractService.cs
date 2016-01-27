using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using James.Web.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace James.Web.Services
{
    public class WorkflowExtractService
    {
        public bool ExtractWorkflow(IFormFile file, Workflow workflow, IHostingEnvironment hostingEnv)
        {
            string folderPath = hostingEnv.WebRootPath + $@"\workflows\{workflow.Id}";
            ExtractFileToFolder(file, folderPath);
            
            //check format
            var directories = Directory.GetDirectories(folderPath);
            if (directories.Length == 1 && File.Exists($"{directories[0]}\\config.json"))
            {
                Directory.Move(directories[0], $"{folderPath}\\extracted");
            }
            else
            {
                Directory.Delete(folderPath, true);
                throw new FormatException("wrong zip format. config.json couldn't be found!");
            }
            File.Move($@"{folderPath}\upload.james", $@"{folderPath}\{workflow.Name}.james");
            return true;
        }

        private void ExtractFileToFolder(IFormFile file, string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            file.SaveAs($@"{folderPath}\upload.james");
            ZipFile.ExtractToDirectory($@"{folderPath}\upload.james", folderPath);
            RemoveMacOsX(folderPath);
        }

        private void RemoveMacOsX(string folder)
        {
            if (Directory.Exists(folder + "\\__MACOSX"))
            {
                Directory.Delete(folder + "\\__MACOSX", true);
            }
        }
    }
}
