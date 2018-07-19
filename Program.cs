using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XPath
{
    class Program
    {
        static void Main()
        {
            const string inputFileName = "C:\\Tmp\\xpath\\122_.xml";
            const string outputFileName = "C:\\Tmp\\xpath\\results.xml";
            const string xpath = "//FlightSegmentReference[@ref='BA0117']/ancestor::AirlineOffer[PricedOffer/Associations/ApplicableFlight/FlightSegmentReference/@ref='BA1594']";

            var nodes = ReadNodes(inputFileName, xpath);

            var resultDocument = BuildResultDocument(nodes);

            FormatAndWriteToFile(resultDocument, outputFileName);  
            
            Console.WriteLine($"{nodes.Count} results saved to file {outputFileName}");
        }

        private static void FormatAndWriteToFile(XmlDocument resultDocument, string fileName)
        {
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, Encoding.Unicode)
            {
                Formatting = Formatting.Indented
            };

            // Write the XML into a formatting XmlTextWriter
            resultDocument.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            var sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            var formattedXml = sReader.ReadToEnd();

            Console.WriteLine(formattedXml);
            
            using (var fileWriter = File.CreateText(fileName))
            {
                fileWriter.WriteLine(formattedXml);
            }
        }

        private static XmlDocument BuildResultDocument(XmlNodeList nodes)
        {
            var resultDocument = new XmlDocument();
            var results = resultDocument.CreateElement("results");
            resultDocument.AppendChild(results);
            
            foreach(XmlNode row in nodes)
            {
                var newRow = resultDocument.ImportNode(row, true);
                results.AppendChild(newRow);
            }

            return resultDocument;
        }

        private static XmlNodeList ReadNodes(string fileName, string xpath)
        {
            var document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.SelectNodes(xpath);
            return nodes;
        }
    }
}