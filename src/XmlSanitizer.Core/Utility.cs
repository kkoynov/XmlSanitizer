using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace XmlSanitizer.Core
{
    public class Utility
    {
        public static HashSet<string> LoadCsv(string filePath)
        {
            var result = new HashSet<string>();
            using (var reader = new StreamReader(filePath))
            //using (var csv = new TextReader(reader))
            {
                while(reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    result.Add(line);
                }
            }

            return result;
        }
    }
}