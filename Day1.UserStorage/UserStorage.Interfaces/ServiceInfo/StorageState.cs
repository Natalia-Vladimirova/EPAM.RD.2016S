using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Mappers;
using UserStorage.Interfaces.XmlEntities;

namespace UserStorage.Interfaces.ServiceInfo
{
    /// <summary>
    /// Used to contain users and last id of user.
    /// </summary>
    public class StorageState : IXmlSerializable
    {
        /// <summary>
        /// Returns last user id.
        /// </summary>
        public int LastId { get; set; }

        /// <summary>
        /// Returns collection of users.
        /// </summary>
        public List<User> Users { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(nameof(StorageState));
            LastId = reader.ReadElementContentAsInt();
            var serializer = new XmlSerializer(typeof(List<XmlUser>));
            Users = ((List<XmlUser>)serializer.Deserialize(reader)).Select(u => u.ToUser()).ToList();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(LastId), LastId.ToString());
            var serializer = new XmlSerializer(typeof(List<XmlUser>));
            serializer.Serialize(writer, Users.Select(u => u.ToXmlUser()).ToList());
        }
    }
}
