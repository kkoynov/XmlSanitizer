using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlSanitizer.Core
{
    public class DefaultXmlProcessor : IXmlProcessor
    {
        private Stream InputXmlStream { get; init; }
        private Stream OutputXmlStream { get; init; }
        private string NameOfElementsToReduce { get; init; }
        private string NameOfTheElementsToFilterOn { get; init; }

        private Func<string, bool> Predicate { get; init; }

        public DefaultXmlProcessor(string inputXmlPath, string outputXmlPath, Func<string, bool> filterPredicate, string nameOfElementsToReduce, string nameOfTheElementsToFilterOn)
            : this(File.OpenRead(inputXmlPath), File.OpenWrite(outputXmlPath), filterPredicate, nameOfElementsToReduce, nameOfTheElementsToFilterOn)
        {
        }

        public DefaultXmlProcessor(Stream inputXmlStream, Stream outputXmlStream, Func<string, bool> filterPredicate, string nameOfElementsToReduce, string nameOfTheElementsToFIlterOn)
        {
            InputXmlStream = inputXmlStream;
            OutputXmlStream = outputXmlStream;
            Predicate = filterPredicate;
            NameOfElementsToReduce = nameOfElementsToReduce;
            NameOfTheElementsToFilterOn = nameOfTheElementsToFIlterOn;
        }

        public void Process()
        {
            using (var mainWriter = XmlWriter.Create(OutputXmlStream))
            using (var reader = XmlReader.Create(InputXmlStream))
            {
                XmlWriter currentWriter = mainWriter;
                Stream entryElementStream = null;

                bool processingTargetElement = false;
                bool inFilterProperty = false;
                bool discardCurrentNode = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == NameOfElementsToReduce)
                            {
                                processingTargetElement = true;
                                entryElementStream = new MemoryStream();
                                currentWriter = XmlWriter.Create(entryElementStream);
                            }
                            else if (reader.Name == NameOfTheElementsToFilterOn && processingTargetElement)
                            {
                                inFilterProperty = true;
                            }
                            if (!discardCurrentNode)
                            {
                                currentWriter.WriteStartElement(reader.Name);
                            }
                            break;
                        case XmlNodeType.Text:
                            if (inFilterProperty)
                            {
                                if (!Predicate(reader.Value)) 
                                {
                                    discardCurrentNode = true; 
                                }
                            }
                            if (!discardCurrentNode)
                            {
                                currentWriter.WriteString(reader.Value);
                            }
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            if (!discardCurrentNode)
                            {
                                currentWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                            }
                            break;
                        case XmlNodeType.Comment:
                            if (!discardCurrentNode)
                            {
                                currentWriter.WriteComment(reader.Value);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (!discardCurrentNode)
                            {
                                currentWriter.WriteFullEndElement();
                            }
                            if (reader.Name == NameOfElementsToReduce)
                            {
                                currentWriter.Close();
                                if (!discardCurrentNode)
                                {
                                    WriteSubTree(mainWriter, entryElementStream);
                                    mainWriter.WriteRaw("\n");
                                }
                                discardCurrentNode = !discardCurrentNode;

                                processingTargetElement = false;
                                currentWriter = mainWriter;
                            }
                            if (reader.Name == NameOfTheElementsToFilterOn)
                            {
                                inFilterProperty = false;
                            }
                            break;
                        case XmlNodeType.Whitespace:
                            //We may want to move whitespaces to the target when they are not in processed node.
                            if (processingTargetElement && !discardCurrentNode)
                                currentWriter.WriteRaw(reader.Value);
                            break;
                        default:
                            throw new Exception("Unaccounted xml node type!");
                    }
                }
            }
        }

        protected void WriteSubTree(XmlWriter mainDocument, Stream subtree)
        {
            subtree.Position = 0;
            using (var reader = XmlReader.Create(subtree))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            mainDocument.WriteStartElement(reader.Name);
                            break;
                        case XmlNodeType.Text:
                            mainDocument.WriteString(reader.Value);
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            //mainDocument.WriteProcessingInstruction(reader.Name, reader.Value);
                            break;
                        case XmlNodeType.Comment:
                            mainDocument.WriteComment(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            mainDocument.WriteFullEndElement();
                            break;
                        case XmlNodeType.Whitespace:
                            mainDocument.WriteRaw(reader.Value);
                            break;
                    }
                }
            }
        }
    }
}
