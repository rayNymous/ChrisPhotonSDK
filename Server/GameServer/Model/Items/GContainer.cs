using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.Items
{
    public class GContainer
    {
        private readonly GItem[] _items;

        public GContainer(ContainerType type, int size)
        {
            Type = type;
            _items = new GItem[size];
        }

        public GItem[] Items
        {
            get { return _items; }
        }

        public int Size
        {
            get { return _items.Length; }
        }

        public ContainerType Type { get; set; }

        public GItem RemoveItem(int index)
        {
            if (index < 0 || index >= _items.Length)
            {
                return null;
            }

            lock (_items)
            {
                GItem item = _items[index];
                _items[index] = null;
                return item;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i] = null;
            }
        }

        public void ForceAddItem(int index, GItem item)
        {
            _items[index] = item;
        }

        public virtual int AddItem(GItem item, int index = -1)
        {
            if (index >= 0 && index < Size)
            {
                lock (_items)
                {
                    if (_items[index] == null)
                    {
                        _items[index] = item;
                        return index;
                    }
                }
            }
            else
            {
                lock (_items)
                {
                    for (int i = 0; i < _items.Length; i++)
                    {
                        if (_items[i] == null)
                        {
                            _items[i] = item;
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public static implicit operator ContainerData(GContainer container)
        {
            var items = new ItemData[container.Size];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = container._items[i];
            }

            return new ContainerData
            {
                Items = items,
                Name = "Loot ",
                Type = container.Type
            };
        }
    }
}