using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public partial class BossController
	{
		private abstract class BaseAttackHandler : IAttackHandler
		{
			protected abstract float DamageCoef { get; }
			public float LastTimeCast { get; set; }
			public abstract float Cooldown { get; }
			public AttackType Type { get; set; }
			public BossStats Stats { get; set; }

			public abstract void Attack(TeamController p_teamController, Animator p_animator);

			public virtual void OnHit(TeamController p_teamController)
			{
			}

			protected DamagePayload GeneratePayload()
			{
				return new DamagePayload
				{
					isBoss = true,
					damageAmount = Stats.damage * DamageCoef,
					damageType = Type
				};
			}
		}
	}
}