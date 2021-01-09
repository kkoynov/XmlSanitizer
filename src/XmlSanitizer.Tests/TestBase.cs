using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XmlSanitizer.Core;

namespace XmlSanitizer.Tests
{
    public class TestBase
    {
        public string RunProcessor(Stream inputXmlStream, HashSet<string> existingSkus)
        {
            using (var outputStream = new MemoryStream())
            {
                var processor = new XmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id));
                processor.Process();

                outputStream.Position = 0;
                StreamReader reader = new StreamReader(outputStream);
                return reader.ReadToEnd();
            }
        }
        public string RunProcessor(string inputXmlFilePath, HashSet<string> existingSkus)
        {
            using (var inputXmlStream = File.OpenRead(inputXmlFilePath))
            {
                return this.RunProcessor(inputXmlStream, existingSkus);
            }
        }
    }
}
