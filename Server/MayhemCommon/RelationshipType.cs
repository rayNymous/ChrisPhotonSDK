using System;

namespace MayhemCommon
{
    [Flags]
    public enum RelationshipType
    {
        Friendly = 0x1,
        Neutral = 0x2,
        Aggressive = 0x4
    }
}