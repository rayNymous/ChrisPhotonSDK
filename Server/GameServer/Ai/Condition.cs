using System;

namespace GameServer.Ai
{
    public class Condition : Behaviour
    {
        public Condition()
        {
            Update = () =>
            {
                if (CanRun != null && CanRun())
                {
                    return BhStatus.Success;
                }
                return BhStatus.Failure;
            };
        }

        public Func<bool> CanRun { protected get; set; }
    }
}