using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    public class UserLoader : IUserLoader
    {
        public StorageState Load()
        {
            var formatter = new XmlSerializer(typeof(StorageState));

            using (var stream = new FileStream(GetFileName(), FileMode.OpenOrCreate))
            {
                return (StorageState)formatter.Deserialize(stream);
            }
        }

        public void Save(StorageState state)
        {
            var formatter = new XmlSerializer(typeof(StorageState));

            using (var stream = new FileStream(GetFileName(), FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, state);
            }
        }
        
        private string GetFileName()
        {
            string fileName = ConfigurationManager.AppSettings["fileName"];

            if (fileName == null)
            {
                throw new KeyNotFoundException("Key 'fileName' is not found in config.");
            }

            return fileName;
        }

    }
}

