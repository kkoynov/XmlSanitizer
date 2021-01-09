using System.IO;
using XmlSanitizer.Core;

namespace XmlSanitizer.CLI
{
    class Program
    {
        /// <summary>
        /// A tool to filter an xml file with CPU performance & memory footprint in mind.
        /// </summary>
        /// <param name="inputXml">The xml file that will be filtered.</param>
        /// <param name="currentSkusCsv"></param>
        /// <param name="outputXmlFilePath">The path to the output xml file.Default is output.xml</param>
        static void Main(FileInfo inputXml, FileInfo currentSkusCsv, string outputXmlFilePath = "output.xml")
        {
            var existingSkus = Utility.LoadExistingValues(currentSkusCsv.FullName, skipHeaders:true);

            var processor = new XmlProcessor(inputXml.FullName, outputXmlFilePath, (id) => existingSkus.Contains(id), "entry", "item_group_id");

            processor.Process();
        }
    }
}
