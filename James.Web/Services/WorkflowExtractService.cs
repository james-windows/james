using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using James.Web.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace James.Web.Services
{
    public class WorkflowExtractService
    {
        private readonly string[] _windowsComponents = {"keyword", "api", "basic", "delay", "nodejs", "powershell", "python", "clipboard","largetype", "magic", "notification", "open", "searchbox"};
        private readonly string[] _osxComponents = { "keyword", "api", "basic", "delay", "nodejs", "python", "clipboard", "largetype", "magic", "notification", "open", "searchbox" };
        public bool ExtractWorkflow(IFormFile file, ref Workflow workflow, IHostingEnvironment hostingEnv)
        {
            string folderPath = hostingEnv.WebRootPath + $@"\workflows\{workflow.Id}";
            ExtractFileToFolder(file, folderPath);
            
            //check format
            if (!File.Exists($"{folderPath}\\config.json"))
            {
                Directory.Delete(folderPath, true);
                throw new FormatException("wrong zip format. config.json couldn't be found!");
            }
            File.Move($@"{folderPath}\upload.james", $@"{folderPath}\{workflow.Name}.james");
            workflow.Platform = GetPlatform(folderPath);
            return true;
        }

        /// <summary>
        /// Determines the platform of the workflow
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private Platform GetPlatform(string folderPath)
        {
            dynamic json = JToken.Parse(File.ReadAllText($"{folderPath}\\config.json"));
            var types = new List<string>();
            foreach (var component in json.components)
            {
                types.Add(component.type.ToString());
            }
            bool windowsReady = types.All(t => _windowsComponents.Any(win => win == t));
            bool osxReady = types.All(t => _osxComponents.Any(osx => osx == t));
            if (!windowsReady && !osxReady)
            {
                throw new FormatException("invalid config.js. Please check the types of your components!");
            }
            if (!windowsReady)
            {
                return Platform.OSX;
            }
            if (!osxReady)
            {
                return Platform.Windows;
            }
            return Platform.Both;
            
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
