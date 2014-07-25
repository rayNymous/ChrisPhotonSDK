using FluentNHibernate.Mapping;
using MayhemCommon;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class ItemContainerMap : ClassMap<ItemContainer>
    {
        public ItemContainerMap()
        {
            Id(x => x.Id).Column("id").GeneratedBy.GuidComb();
            Map(x => x.Size).Column("size");
            Map(x => x.Type).Column("type").CustomType<ContainerType>();
            HasMany(x => x.Items).Inverse().Cascade.All().KeyColumns.Add("container_id");
            Table("container");
        }
    }
}