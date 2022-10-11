using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlSanitizer.Core
{
    internal class SimpleWalker
    {
        private XmlReader Reader { get; init; }
        private XmlWriter MainWriter{ get; init; }
        private XmlWriter CurrentWriter{ get; set; }

        private string NameOfElementsToReduce { get; init; }
        private string NameOfTheElementsToFilterOn { get; init; }

        private Func<string, bool> Predicate { get; init; }



        public SimpleWalker(XmlReader reader, XmlWriter mainWriter, Func<string, bool> filterPredicate, string nameOfElementsToReduce, string nameOfTheElementsToFIlterOn)
        {
            Reader = reader;
            MainWriter = CurrentWriter = mainWriter;
            Predicate = filterPredicate;
            NameOfElementsToReduce = nameOfElementsToReduce;
            NameOfTheElementsToFilterOn = nameOfTheElementsToFIlterOn;
        }
        public void ElementStart() 
        {
        }

        public void Text()
        {
        }

        public void ProcessingInstruction()
        {
        }

        public void ElementEnd()
        {
        }

        public void Whitespace()
        {
        }
    }
}
