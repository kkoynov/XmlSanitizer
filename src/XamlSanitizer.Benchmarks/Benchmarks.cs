using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlSanitizer.Core;

namespace XamlSanitizer.Benchmarks
{
    //[ClrJob, CoreJob]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net481)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    public class Benchmarks
    {
        protected string elementNameToReduce = "entry";
        protected string elementNameToFilterOn = "item_group_id";

        [Benchmark]
        public void UsingStreams()
        {
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var outputXmlFilePath = Path.Combine("Resources", "output.xml");
            var idOfFirstNode = "BEE4B228-0E0C-4EBF-81AE-49C10B8144FD";
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode, idOfFirstNode };

            using (var inputXmlStream = File.Open(inputXmlFilePath,FileMode.Open,FileAccess.Read))
            using (var outputStream = new MemoryStream())
            {
                var processor = new DefaultXmlProcessor(inputXmlStream, outputStream, (id) => existingSkus.Contains(id), elementNameToReduce, elementNameToFilterOn);
                processor.Process();
            }
        }

        //[Benchmark]
        public void UsingPaths()
        {
            var inputXmlFilePath = Path.Combine("Resources", "Footshop_eu.xml");
            var outputXmlFilePath = Path.Combine("Resources", "output.xml");
            var idOfFirstNode = "BEE4B228-0E0C-4EBF-81AE-49C10B8144FD";
            var idOfLastNode = "F17DCA59-7F10-4357-A510-A83C244E046A";
            var existingSkus = new HashSet<string>() { idOfLastNode, idOfFirstNode };

            using (var inputXmlStream = File.OpenRead(inputXmlFilePath))
            using (var outputStream = new MemoryStream())
            {
                var processor = new DefaultXmlProcessor(inputXmlFilePath, outputXmlFilePath, (id) => existingSkus.Contains(id), elementNameToReduce, elementNameToFilterOn);
                processor.Process();
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
