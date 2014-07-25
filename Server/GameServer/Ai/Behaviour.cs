using System;
using ExitGames.Logging;

namespace GameServer.Ai
{
    public class Behaviour : IBehaviour
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();
        public Action Initialize { protected get; set; }
        public BhStatus Status { get; set; }
        public Func<BhStatus> Update { get; set; }
        public IBehaviour Parent { get; set; }
        public Action<BhStatus> Terminate { protected get; set; }

        public BhStatus Tick()
        {
            if (Status == BhStatus.Invalid && Initialize != null)
            {
                Initialize();
            }

            Status = Update();

            if (Status != BhStatus.Running && Terminate != null)
            {
                Terminate(Status);
            }

            return Status;
        }

        public virtual void Reset()
        {
        }
    }
}