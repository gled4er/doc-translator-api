//Simple CSV writer class.

using System;
using System.IO;
using System.Text;

namespace TranslationAssistant.Business
{
    public class CsvWriter : IDisposable
    {
        public enum Disposition { translate, DNT, attribute }

        private StreamWriter CsvStream;

        /// <summary>
        /// Creates and initializes a CSV file for writing
        /// </summary>
        /// <param name="TmxFilename">TMX file name</param>
        public CsvWriter(string filename)
        {
            CsvStream = new StreamWriter(filename, false, Encoding.UTF8);
            WriteHeader();
        }


        public void WriteElement(string Element, XMLTranslationManager.Properties props)
        {
            CsvStream.Write("\"{0}\",", CSVEncode(Element));
            CsvStream.Write("\"{0}\",", CSVEncode(props.Type));
            CsvStream.Write("\"{0}\"\n", CSVEncode(props.Disposition));
        }

        private string CSVEncode(string segment)
        {
            segment = segment.Replace("\"", "\"\"");
            return segment;
        }

        private string Statusmessage(Disposition disposition)
        {
            switch (disposition)
            {
                case Disposition.translate:
                    return ("translate");
                case Disposition.DNT:
                    return ("do not translate");
                case Disposition.attribute:
                    return ("attribute");
                default:
                    return("");
            }
        }

        private void WriteHeader()
        {
            CsvStream.WriteLine("\"Name\",\"Type\",\"Disposition\"");
        }

        public void Dispose()
        {
            CsvStream.Flush();
            CsvStream.Close();
            CsvStream.Dispose();
        }
    }
}
