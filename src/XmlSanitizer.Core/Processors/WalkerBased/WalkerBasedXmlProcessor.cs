using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using XmlSanitizer.Core.Interfaces;
using XmlSanitizer.Core.Processors.WalkerBased.WalkerBasedXmlProcessor;

namespace XmlSanitizer.Core.Processors
{
    public class WalkerBasedXmlProcessor : IXmlProcessor
    {
        public void Process(ProcessRequest request)
        {
            using (var reader = XmlReader.Create(request.InputXmlStream))
            using (var mainWriter = XmlWriter.Create(request.OutputXmlStream))
            {
                XmlWriter currentWriter = mainWriter;
                var walker = new SimpleWalker(reader, mainWriter, request);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            walker.ElementStart();
                            break;
                        case XmlNodeType.Text:
                            walker.Text();
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                            walker.ProcessingInstructionAndComments();
                            break;
                        case XmlNodeType.EndElement:
                            walker.ElementEnd();
                            break;
                        case XmlNodeType.Whitespace:
                            walker.Whitespace();
                            break;
                        default:
                            throw new Exception("Unaccounted xml node type!");
                    }
                }
            }
        }
    }
}
