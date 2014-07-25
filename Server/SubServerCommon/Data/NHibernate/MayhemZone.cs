using System;
using MayhemCommon;

namespace SubServerCommon.Data.NHibernate
{
    public class MayhemZone
    {
        public virtual Guid Id { get; set; }
        public virtual int TemplateId { get; set; }
        public virtual int Price { get; set; }
        public virtual ZoneType Type { get; set; }
        public virtual int MaxPlayers { get; set; }
        public virtual int Order { get; set; }
        public virtual String Name { get; set; }
        public virtual String Thumbnail { get; set; }
    }
}