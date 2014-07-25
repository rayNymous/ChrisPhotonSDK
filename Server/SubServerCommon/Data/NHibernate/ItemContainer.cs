using System;
using System.Collections.Generic;
using MayhemCommon;

namespace SubServerCommon.Data.NHibernate
{
    public class ItemContainer
    {
        public virtual Guid Id { get; set; }
        public virtual int Size { get; set; }
        public virtual ContainerType Type { get; set; }
        public virtual IList<MayhemItem> Items { get; set; }
    }
}