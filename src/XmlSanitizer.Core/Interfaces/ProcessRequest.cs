using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XmlSanitizer.Core.Interfaces
{
    public class ProcessRequest
    {
        public Stream InputXmlStream { get; init; }
        public Stream OutputXmlStream { get; init; }
        public Func<string, bool> FilterOutPredicate { get; init; }
        public string NameOfElementsToReduce { get; init; }
        public string NameOfTheElementsToFilterOn { get; init; }
    }
}
