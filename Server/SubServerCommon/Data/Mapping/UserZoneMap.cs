using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class UserZoneMap : ClassMap<UserZone>
    {
        public UserZoneMap()
        {
            Id(x => x.Id).Column("id");
            References(x => x.User).Column("user_id");
            References(x => x.Zone).Column("zone_id");
            Map(x => x.Stars).Column("stars");
            Table("user_zone");
        }
    }
}