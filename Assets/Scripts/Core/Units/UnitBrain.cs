using System;
using System.Collections;
using Delivivor.UI;
using TMPro;
using TrashBoat.Core.Boss;
using TrashBoat.Core.Units.Implementations;
using UnityEngine;
using UnityEngine.AI;

namespace TrashBoat.Core.Units
{
	public class UnitBrain : MonoBehaviour
	{
		[Header("Dev")] [SerializeField] private TextMeshProUGUI m_text;
		[SerializeField] private RadialProgressUI m_cooldownUI;
		private NavMeshAgent m_agent;
		private float m_armor;
		private float m_health;
		private UnitImpl m_impl;

		private float m_maxHealth;

		public AttackType Type { get; private set; }

		public PositionType Position { get; private set; }

		public float HealthPercent => m_health / m_maxHealth;

		public void Reset()
		{
			m_impl.Init();
		}

		public event Action OnBeforeDeath;
		public event Action<PositionType> OnDeath;

		public void Init(UnitStats p_stats)
		{
			m_maxHealth = p_stats.Health;
			m_health = p_stats.Health;
			Type = p_stats.AttackType;

			m_agent = GetComponent<NavMeshAgent>();
			m_impl = GetComponent<UnitImpl>();
			m_impl.Owner = this;
			m_impl.Init();

			m_text.text = $"{m_health}";

			if (TryGetComponent(out Renderer l_renderer)) l_renderer.material.color = DevGetColor(Type);
		}

		public void Pause()
		{
			m_impl.Pause();
		}

		public void SetPosition(PositionType p_position, Vector3 p_worldPos)
		{
			m_agent.SetDestination(p_worldPos);
			Position = p_position;
			StartCoroutine(nameof(WaitUntilReachTarget));
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
			var l_damage = p_payload.damageAmount;

			if (m_armor > 0)
			{
				var l_armorDamage = Math.Min(m_armor, l_damage);
				m_armor -= l_armorDamage;

				l_damage -= l_armorDamage;
			}

			m_health -= l_damage;
			UpdateHealthText();

			if (m_health <= 0.0f) OnDie();
		}

		public void Heal(float p_amount)
		{
			m_health += p_amount;
			m_health = Math.Min(m_health, m_maxHealth);
			UpdateHealthText();
		}

		public void GrantArmor(float p_amount)
		{
			m_armor += p_amount;
			m_armor = Mathf.Min(m_armor, m_maxHealth);
			UpdateHealthText();
		}

		private void OnDie()
		{
			OnBeforeDeath?.Invoke();
			m_impl.DeathReset();
			OnDeath?.Invoke(Position);
		}

		private void UpdateHealthText()
		{
			m_text.text = m_armor > 0 ? $"{m_health:F0} (+{m_armor:F0})" : $"{m_health:F0}";
		}

		/// <summary>
		///   Wait agent to reach his target
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitUntilReachTarget()
		{
			yield return new WaitForSeconds(0.025f);
			yield return new WaitUntil(() => m_agent.remainingDistance < 0.01);
			transform.rotation = Quaternion.identity;
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