using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ExiLibary
{
    public class Helper
    {
        public static string RemoveLastChar(string decompressedXml, int numbersOfChar)
        {
            decompressedXml = decompressedXml.Remove(decompressedXml.Length - numbersOfChar);
            return decompressedXml;
        }

        public static string AddWithNewLine(string value)
        {
            return string.Format("{0}{1}", value, Environment.NewLine);
        }

        public static string PrintXml(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                mStream.Position = 0;

                StreamReader sReader = new StreamReader(mStream);

                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException ex)
            {
                return xml;
            }
            try
            {
                mStream.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                return xml;
            }

            return result;
        }
    }
}