using System;

namespace TrashBoat.Core
{
    [Serializable]
    public enum AttackType
    {
        // BOSS
        BASIC,
        SPECIAL,
        
        // UNIT
        SHIELD,
        DRILL,
        FLAME,
        HEAL,
        ACID,
        PLASMA,
        HEAVY,
        ARMOR
    }
}