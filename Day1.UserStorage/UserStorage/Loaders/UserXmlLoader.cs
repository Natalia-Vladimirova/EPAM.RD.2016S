using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Loaders
{
    /// <summary>
    /// Loader that loads and saves storage state from/to xml file using file name from App.config.
    /// </summary>
    /// <remarks>
    /// A name of the file is taken from the appSettings tag in App.config. The key name must be 'fileName'.
    /// </remarks>
    public class UserXmlLoader : IUserLoader
    {
        /// <summary>
        /// Loads storage state from the xml file.
        /// </summary>
        /// <returns>
        /// Saved storage state.
        /// </returns>
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

        /// <summary>
        /// Saves storage state to the xml file.
        /// </summary>
        /// <param name="state">
        /// Storage state to save.
        /// </param>
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
