using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSanitizer.Core;

namespace XmlSanitizer.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestNoneMatching()
        {
            var inputXmlStream = File.OpenRead(Path.Combine("Resources", "test.xml"));
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));
            var outputStream = new MemoryStream();

            var processor = new XmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id));
            processor.Process();

            outputStream.Position = 0;
            StreamReader reader = new StreamReader(outputStream);
            string resultXml = reader.ReadToEnd();
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><feed></feed>", resultXml);
        }

        [TestMethod]
        public void TestSingleMatching()
        {
            var inputXmlStream = File.OpenRead(Path.Combine("Resources", "test.xml"));
            var existingSkus = new HashSet<string>() { "BUASL" };
            var outputStream = new MemoryStream();

            var processor = new XmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id));
            processor.Process();

            outputStream.Position = 0;
            StreamReader reader = new StreamReader(outputStream);
            string resultXml = reader.ReadToEnd();
            Assert.AreEqual(resultXml.Contains("<item_group_id>BUASL</item_group_id>"), true, "Should contain BUALS entry.");
            Assert.AreEqual(resultXml.Split("<entry>").Length, 2, "There should be only one entry.");
        }

        [TestMethod]
        public void TestSkipHeadersDefault()
        {
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));

            Assert.AreEqual(existingSkus.Contains("SKUs"), false);
        }

        [TestMethod]
        public void TestSkipHeadersNotEnabled()
        {
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"), skipHeaders:false);

            Assert.AreEqual(existingSkus.Contains("SKUs"), true);
        }
    }
}
