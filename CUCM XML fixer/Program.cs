using System;
using System.Linq;
using System.IO;
using System.Text;

namespace CUCM_XML_fixer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the folders to exclude
            string[] excludedFolders = { "ccm", "Common", "Projects", "src", "vos", "WebContent" };

            // Get the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Get all XML files in the current directory and subdirectories, excluding specified folders
            var xmlFiles = Directory.GetFiles(currentDirectory, "*.xml", SearchOption.AllDirectories)
                                    .Where(file => !excludedFolders.Any(folder => file.Contains(Path.DirectorySeparatorChar + folder + Path.DirectorySeparatorChar)))
                                    .ToList();

            foreach (var file in xmlFiles)
            {
                CheckAndFixXmlFile(file);
            }

            Console.WriteLine("Processing complete! ✅");
            Console.ReadLine();
        }

        static void CheckAndFixXmlFile(string filePath)
        {
            // Use UTF-8 encoding with BOM
            Encoding utf8WithBom = new UTF8Encoding(true);
            string[] lines;

            // Read the file using StreamReader with UTF-8 encoding with BOM
            using (StreamReader reader = new StreamReader(filePath, utf8WithBom))
            {
                lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }

            if (lines.Length > 0 && lines[0].StartsWith("<?xml"))
            {
                // Check if the first line contains both the XML declaration and the <phrases> element
                if (lines[0].Contains("<?xml") && lines[0].Contains("<phrases"))
                {
                    // Split the line into XML declaration and <phrases> element
                    string xmlDeclaration = lines[0].Substring(0, lines[0].IndexOf("?>") + 2);
                    string phrasesElement = lines[0].Substring(lines[0].IndexOf("?>") + 2).Trim();

                    // Create the new lines with a line break between the XML declaration and the <phrases> element
                    lines = new[] { xmlDeclaration, phrasesElement }.Concat(lines.Skip(1)).ToArray();

                    // Write the file using StreamWriter with UTF-8 encoding with BOM
                    using (StreamWriter writer = new StreamWriter(filePath, false, utf8WithBom))
                    {
                        foreach (var line in lines)
                        {
                            writer.WriteLine(line);
                        }
                    }

                    Console.WriteLine($"Fixed line break in: {filePath} ✨");
                }
            }
        }
    }
}
