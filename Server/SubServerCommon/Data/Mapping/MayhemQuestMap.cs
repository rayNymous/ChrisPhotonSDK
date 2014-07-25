using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class MayhemQuestMap : ClassMap<MayhemQuest>
    {
        public MayhemQuestMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.UserId).Column("user_id");
            Map(x => x.QuestId).Column("quest_id");
            Map(x => x.Data).Column("data");
            Map(x => x.Progress).CustomType<QuestProgressState>().Column("state");
            Table("user_quest");
        }
    }
}
