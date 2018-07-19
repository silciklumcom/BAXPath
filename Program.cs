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
            ProcessGroup("Default", "BA0173", "BA0174");
            ProcessGroup("Humanitarian", "BA0173", "BA0174");
            ProcessGroup("Published", "BA0173", "BA0174");
            
            ProcessGroup("Default", "BA0177", "BA0176");
            ProcessGroup("Humanitarian", "BA0177", "BA0176");
            ProcessGroup("Published", "BA0177", "BA0176");
            
            ProcessGroup("Default", "BA0115", "BA0182");
            ProcessGroup("Humanitarian", "BA0115", "BA0182");
            ProcessGroup("Published", "BA0115", "BA0182");
        }

        private static void ProcessGroup(string name, string flight1, string flight2)
        {
            var inputFileName = $"C:\\Tmp\\xpath\\{name}.xml";
            var outputFileName = $"C:\\Tmp\\xpath\\{name}_{flight1}_{flight2}.xml";
            
            var nodes = ReadNodes(inputFileName, flight1, flight2);
            var resultDocument = BuildResultDocument(nodes);
            FormatAndWriteToFile(resultDocument, outputFileName);
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

        private static XmlNodeList ReadNodes(string fileName, string flight1, string flight2)
        {
            var xpath = $"//FlightSegmentReference[@ref='{flight1}']/ancestor::AirlineOffer[PricedOffer/Associations/ApplicableFlight/FlightSegmentReference/@ref='{flight2}']";
            
            var document = new XmlDocument();
            document.Load(fileName);

            var nodes = document.SelectNodes(xpath);
            return nodes;
        }
    }
}