using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrashBoat.Core
{
    public class BossAnimationsHandler : MonoBehaviour
    {
        public event Action<AttackType> OnAnimationHit;
        
        public void SwipeOnHit()
        {
            this.OnAnimationHit?.Invoke(AttackType.BASIC);
        }
        
        public void RoarOnHit()
        {
            this.OnAnimationHit?.Invoke(AttackType.SPECIAL);
        }
    }
}