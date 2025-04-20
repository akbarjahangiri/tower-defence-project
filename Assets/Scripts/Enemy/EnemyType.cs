using System;

namespace Enemy
{
    [Flags]
    public enum EnemyType
    {
        Type1 = 0,
        Type2 = 1 << 1,
        Type3 = 1 << 2,
    }
}