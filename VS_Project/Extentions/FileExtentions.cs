using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            var folderPath = Path.GetFullPath($"../../AI_Models/{folder}");
            var filePath = Path.Combine(folderPath, fileName);
            SaveFile(folder, filePath);
            File.WriteAllText(filePath, content);
        }
    }
}
