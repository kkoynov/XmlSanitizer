using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlSanitizer.Core
{
    public class XmlProcessor
    {
        private Stream InputXmlStream { get; }
        private Stream OutputXmlStream { get; }
        private Func<string, bool> Predicate { get; }


        public XmlProcessor(string inputXmlPath, string outputXmlPath, Func<string,bool> predicate)
        {
            InputXmlStream = File.OpenRead(inputXmlPath);
            OutputXmlStream= File.OpenRead(outputXmlPath);
            Predicate = predicate;
        }

        public XmlProcessor(Stream inputXmlStream, Stream outputXmlStream, Func<string, bool> predicate)
        {
            InputXmlStream = inputXmlStream;
            OutputXmlStream = outputXmlStream;
            Predicate = predicate;
        }

        public void Process() 
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();

            using (var mainWriter = XmlWriter.Create(InputXmlStream))
            using (var reader = XmlReader.Create(OutputXmlStream))
            {
                XmlWriter currentWriter = mainWriter;
                Stream entryElementStream = null;
                bool processingEntry = false;
                string groupId = "";
                bool inGroupId = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "entry")
                            {
                                processingEntry = true;
                                entryElementStream = new MemoryStream();
                                currentWriter = XmlWriter.Create(entryElementStream);
                            }
                            else if (reader.Name == "item_group_id" && processingEntry)
                            {
                                inGroupId = true;
                            }
                            currentWriter.WriteStartElement(reader.Name);
                            break;
                        case XmlNodeType.Text:
                            if (inGroupId)
                            {
                                groupId = reader.Value;
                            }
                            currentWriter.WriteString(reader.Value);
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            currentWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                            break;
                        case XmlNodeType.Comment:
                            currentWriter.WriteComment(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            currentWriter.WriteFullEndElement();
                            if (reader.Name == "entry")
                            {
                                currentWriter.Close();
                                entryElementStream.Position = 0;
                                if (Predicate(groupId))
                                {
                                    WriteSubTree(mainWriter, entryElementStream);
                                    mainWriter.WriteRaw("\n");
                                }

                                processingEntry = false;
                                currentWriter = mainWriter;
                                groupId = "";
                            }
                            if (reader.Name == "item_group_id")
                            {
                                inGroupId = false;
                            }
                            break;
                        case XmlNodeType.Whitespace:
                            if(processingEntry)
                                currentWriter.WriteRaw(reader.Value);
                            break;
                    }
                }
            }
            stopWatch.Stop();

            Console.WriteLine($"Processing took {stopWatch.Elapsed}.");
        }

        protected void WriteSubTree(XmlWriter mainDocument, Stream subtree)
        {
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
