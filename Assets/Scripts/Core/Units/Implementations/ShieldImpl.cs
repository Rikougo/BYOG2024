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
            if (this.CanCast())
            {
                this.m_lastCastTime = Time.time;
                p_bossController.Damage(this.CreateDamagePayload(m_damage), this.Owner.Position);

                if (this.Owner.Position == PositionType.FRONT_LEFT || this.Owner.Position == PositionType.FRONT_RIGHT)
                {
                    PositionType l_targetPos = this.Owner.Position == PositionType.FRONT_LEFT
                                                   ? PositionType.BACK_LEFT
                                                   : PositionType.BACK_RIGHT;
                    TeamController.UnitSlot l_targetSlot = p_teamController[l_targetPos];

                    if (l_targetSlot.isActive)
                    {
                        l_targetSlot.unitInstance.GrantArmor(m_armorValue);
                    }
                }

                this.Owner.GrantArmor(m_armorValue);
            }
        }
    }
}