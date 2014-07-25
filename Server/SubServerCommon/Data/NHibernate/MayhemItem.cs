using System;

namespace SubServerCommon.Data.NHibernate
{
    public class MayhemItem
    {
        public virtual Guid Id { get; set; }
        public virtual ItemContainer Container { get; set; }
        public virtual int SlotIndex { get; set; }
        public virtual int TemplateId { get; set; }
    }
}