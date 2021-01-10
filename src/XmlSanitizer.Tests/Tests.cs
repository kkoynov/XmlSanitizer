using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSanitizer.Core;

namespace XmlSanitizer.Tests
{
    [TestClass]
    public class Tests : TestBase
    {
        [TestMethod]
        public void TestNoneMatching()
        {
            var inputXmlFilePath = Path.Combine("Resources", "test.xml");
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));

            string resultXml = RunProcessor(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn);

            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><feed></feed>", resultXml);
        }

        [TestMethod]
        public void TestSingleMatching()
        {
            var inputXmlFilePath = Path.Combine("Resources", "test.xml");
            var existingSkus = new HashSet<string>() { "BUASL" };
    
            string resultXml = RunProcessor(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn);

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

        [TestMethod]
        public void TestFootshopSingleMatchinOnLastEntry()
        {
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode };

            string resultXml = RunProcessor(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn);

            Assert.AreEqual(resultXml.Contains($"<item_group_id>{idOfLastNode}</item_group_id>"), true, $"Should contain {idOfLastNode} entry.");
            Assert.AreEqual(resultXml.Split("<entry>").Length, 2, "There should be only one entry.");
        }

        [TestMethod]
        public void TestFootshopTwoMatchingOnFirstAndLastEntries()
        {
            Assert.Fail();
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var idOfFirstNode = "BEE4B228-0E0C-4EBF-81AE-49C10B8144FD";
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode, idOfFirstNode };

            string resultXml = RunProcessor(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn);

            Assert.AreEqual(resultXml.Contains($"<item_group_id>{idOfFirstNode}</item_group_id>"), true, $"Should contain {idOfFirstNode} entry.");
            Assert.AreEqual(resultXml.Contains($"<item_group_id>{idOfLastNode}</item_group_id>"), true, $"Should contain {idOfLastNode} entry.");
            Assert.AreEqual(resultXml.Split("<entry>").Length, 3, "There should be only two entries.");
        }

    }
}
