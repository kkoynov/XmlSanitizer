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
        private string NameOfElementsToReduce { get; }
        private string NameOfTheElementsToFilterOn { get; }

        private Func<string, bool> Predicate { get; }

        public XmlProcessor(string inputXmlPath, string outputXmlPath, Func<string,bool> filterPredicate, string nameOfElementsToReduce, string nameOfTheElementsToFilterOn) 
            : this(File.OpenRead(inputXmlPath), File.OpenRead(outputXmlPath), filterPredicate, nameOfElementsToReduce, nameOfTheElementsToFilterOn)
        {
        }

        public XmlProcessor(Stream inputXmlStream, Stream outputXmlStream, Func<string, bool> filterPredicate, string nameOfElementsToReduce, string nameOfTheElementsToFIlterOn)
        {
            InputXmlStream = inputXmlStream;
            OutputXmlStream = outputXmlStream;
            Predicate = filterPredicate;
            NameOfElementsToReduce = nameOfElementsToReduce;
            NameOfTheElementsToFilterOn = nameOfTheElementsToFIlterOn;
        }

        public void Process() 
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();

            using (var mainWriter = XmlWriter.Create(OutputXmlStream))
            using (var reader = XmlReader.Create(InputXmlStream))
            {
                XmlWriter currentWriter = mainWriter;
                Stream entryElementStream = null;
                bool processingTargetElement = false;
                string filterPropertyName = "";
                bool inFilterProperty = false;
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
                            currentWriter.WriteStartElement(reader.Name);
                            break;
                        case XmlNodeType.Text:
                            if (inFilterProperty)
                            {
                                filterPropertyName = reader.Value;
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
                            if (reader.Name == NameOfElementsToReduce)
                            {
                                currentWriter.Close();
                                entryElementStream.Position = 0;
                                if (Predicate(filterPropertyName))
                                {
                                    WriteSubTree(mainWriter, entryElementStream);
                                    mainWriter.WriteRaw("\n");
                                }

                                processingTargetElement = false;
                                currentWriter = mainWriter;
                                filterPropertyName = "";
                            }
                            if (reader.Name == NameOfTheElementsToFilterOn)
                            {
                                inFilterProperty = false;
                            }
                            break;
                        case XmlNodeType.Whitespace:
                            if(processingTargetElement)
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
