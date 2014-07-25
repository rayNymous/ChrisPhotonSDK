using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Column("id").GeneratedBy.GuidComb();
            Map(x => x.Username).Column("username");
            Map(x => x.Password).Column("password");
            Map(x => x.Salt).Column("salt");
            Map(x => x.Email).Column("email");
            Map(x => x.Created).Column("created_at");
            Map(x => x.Edited).Column("edited_at");

            Map(x => x.CharacterSlots).Column("character_slots");
            References(x => x.Inventory).Column("container_id").Cascade.All();

            Map(x => x.Coins).Column("coins");
            Map(x => x.Fame).Column("fame");
            Table("user");
        }
    }
}