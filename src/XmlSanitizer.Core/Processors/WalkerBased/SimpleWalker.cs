using System.IO;
using System.Xml;
using XmlSanitizer.Core.Interfaces;

namespace XmlSanitizer.Core.Processors.WalkerBased
{
    internal struct SimpleWalker
    {
        XmlReader Reader { get; init; }
        XmlWriter MainWriter { get; init; }
        XmlWriter CurrentWriter { get; set; }

        ProcessRequest Request { get; init; }

        Stream EntryElementStream { get; set; }


        bool processingTargetElement = false;
        bool inFilterProperty = false;
        bool discardCurrentNode = false;

        private void WriteSubTree(XmlWriter mainDocument, Stream subtree)
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

        public SimpleWalker(XmlReader reader, XmlWriter mainWriter, ProcessRequest request)
        {
            Reader = reader;
            MainWriter = CurrentWriter = mainWriter;
            Request = request;
        }

        public void ElementStart()
        {
            if (Reader.Name == Request.NameOfElementsToReduce)
            {
                processingTargetElement = true;
                EntryElementStream = new MemoryStream();
                CurrentWriter = XmlWriter.Create(EntryElementStream);
            }
            else if (Reader.Name == Request.NameOfTheElementsToFilterOn && processingTargetElement)
            {
                inFilterProperty = true;
            }
            CurrentWriter.WriteStartElement(Reader.Name);
        }

        public void Text()
        {
            if (inFilterProperty)
            {
                if (!Request.FilterOutPredicate(Reader.Value))
                {
                    discardCurrentNode = true;
                }
            }
            CurrentWriter.WriteString(Reader.Value);
        }

        public void ProcessingInstructionAndComments()
        {
            CurrentWriter.WriteProcessingInstruction(Reader.Name, Reader.Value);
        }

        public void ElementEnd()
        {
            CurrentWriter.WriteFullEndElement();
            if (Reader.Name == Request.NameOfElementsToReduce)
            {
                CurrentWriter.Close();
                if (!discardCurrentNode)
                {
                    WriteSubTree(MainWriter, EntryElementStream);
                    MainWriter.WriteRaw("\n");
                }
                discardCurrentNode = !discardCurrentNode;

                processingTargetElement = false;
                CurrentWriter = MainWriter;
            }
            if (Reader.Name == Request.NameOfTheElementsToFilterOn)
            {
                inFilterProperty = false;
            }
        }

        public void Whitespace()
        {
            //We may want to move whitespaces to the target when they are not in processing node.
            if (processingTargetElement && !discardCurrentNode)
                CurrentWriter.WriteRaw(Reader.Value);
        }
    }
}
