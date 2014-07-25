using System;

namespace SubServerCommon.Data.NHibernate
{
    public class User
    {
        public virtual Guid Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Salt { get; set; }
        public virtual string Email { get; set; }
        public virtual ItemContainer Inventory { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Edited { get; set; }
        public virtual int CharacterSlots { get; set; }
        public virtual int Fame { get; set; }
        public virtual int Coins { get; set; }
    }
}