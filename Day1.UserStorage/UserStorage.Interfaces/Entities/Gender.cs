using System.Runtime.Serialization;

namespace UserStorage.Interfaces.Entities
{
    [DataContract]
    public enum Gender
    {
        [EnumMember] Male,
        [EnumMember] Female
    }
}
