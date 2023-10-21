using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VS_Project.Algorithms;
using VS_Project.Singletone;

namespace VS_Project.Extentions
{
    public static class FileExtentions
    {
        private static void SaveFile(string folder, string name)
        {
            var folderPath = Path.GetFullPath("../../");
            var projPath = Path.Combine(folderPath, "VS_Project.csproj");

            var doc = XDocument.Load(projPath);

            var relativePath = Path.Combine("AI_Models", folder, name); // Constructs the relative path

            var itemGroup = new XElement("ItemGroup");
            var newFile = new XElement("Content", new XAttribute("Include", relativePath)); // Uses the relative path
            var copySetting = new XElement("CopyToOutputDirectory", "Always");

            newFile.Add(copySetting);
            itemGroup.Add(newFile);

            // Prevents the xmlns="" namespace from being added
            if (itemGroup.Attribute(XNamespace.Xmlns + "xmlns") != null)
            {
                itemGroup.Attribute(XNamespace.Xmlns + "xmlns").Remove();
            }

            doc.Root.Add(itemGroup);

            doc.Save(projPath);
        }


        public static void SaveFile(string folder, string fileName, string content)
        {
            Directory.CreateDirectory($"../../../Knowledge Base/{folder}");
            var folderPath = Path.GetFullPath($"../../../Knowledge Base/{folder}");
            var filePath = Path.Combine(folderPath, fileName);
            //SaveFile(folder, filePath);
            File.WriteAllText(filePath, content);
            Console.WriteLine($"Saved knowledgebase to path: {filePath}");
        }

        public static T LoadModel<T>(string name)
        {
            var folderPath = Path.GetDirectoryName($"../../../Knowledge Base/{nameof(T)}");
            var filePath = Path.Combine(folderPath, $"{name}.json");
            var jsonContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        public static T LoadModelDynamic<T>(string folder)
        {
            var folderPath = Path.GetFullPath($"../../../Knowledge Base/{folder}");
            Console.WriteLine($"Choose {folder} model name");
            int i = 0;
            foreach (var path in Directory.GetFiles(folderPath))
            {
                Console.WriteLine($"{++i}. {path}");
            }
            var filePath = Directory.GetFiles(folderPath)[int.Parse(Console.ReadLine()) - 1];
            var jsonContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }
    }
}
