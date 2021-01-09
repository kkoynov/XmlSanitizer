using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XmlSanitizer.Core;

namespace XmlSanitizer.Tests
{
    public class TestBase
    {
        protected string elementNameToReduce = "entry";
        protected string elementNameToFilterOn = "item_group_id";

        public string RunProcessor(Stream inputXmlStream, HashSet<string> existingSkus, string nameOfElementsToReduce, string nameOfTheElementsToFilterOn)
        {
            using (var outputStream = new MemoryStream())
            {
                var processor = new XmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id), nameOfElementsToReduce, nameOfTheElementsToFilterOn);
                processor.Process();

                outputStream.Position = 0;
                StreamReader reader = new StreamReader(outputStream);
                return reader.ReadToEnd();
            }
        }
        public string RunProcessor(string inputXmlFilePath, HashSet<string> existingSkus, string nameOfElementsToReduce, string nameOfTheElementsToFilterOn)
        {
            using (var inputXmlStream = File.OpenRead(inputXmlFilePath))
            {
                return this.RunProcessor(inputXmlStream, existingSkus, nameOfElementsToReduce, nameOfTheElementsToFilterOn);
            }
        }
    }
}
