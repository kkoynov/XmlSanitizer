using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using XmlSanitizer.Core;

namespace XmlSanitizer
{
    class Program
    {
        /// <summary>
        /// A tool, which 
        /// </summary>
        /// <param name="feedXml"></param>
        static void Main(FileInfo feedXml, FileInfo currentSkusCsv, string outputXML = "output.xml")
        {
            var existingSkus = Utility.LoadCsv(currentSkusCsv.FullName);

            var processor = new XmlProcessor(feedXml.FullName, outputXML, (id) => existingSkus.Contains(id));

            processor.Process();
        }
    }
}
