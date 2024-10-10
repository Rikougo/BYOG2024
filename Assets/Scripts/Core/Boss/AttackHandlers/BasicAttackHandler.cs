using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public partial class BossController
	{
		private class BasicAttackHandler : BaseAttackHandler
		{
			public override float Cooldown => 1.5f;
			protected override float DamageCoef => 1.0f;

			public override void Attack(TeamController p_teamController, Animator p_animator)
			{
				Debug.Log($"[Monster] [{Time.time}] Cast Basic {GeneratePayload()}");
				p_animator.SetTrigger("swipe");
			}

			public override void OnHit(TeamController p_teamController)
			{
				Debug.Log($"[Monster] [{Time.time}] On basic hit");
				p_teamController.DamageTeamMember(PositionTypeHelper.GetFront(), GeneratePayload());
			}
		}
	}
}