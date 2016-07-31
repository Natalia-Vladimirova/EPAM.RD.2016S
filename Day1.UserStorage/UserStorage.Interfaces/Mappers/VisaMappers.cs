using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.XmlEntities;

namespace UserStorage.Interfaces.Mappers
{
    public static class VisaMappers
    {
        public static Visa ToVisa(this XmlVisa xmlVisa)
        {
            return new Visa
            {
                Country = xmlVisa.Country,
                Start = xmlVisa.Start,
                End = xmlVisa.End
            };
        }

        public static XmlVisa ToXmlVisa(this Visa visa)
        {
            return new XmlVisa
            {
                Country = visa.Country,
                Start = visa.Start,
                End = visa.End
            };
        }
    }
}
