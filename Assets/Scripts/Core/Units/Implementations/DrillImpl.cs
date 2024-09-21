using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
    public class DrillImpl : UnitImpl
    {
        [SerializeField] private float m_tickDamage;
        [SerializeField] private int m_tickAmount;
        [SerializeField] private float m_tickTime;

        private bool m_ticking;
        private float m_lastTickTime;
        private int m_tickingAmount;

        public override void Init()
        {
            base.Init();
            m_ticking = false;
        }

        public override void Tick(TeamController p_teamController, BossController p_bossController)
        {
            base.Tick(p_teamController, p_bossController);
            if (this.CanCast())
            {
                m_lastCastTime = Time.time;
                m_ticking = true;
                m_tickingAmount = 1;
            }

            if (m_ticking)
            {
                if (Time.time - m_lastTickTime > m_tickTime)
                {
                    p_bossController.Damage(this.CreateDamagePayload(m_tickDamage), this.Owner.Position);
                    m_lastTickTime = Time.time;
                    m_tickingAmount++;

                    if (m_tickingAmount > m_tickAmount)
                    {
                        m_ticking = false;
                    }
                }
            }
        }
    }
}