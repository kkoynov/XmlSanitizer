using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSanitizer.Core.Interfaces;
using XmlSanitizer.Core.Processors;
using XmlSanitizer.Core.Processors.WalkerBased;

namespace XamlSanitizer.Benchmarks
{
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net481)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    public class Benchmarks
    {
        protected string elementNameToReduce = "entry";
        protected string elementNameToFilterOn = "item_group_id";
        [Params(typeof(DefaultXmlProcessor), typeof(WalkerBasedXmlProcessor))]
        public Type? processorType;

        //Ignoring this since is Benchmark related and is guaranteed not to be null during benchmark runs.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IXmlProcessor processor;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (processorType is null) { throw new ArgumentException("processorType should not be null"); }
            var NewProcessor = Activator.CreateInstance(processorType) as IXmlProcessor;
            if (NewProcessor is null) { throw new ArgumentException("processor should not be null"); }
            processor = NewProcessor;
        }

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
                if (processor is null) { throw new ArgumentException("processor should not be null"); }
                processor.Process(new ProcessRequest()
                {
                    InputXmlStream = inputXmlStream,
                    OutputXmlStream = outputStream,
                    FilterOutPredicate = existingSkus.Contains,
                    NameOfElementsToReduce = elementNameToReduce,
                    NameOfTheElementsToFilterOn = elementNameToFilterOn
                });
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
