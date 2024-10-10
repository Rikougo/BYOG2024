using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public partial class BossController
	{
		private interface IAttackHandler
		{
			public float LastTimeCast { get; set; }
			public float Cooldown { get; }
			public AttackType Type { get; set; }
			public BossStats Stats { get; set; }

			public void Attack(TeamController p_teamController, Animator p_animator);
			public void OnHit(TeamController p_teamController);
		}
	}
}