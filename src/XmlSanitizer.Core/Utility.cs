using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Linq;

namespace XmlSanitizer.Core
{
    public class Utility
    {
        public static void ReadXml(string filePath)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (var fileStream = File.OpenText(filePath))
            using (XmlReader reader = XmlReader.Create(fileStream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            //Console.WriteLine($"Start Element: {reader.Name}. Has Attributes? : {reader.HasAttributes}");
                            break;
                        case XmlNodeType.Text:
                            //Console.WriteLine($"Inner Text: {reader.Value}");
                            break;
                        case XmlNodeType.EndElement:
                            //Console.WriteLine($"End Element: {reader.Name}");
                            break;
                        default:
                            //Console.WriteLine($"Unknown: {reader.NodeType}");
                            break;
                    }
                }
            }
            stopWatch.Stop();

            Console.WriteLine(stopWatch.Elapsed);

        }

        public static void LoadVsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<string>();
                var list = records.ToList();
            }
        }
    }
}