using System;
using System.Runtime.Serialization;

namespace UserStorage.Interfaces.Entities
{
    [DataContract]
    public struct Visa
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public DateTime Start { get; set; }

        [DataMember]
        public DateTime End { get; set; }
    }
}
