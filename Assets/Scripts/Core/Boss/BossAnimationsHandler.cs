using System;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public class BossAnimationsHandler : MonoBehaviour
	{
		public event Action<AttackType> OnAnimationHit;

		public void SwipeOnHit()
		{
			OnAnimationHit?.Invoke(AttackType.BASIC);
		}

		public void RoarOnHit()
		{
			OnAnimationHit?.Invoke(AttackType.SPECIAL);
		}
	}
}