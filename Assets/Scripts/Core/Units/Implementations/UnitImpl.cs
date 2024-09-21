using Delivivor.UI;
using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
    public class UnitImpl : MonoBehaviour
    {
        [SerializeField] protected float m_cooldown = 2.0f;
        
        protected float m_lastCastTime;
        
        public UnitBrain Owner { get; set; }
        public float CooldownProgress => (Time.time - m_lastCastTime) / m_cooldown;

        public virtual void Init()
        {
            m_lastCastTime = Time.time;
        }
        
        public virtual void Tick(TeamController p_teamController, BossController p_bossController)
        {
        }

        public virtual void Pause()
        {
            
        }

        public virtual void DeathReset()
        {
            
        }

        protected bool CanCast()
        {
            return Time.time - m_lastCastTime >= m_cooldown;
        }

        protected DamagePayload CreateDamagePayload(float p_damage)
        {
            return new DamagePayload()
            {
                isBoss = false,
                damageType = this.Owner.Type,
                damageAmount = p_damage
            };
        }
    }
}