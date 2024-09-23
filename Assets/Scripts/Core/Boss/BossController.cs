using System;
using System.Collections.Generic;
using System.Linq;
using TrashBoat.Core.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrashBoat.Core
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossStatsAsset m_statsAsset;
        [SerializeField] private Animator m_animator;
        [SerializeField] private float m_globalCooldown = 0.5f;

        private int m_currentLevel;
        private BossStats m_stats;
        private BossStats m_referenceStats;
        private List<IAttackHandler> m_attackHandlers;
        private bool m_alive;
        private float m_lastTimeCast;

        public int CurrentLevel => m_currentLevel;
        
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
            m_lastTimeCast = Time.time;
            m_alive = true;
            m_animator.SetBool("Alive", m_alive);
            m_stats = m_statsAsset.ComputeStats(m_currentLevel);
            m_referenceStats = m_stats;
            m_attackHandlers = new List<IAttackHandler>();
            this.RegisterAttackHandler(AttackType.BASIC);
            this.RegisterAttackHandler(AttackType.SPECIAL);
            
            this.StatsUpdated?.Invoke(m_stats, m_referenceStats);
        }
        
        public void Tick(TeamController p_teamController)
        {
            float l_time = Time.time;

            if (l_time - m_lastTimeCast > m_globalCooldown)
            {
                List<IAttackHandler> l_availableHandlers = m_attackHandlers
                    .Where(p_handler => l_time - p_handler.LastTimeCast > p_handler.Cooldown)
                    .ToList();

                if (l_availableHandlers.Count > 0)
                {
                    IAttackHandler l_chosenHandler = l_availableHandlers[Random.Range(0, l_availableHandlers.Count)];
                    l_chosenHandler.Attack(p_teamController, m_animator);
                    l_chosenHandler.LastTimeCast = Time.time;
                }

                m_lastTimeCast = l_time;
            }
        }

        public void AttackHit(AttackType p_type, TeamController p_teamController)
        {
            m_attackHandlers.First(p_handler => p_handler.Type == p_type).OnHit(p_teamController);
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
            l_handler.Stats = m_stats;
            m_attackHandlers.Add(l_handler);

            this.OnAttackTypeLearned?.Invoke(p_attackType);
        }

        private bool HasAttackType(AttackType p_attackType)
        {
            return m_attackHandlers.Any(p_handler => p_handler.Type == p_attackType);
        }

        private void Die()
        {
            m_alive = false;
            m_animator.SetBool("Alive", m_alive);
            m_animator.SetTrigger("death");
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
            public BossStats Stats { get; set; }
            
            public void Attack(TeamController p_teamController, Animator p_animator);
            public void OnHit(TeamController p_teamController);
        }

        private abstract class BaseAttackHandler : IAttackHandler
        {
            public float LastTimeCast { get; set; }
            public abstract float Cooldown { get; }
            public AttackType Type { get; set; }
            public BossStats Stats { get; set; }
            protected abstract float DamageCoef { get; }
            
            public abstract void Attack(TeamController p_teamController, Animator p_animator);
            public virtual void OnHit(TeamController p_teamController) { }

            protected DamagePayload GeneratePayload()
            {
                return new DamagePayload()
                {
                    isBoss = true,
                    damageAmount = this.Stats.damage * this.DamageCoef,
                    damageType = this.Type,
                };
            }
        }

        private class BasicAttackHandler : BaseAttackHandler
        {
            public override float Cooldown => 1.5f;
            protected override float DamageCoef => 1.0f;

            public override void Attack(TeamController p_teamController, Animator p_animator)
            {
                Debug.Log($"[Monster] [{Time.time}] Cast Basic {this.GeneratePayload()}");
                p_animator.SetTrigger("swipe");
            }

            public override void OnHit(TeamController p_teamController)
            {
                Debug.Log($"[Monster] [{Time.time}] On basic hit");
                p_teamController.DamageTeamMember(PositionTypeHelper.GetFront(), this.GeneratePayload());
            }
        }

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
                p_teamController.DamageTeamMember(PositionTypeHelper.GetAll(), this.GeneratePayload());
            }
        }
    }
}