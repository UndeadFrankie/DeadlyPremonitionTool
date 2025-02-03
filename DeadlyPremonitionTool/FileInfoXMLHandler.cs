using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DeadlyPremonitionTool
{
    public class FileInfoXMLHandler
    {
        // Method to write a list of FileInfoXML objects to an XML file
        public void WriteToXml(List<FileInfoXML> fileInfoList, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoXML>));
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, fileInfoList);
            }
        }

        // Method to read a list of FileInfoXML objects from an XML file
        public List<FileInfoXML> ReadFromXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoXML>));
            using (StreamReader reader = new StreamReader(fileName))
            {
                return (List<FileInfoXML>)serializer.Deserialize(reader);
            }
        }
    }
}
