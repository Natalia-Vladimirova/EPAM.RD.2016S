using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Loaders
{
    [Serializable]
    public class UserXmlLoader : IUserLoader
    {
        public StorageState Load()
        {
            var formatter = new XmlSerializer(typeof(StorageState));

            using (var stream = new FileStream(GetFileName(), FileMode.OpenOrCreate))
            {
                if (stream.Length == 0)
                {
                    return new StorageState()
                    {
                        Users = new List<User>()
                    };
                }

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
