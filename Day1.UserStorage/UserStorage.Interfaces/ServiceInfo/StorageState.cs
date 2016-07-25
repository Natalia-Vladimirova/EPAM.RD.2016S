using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    public class StorageState : IXmlSerializable
    {
        public int LastId { get; set; }
        public List<User> Users { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(nameof(StorageState));
            LastId = reader.ReadElementContentAsInt();
            var serializer = new XmlSerializer(typeof(List<User>));
            Users = (List<User>)serializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(LastId), LastId.ToString());
            var serializer = new XmlSerializer(typeof(List<User>));
            serializer.Serialize(writer, Users);
        }
    }
}
