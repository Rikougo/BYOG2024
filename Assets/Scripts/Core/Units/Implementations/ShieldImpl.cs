using TrashBoat.Core.Boss;
using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
	public class ShieldImpl : UnitImpl
	{
		[SerializeField] private float m_damage;
		[SerializeField] private float m_armorValue;

		public override void Tick(TeamController p_teamController, BossController p_bossController)
		{
			base.Tick(p_teamController, p_bossController);
			if (CanCast())
			{
				m_lastCastTime = Time.time;
				OnAttack();
				p_bossController.Damage(CreateDamagePayload(m_damage), Owner.Position);

				ComputeShieldTarget(p_teamController);

				Owner.GrantArmor(m_armorValue);
			}
		}

		private void ComputeShieldTarget(TeamController p_teamController)
		{
			if (Owner.Position == PositionType.FRONT_LEFT || Owner.Position == PositionType.FRONT_RIGHT)
			{
				var l_targetPos = Owner.Position == PositionType.FRONT_LEFT
					                  ? PositionType.BACK_LEFT
					                  : PositionType.BACK_RIGHT;
				var l_targetSlot = p_teamController[l_targetPos];

				if (l_targetSlot.isActive) l_targetSlot.unitInstance.GrantArmor(m_armorValue);
			}
		}
	}
}