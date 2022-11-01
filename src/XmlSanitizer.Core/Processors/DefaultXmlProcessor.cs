using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using XmlSanitizer.Core.Interfaces;

namespace XmlSanitizer.Core.Processors
{
    public class DefaultXmlProcessor : IXmlProcessor
    {
        public void Process(ProcessRequest request)
        {
            using (var mainWriter = XmlWriter.Create(request.OutputXmlStream))
            using (var reader = XmlReader.Create(request.InputXmlStream))
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
                            if (reader.Name == request.NameOfElementsToReduce)
                            {
                                processingTargetElement = true;
                                entryElementStream = new MemoryStream();
                                currentWriter = XmlWriter.Create(entryElementStream);
                            }
                            else if (reader.Name == request.NameOfTheElementsToFilterOn && processingTargetElement)
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
                                if (!request.FilterOutPredicate(reader.Value))
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
                            if (reader.Name == request.NameOfElementsToReduce)
                            {
                                currentWriter.Close();
                                if (!discardCurrentNode)
                                {
                                    WriteSubTree(mainWriter, entryElementStream);
                                }
                                discardCurrentNode = !discardCurrentNode;

                                processingTargetElement = false;
                                currentWriter = mainWriter;
                            }
                            if (reader.Name == request.NameOfTheElementsToFilterOn)
                            {
                                inFilterProperty = false;
                            }
                            break;
                        case XmlNodeType.Whitespace:
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

            mainDocument.WriteRaw("\n");
        }
    }
}
