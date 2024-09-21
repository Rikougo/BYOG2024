using System;
using System.Collections.Generic;
using System.Linq;
using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossStatsAsset m_statsAsset;
        [SerializeField] private GameObject m_bossModel;

        private int m_currentLevel;
        private BossStats m_stats;
        private BossStats m_referenceStats;
        private List<IAttackHandler> m_attackHandlers;
        private bool m_alive;

        public event Action<AttackType> OnAttackTypeLearned;
        public event Action<float, bool, AttackType> OnDamageReceived;
        public event Action<BossStats, BossStats> StatsUpdated;
        public event Action OnDefeated;

        public void Init()
        {
            m_currentLevel = 0;
            this.Reset();
        }
        
        public void Reset()
        {
            m_bossModel.SetActive(true);
            m_alive = true;
            m_stats = m_statsAsset.ComputeStats(m_currentLevel);
            m_referenceStats = m_stats;
            m_attackHandlers = new List<IAttackHandler>();
            this.RegisterAttackHandler(AttackType.BASIC);
            // TODO this.RegisterAttackHandler(AttackType.SPECIAL);
            
            this.StatsUpdated?.Invoke(m_stats, m_referenceStats);
        }
        
        public void Tick(TeamController p_teamController)
        {
            float l_time = Time.time;
            
            foreach (IAttackHandler l_attackHandler in m_attackHandlers)
            {
                if (l_time - l_attackHandler.LastTimeCast > l_attackHandler.Cooldown)
                {
                    l_attackHandler.LastTimeCast = l_time;
                    l_attackHandler.Attack(p_teamController);
                }
            }
        }

        public void Damage(DamagePayload p_payload, PositionType p_position)
        {
            if (!m_alive) return;
            
            float l_damage = p_payload.damageAmount;

            if (m_stats.armor > 0)
            {
                float l_armorDamage = Math.Min(m_stats.armor, l_damage);
                m_stats.armor -= l_armorDamage;

                this.OnDamageReceived?.Invoke(l_armorDamage, true, p_payload.damageType);
                l_damage -= l_armorDamage;
            }

            this.OnDamageReceived?.Invoke(l_damage, false, p_payload.damageType);
            m_stats.health -= l_damage;

            if (m_stats.health <= 0)
            {
                this.Die();
            }
            
            this.StatsUpdated?.Invoke(m_stats, m_referenceStats);
        }

        public void OnUnitDeath(AttackType p_attackType)
        {
            if (!this.HasAttackType(p_attackType))
            {
                // TODO this.RegisterAttackHandler(p_attackType);
            }
        }

        private void RegisterAttackHandler(AttackType p_attackType)
        {
            if (this.HasAttackType(p_attackType))
            {
                Debug.LogError($"BossController::RegisterAttackHandler: Already has attack type [{p_attackType}]");
                return;
            }
            
            IAttackHandler l_handler = BossController.GetHandlerByType(p_attackType);
            
            l_handler.LastTimeCast = Time.time;
            m_attackHandlers.Add(l_handler);

            this.OnAttackTypeLearned?.Invoke(p_attackType);
        }

        private bool HasAttackType(AttackType p_attackType)
        {
            return m_attackHandlers.Any(p_handler => p_handler.Type == p_attackType);
        }

        private void Die()
        {
            m_bossModel.SetActive(false);
            m_alive = false;
            this.OnDefeated?.Invoke();
            m_currentLevel++;
        }

        private static IAttackHandler GetHandlerByType(AttackType p_type)
        {
            IAttackHandler l_handler;
            
            switch (p_type)
            {
                case AttackType.BASIC:
                    l_handler = new BasicAttackHandler();
                    break;
                case AttackType.SPECIAL:
                    l_handler = new SpecialAttackHandler();
                    break;
                default:
                    throw new NotImplementedException();
            }

            l_handler.Type = p_type;

            return l_handler;
        }

        private interface IAttackHandler
        {
            public float LastTimeCast { get; set; }
            public float Cooldown { get; }
            public AttackType Type { get; set; }
            
            public void Attack(TeamController p_teamController);
        }

        private class BasicAttackHandler : IAttackHandler
        {
            public float LastTimeCast { get; set; }
            public float Cooldown => 0.5f;
            public AttackType Type { get; set; }

            public void Attack(TeamController p_teamController)
            {
                p_teamController.DamageTeamMember(PositionTypeHelper.GetFront(), this.GeneratePayload());
            }

            private DamagePayload GeneratePayload()
            {
                return new DamagePayload()
                {
                    isBoss = true,
                    damageAmount = 5.0f,
                    damageType = AttackType.BASIC,
                };
            }
        }

        private class SpecialAttackHandler : IAttackHandler
        {
            public float LastTimeCast { get; set; }
            public float Cooldown => 5.0f;
            public AttackType Type { get; set; }

            public void Attack(TeamController p_teamController)
            {
                throw new NotImplementedException();
            }
        }
    }
}