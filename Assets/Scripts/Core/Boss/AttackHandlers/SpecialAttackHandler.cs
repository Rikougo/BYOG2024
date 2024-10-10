using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public partial class BossController
	{
		private class SpecialAttackHandler : BaseAttackHandler
		{
			public override float Cooldown => 3.0f;
			protected override float DamageCoef => 0.5f;

			public override void Attack(TeamController p_teamController, Animator p_animator)
			{
				Debug.Log($"[Monster] [{Time.time}] Cast Special");
				p_animator.SetTrigger("roar");
			}

			public override void OnHit(TeamController p_teamController)
			{
				Debug.Log($"[Monster] [{Time.time}] On special hit");
				p_teamController.DamageTeamMember(PositionTypeHelper.GetAll(), GeneratePayload());
			}
		}
	}
}