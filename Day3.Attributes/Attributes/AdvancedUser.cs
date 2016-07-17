using System.ComponentModel;

namespace Attributes
{
    public class AdvancedUser : User
    {
        public int _externalId;

        [DefaultValue(3443454)]
        public int ExternalId
        {
            get
            {
                return _externalId;
            }
        }

        [MatchParameterWithProperty("id", "Id")]
        [MatchParameterWithProperty("externalId", "ExternalId")]
        public AdvancedUser(int id, int externalId) : base(id)
        {
            _externalId = externalId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            AdvancedUser user = obj as AdvancedUser;

            if (user == null)
            {
                return false;
            }

            return base.Equals(user) && ExternalId == user.ExternalId;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ ExternalId.GetHashCode();
        }

    }
}
