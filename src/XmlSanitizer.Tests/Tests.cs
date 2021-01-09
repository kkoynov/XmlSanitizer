using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSanitizer.Core;

namespace XmlSanitizer.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestSimple()
        {
            var inputXmlStream = File.OpenRead(Path.Combine("Resources", "test.xml"));
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));
            var outputStream = new MemoryStream();

            var processor = new XmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id));
            processor.Process();
        }
    }
}
