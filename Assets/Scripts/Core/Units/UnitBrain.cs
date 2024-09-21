using System;
using System.Collections;
using Delivivor.UI;
using TMPro;
using TrashBoat.Core.Units.Implementations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace TrashBoat.Core.Units
{
    public class UnitBrain : MonoBehaviour
    {
        [Header("Dev")] 
        [SerializeField] private TextMeshProUGUI m_text;
        [SerializeField] private RadialProgressUI m_cooldownUI;

        private float m_maxHealth;
        private float m_health;
        private float m_armor;
        private AttackType m_attackType;
        private PositionType m_position;
        private UnitImpl m_impl;
        private NavMeshAgent m_agent;
        
        public AttackType Type => m_attackType;
        public PositionType Position => m_position;
        public float HealthPercent => m_health / m_maxHealth;
        public event Action OnPredeath;
        public event Action<PositionType> OnDeath; 
        
        public void Init(UnitStats p_stats)
        {
            m_maxHealth = p_stats.Health;
            m_health = p_stats.Health;
            m_attackType = p_stats.AttackType;

            m_agent = this.GetComponent<NavMeshAgent>();
            m_impl = this.GetComponent<UnitImpl>();
            m_impl.Owner = this;
            m_impl.Init();

            m_text.text = $"{m_health}";

            this.GetComponent<Renderer>().material.color = DevGetColor(m_attackType);
        }

        public void Reset()
        {
            m_impl.Init();
        }

        public void Pause()
        {
            m_impl.Pause();
        }

        public void SetPosition(PositionType p_position, Vector3 p_worldPos)
        {
            m_agent.SetDestination(p_worldPos);
            m_position = p_position;
            this.StartCoroutine("WaitUntilReachTarget");
        }

        public void Tick(TeamController p_teamController, BossController p_bossController)
        {
            if (m_impl != null)
            {
                m_cooldownUI.SetProgress(m_impl.CooldownProgress);
                m_impl.Tick(p_teamController, p_bossController);
            }
        }
        
        public void TakeDamage(DamagePayload p_payload)
        {
            float l_damage = p_payload.damageAmount;

            if (m_armor > 0)
            {
                float l_armorDamage = Math.Min(m_armor, l_damage);
                m_armor -= l_armorDamage;
                
                l_damage -= l_armorDamage;
            }
            
            m_health -= l_damage;
            this.UpdateHealthText();

            if (m_health <= 0.0f)
            {
                this.OnDie();
            }
        }

        public void Heal(float p_amount)
        {
            m_health += p_amount;
            m_health = Math.Min(m_health, m_maxHealth);
            this.UpdateHealthText();
        }

        public void GrantArmor(float p_amount)
        {
            m_armor += p_amount;
            m_armor = Mathf.Min(m_armor, m_maxHealth);
            this.UpdateHealthText();
        }

        private void OnDie()
        {
            this.OnPredeath?.Invoke();
            m_impl.DeathReset();
            this.OnDeath?.Invoke(m_position);
        }

        private void UpdateHealthText()
        {
            m_text.text = m_armor > 0 ? $"{m_health:F0} (+{m_armor:F0})" : $"{m_health:F0}";
        }
        
        IEnumerator WaitUntilReachTarget(){		
		    yield return new WaitForSeconds(0.025f);
		    yield return new WaitUntil(() => m_agent.remainingDistance < 0.01);
		    transform.rotation = quaternion.identity;
        }

        public static Color DevGetColor(AttackType p_attackType)
        {
            switch (p_attackType)
            {
                case AttackType.SHIELD:
                    return Color.cyan;
                case AttackType.DRILL:
                    return Color.yellow;
                case AttackType.HEAL:
                    return Color.green;
                case AttackType.FLAME:
                    return Color.red;
                default:
                    return Color.gray;
            }
        }
    }
}
