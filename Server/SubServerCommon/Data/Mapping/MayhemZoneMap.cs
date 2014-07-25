using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class MayhemZoneMap : ClassMap<MayhemZone>
    {
        public MayhemZoneMap()
        {
            Id(x => x.Id).Column("id").GeneratedBy.GuidComb();
            Map(x => x.TemplateId).Column("template_id");
            Map(x => x.Price).Column("price");
            Map(x => x.Type).Column("type");
            Map(x => x.MaxPlayers).Column("max_players");
            Map(x => x.Order).Column("order");
            Map(x => x.Name).Column("name");
            Map(x => x.Thumbnail).Column("thumbnail");
            Table("zone");
        }
    }
}