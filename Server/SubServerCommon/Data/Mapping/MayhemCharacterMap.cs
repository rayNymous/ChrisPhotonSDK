using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class MayhemCharacterMap : ClassMap<MayhemCharacter>
    {
        public MayhemCharacterMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Name).Column("name");
            References(x => x.Zone).Column("zone_id");
            References(x => x.User).Column("user_id");
            Map(x => x.Fame).Column("fame");
            Map(x => x.Sex).Column("sex");
            References(x => x.Inventory).Column("inventory_id").Cascade.All();
            Map(x => x.Position).Column("position");
            Map(x => x.Coins).Column("coins");
            References(x => x.Equipment).Column("equipment_id").Cascade.All();
            Table("game_character");
        }
    }
}