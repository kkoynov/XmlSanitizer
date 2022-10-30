using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XmlSanitizer.Core.Interfaces
{
    public class ProcessRequest
    {
        public Stream InputXmlStream { get; set; }
        public Stream OutputXmlStream { get; set; }
        public Func<string, bool> FilterOutPredicate { get; set; }
        public string NameOfElementsToReduce { get; set; }
        public string NameOfTheElementsToFilterOn { get; set; }
    }
}
