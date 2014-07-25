using System.Collections.Generic;

namespace GameServer.Ai
{
    public class Composite : Behaviour
    {
        protected Composite()
        {
            Children = new List<IBehaviour>();
            Initialize = () => { };
            Terminate = status => { };
            Update = () => BhStatus.Running;
        }

        protected List<IBehaviour> Children { get; set; }

        public int ChildCount
        {
            get { return Children.Count; }
        }

        public IBehaviour GetChild(int index)
        {
            return Children[index];
        }

        public void Add(Composite composite)
        {
            Children.Add(composite);
        }

        public T Add<T>() where T : class, IBehaviour, new()
        {
            var t = new T {Parent = this};
            Children.Add(t);
            return t;
        }
    }
}