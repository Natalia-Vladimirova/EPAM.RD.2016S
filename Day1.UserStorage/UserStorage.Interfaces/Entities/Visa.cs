using System;

namespace UserStorage.Interfaces.Entities
{
    [Serializable]
    public struct Visa
    {
        public string Country { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
