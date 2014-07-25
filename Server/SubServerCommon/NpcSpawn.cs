namespace SubServerCommon.Data.NHibernate
{
    public class NpcSpawn
    {
        public int Id { get; set; }
        public int NpcTemplate { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Count { get; set; }
    }
}