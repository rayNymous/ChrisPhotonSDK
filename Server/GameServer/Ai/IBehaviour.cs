using System;

namespace GameServer.Ai
{
    public interface IBehaviour
    {
        BhStatus Status { get; set; }
        Action Initialize { set; }
        Func<BhStatus> Update { set; }
        Action<BhStatus> Terminate { set; }
        IBehaviour Parent { get; set; }

        BhStatus Tick();
        void Reset();
    }
}