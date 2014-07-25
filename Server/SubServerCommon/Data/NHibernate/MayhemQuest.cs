using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubServerCommon.Data.NHibernate
{
    public class MayhemQuest
    {
        public virtual int Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual int QuestId { get; set; }
        public virtual String Data { get; set; }
        public virtual QuestProgressState Progress { get; set; }
        
    }
}
