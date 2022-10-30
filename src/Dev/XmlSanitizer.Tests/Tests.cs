using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlSanitizer.Core;
using XmlSanitizer.Core.Interfaces;
using XmlSanitizer.Core.Processors;
using XmlSanitizer.Core.Processors.WalkerBased;

namespace XmlSanitizer.Tests
{
    [TestClass]
    public class Tests : TestBase
    {
        [DataRow(typeof(DefaultXmlProcessor))]
        [DataRow(typeof(WalkerBasedXmlProcessor))]
        [TestMethod]
        public void TestNoneMatching(Type processorType)
        {
            // Arrange
            var inputXmlFilePath = Path.Combine("Resources", "test.xml");
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));

            // Act
            string resultXml = RunProcessor(CreateProcessor(processorType), CreateRequest(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn));
            
            // Assert
            resultXml.Should().BeEquivalentTo("<?xml version=\"1.0\" encoding=\"UTF-8\"?><feed></feed>");
        }

        [DataRow(typeof(DefaultXmlProcessor))]
        [DataRow(typeof(WalkerBasedXmlProcessor))]
        [TestMethod]
        public void TestSingleMatching(Type processorType)
        {
            // Arrange
            var inputXmlFilePath = Path.Combine("Resources", "test.xml");
            var existingSkus = new HashSet<string>() { "BUASL" };
            
    
            // Act
            string resultXml = RunProcessor(CreateProcessor(processorType), CreateRequest(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn));
            
            // Assert
            resultXml.Should().Contain("<item_group_id>BUASL</item_group_id>", "Should contain BUALS entry.");
            resultXml.Split("<entry>").Length.Should().Be(2, "There should be only one entry.");
        }

        [TestMethod]
        public void TestSkipHeadersDefault()
        {
            // Arrange
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"));
            // Assert
            existingSkus.Should().NotContain("SKUs");
        }

        [TestMethod]
        public void TestSkipHeadersNotEnabled()
        {
            //Arrange
            var existingSkus = Utility.LoadExistingValues(Path.Combine("Resources", "existingtest.csv"), skipHeaders:false);
            // Assert
            existingSkus.Should().Contain("SKUs");
        }

        [DataRow(typeof(DefaultXmlProcessor))]
        [DataRow(typeof(WalkerBasedXmlProcessor))]
        [TestMethod]
        public void TestFootshopSingleMatchinOnLastEntry(Type processorType)
        {
            // Arrange
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode };

            // Act
            string resultXml = RunProcessor(CreateProcessor(processorType), CreateRequest(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn));

            // Assert
            resultXml.Should().Contain($"<item_group_id>{idOfLastNode}</item_group_id>",  $"Should contain {idOfLastNode} entry.");
            resultXml.Split("<entry>").Length.Should().Be(2, "There should be only one entry.");
        }

        [DataRow(typeof(DefaultXmlProcessor))]
        [DataRow(typeof(WalkerBasedXmlProcessor))]
        [TestMethod]
        public void TestFootshopTwoMatchingOnFirstAndLastEntries(Type processorType)
        {
            // Arrange
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var idOfFirstNode = "BEE4B228-0E0C-4EBF-81AE-49C10B8144FD";
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode, idOfFirstNode };

            // Act
            string resultXml = RunProcessor(CreateProcessor(processorType), CreateRequest(inputXmlFilePath, existingSkus, elementNameToReduce, elementNameToFilterOn));

            // Assert
            resultXml.Should().Contain($"<item_group_id>{idOfFirstNode}</item_group_id>", $"Should contain {idOfFirstNode} entry.");
            resultXml.Should().Contain($"<item_group_id>{idOfLastNode}</item_group_id>", $"Should contain {idOfLastNode} entry.");
            resultXml.Split("<entry>").Length.Should().Be(3, "There should be only two entries.");
        }

    }
}
