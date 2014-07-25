using System;

namespace GameServer.Ai
{
    public class Decorator : Composite
    {
        public Decorator()
        {
            Update = () =>
            {
                if (CanRun != null && CanRun() && Children != null && Children.Count > 0)
                {
                    return Children[0].Tick();
                }
                return ReturnStatus;
            };
        }

        public Func<bool> CanRun { protected get; set; }
        public BhStatus ReturnStatus { protected get; set; }
    }
}