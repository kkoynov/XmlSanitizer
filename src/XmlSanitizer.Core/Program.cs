using System.IO;
using XmlSanitizer.Core.Interfaces;
using XmlSanitizer.Core.Processors;

namespace XmlSanitizer.Core
{
    class Program
    {
        /// <summary>
        /// A tool to filter an xml file with CPU performance and memory footprint in mind.
        /// </summary>
        /// <param name="inputXml">The xml file that will be filtered.</param>
        /// <param name="currentSkusCsv"></param>
        /// <param name="outputXmlFilePath">The path to the output xml file.Default is output.xml</param>
        /// <param name="skipHeadersInCsv"></param>
        static void Main(FileInfo inputXml, FileInfo currentSkusCsv, string outputXmlFilePath = "output.xml", bool skipHeadersInCsv = false)
        {
            var existingSkus = Utility.LoadExistingValues(currentSkusCsv.FullName, skipHeadersInCsv);

            var processor = new DefaultXmlProcessor();

            processor.Process(new ProcessRequest()
            {
                InputXmlStream = File.OpenRead(inputXml.FullName),
                OutputXmlStream = File.OpenWrite(outputXmlFilePath),
                FilterOutPredicate = existingSkus.Contains,
                NameOfElementsToReduce = "entry",
                NameOfTheElementsToFilterOn = "item_group_id"
            });
        }
    }
}
