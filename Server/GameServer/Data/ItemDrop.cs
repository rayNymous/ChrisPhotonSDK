namespace GameServer.Data
{
    public class ItemDrop
    {
        private int _maxStackSize = 1;
        private int _minStackSize = 1;

        public ItemDrop(int templateId, float dropChance)
        {
            TemplateId = templateId;
            DropChance = dropChance;
        }

        public int TemplateId { get; set; }
        public float DropChance { get; set; }

        public int MinStackSize
        {
            get { return _minStackSize; }
        }

        public int MaxStackSize
        {
            get { return _maxStackSize; }
        }
    }
}