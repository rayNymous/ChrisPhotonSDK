using System;
using MayhemCommon.MessageObjects;

namespace SubServerCommon.Data.NHibernate
{
    public class MayhemCharacter
    {
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Name { get; set; }
        public virtual string Sex { get; set; }
        public virtual int Fame { get; set; }
        public virtual MayhemZone Zone { get; set; }
        public virtual ItemContainer Inventory { get; set; }
        public virtual string Position { get; set; }
        public virtual int Coins { get; set; }
        public virtual ItemContainer Equipment { get; set; }

        public virtual CharacterListItem BuildCharacterListItem()
        {
            return new CharacterListItem
            {
                Id = Id,
                Name = Name,
                Sex = Sex,
                Deployed = Zone != null,
                Fame = Fame
            };
        }
    }
}