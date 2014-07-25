using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class MayhemItemMap : ClassMap<MayhemItem>
    {
        public MayhemItemMap()
        {
            Id(x => x.Id).Column("id").GeneratedBy.Assigned();
            References(x => x.Container).Column("container_id");
            Map(x => x.SlotIndex).Column("slot_index");
            Map(x => x.TemplateId).Column("template_id");
            Table("item");
        }
    }
}