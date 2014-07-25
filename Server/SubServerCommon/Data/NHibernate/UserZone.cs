namespace SubServerCommon.Data.NHibernate
{
    public class UserZone
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual MayhemZone Zone { get; set; }
        public virtual int Stars { get; set; }
    }
}