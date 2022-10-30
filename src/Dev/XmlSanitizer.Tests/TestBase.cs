using System;
using System.Collections.Generic;
using System.IO;
using XmlSanitizer.Core.Interfaces;

namespace XmlSanitizer.Tests
{
    public class TestBase
    {
        protected string elementNameToReduce = "entry";
        protected string elementNameToFilterOn = "item_group_id";

        public string RunProcessor(IXmlProcessor processor, ProcessRequest request)
        {
            processor.Process(request);

            request.OutputXmlStream.Position = 0;
            StreamReader reader = new StreamReader(request.OutputXmlStream);
            return reader.ReadToEnd();
        }

        protected IXmlProcessor CreateProcessor(Type type)
        {
            return (IXmlProcessor)Activator.CreateInstance(type);
        }

        protected ProcessRequest CreateRequest(string inputXmlFilePath, HashSet<string> existingSkus, string nameOfElementsToReduce, string nameOfTheElementsToFilterOn)
        {
            return new ProcessRequest()
            {
                InputXmlStream = File.OpenRead(inputXmlFilePath),
                OutputXmlStream = new MemoryStream(),
                FilterOutPredicate = existingSkus.Contains,
                NameOfElementsToReduce = nameOfElementsToReduce,
                NameOfTheElementsToFilterOn = nameOfTheElementsToFilterOn
            };
        }
    }
}
