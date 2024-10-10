using System;
using System.Collections.Generic;
using System.Linq;
using TrashBoat.Core.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrashBoat.Core.Boss
{
	public partial class BossController : MonoBehaviour
	{
		[SerializeField] private BossStatsAsset m_statsAsset;
		[SerializeField] private Animator m_animator;
		[SerializeField] private float m_globalCooldown = 0.5f;
		private bool m_alive;
		private List<IAttackHandler> m_attackHandlers;

		private float m_lastTimeCast;
		private BossStats m_referenceStats;
		private BossStats m_stats;

		public int CurrentLevel { get; private set; }

		public void Reset()
		{
			m_lastTimeCast = Time.time;
			m_alive = true;
			m_animator.SetBool("Alive", m_alive);
			m_stats = m_statsAsset.ComputeStats(CurrentLevel);
			m_referenceStats = m_stats;
			m_attackHandlers = new List<IAttackHandler>();
			RegisterAttackHandler(AttackType.BASIC);
			RegisterAttackHandler(AttackType.SPECIAL);

			StatsUpdated?.Invoke(m_stats, m_referenceStats);
		}

		public event Action<AttackType> OnAttackTypeLearned;
		public event Action<float, bool, AttackType> OnDamageReceived;
		public event Action<BossStats, BossStats> StatsUpdated;
		public event Action OnDefeated;

		public void Init()
		{
			CurrentLevel = 0;
			Reset();
		}

		public void Tick(TeamController p_teamController)
		{
			var l_time = Time.time;

			if (l_time - m_lastTimeCast > m_globalCooldown)
			{
				var l_availableHandlers = m_attackHandlers
					.Where(p_handler => l_time - p_handler.LastTimeCast > p_handler.Cooldown)
					.ToList();

				if (l_availableHandlers.Count > 0)
				{
					var l_chosenHandler = l_availableHandlers[Random.Range(0, l_availableHandlers.Count)];
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

			var l_damage = p_payload.damageAmount;

			if (m_stats.armor > 0)
			{
				var l_armorDamage = Math.Min(m_stats.armor, l_damage);
				m_stats.armor -= l_armorDamage;

				OnDamageReceived?.Invoke(l_armorDamage, true, p_payload.damageType);
				l_damage -= l_armorDamage;
			}

			OnDamageReceived?.Invoke(l_damage, false, p_payload.damageType);
			m_stats.health -= l_damage;

			if (m_stats.health <= 0) Die();

			StatsUpdated?.Invoke(m_stats, m_referenceStats);
		}

		public void OnUnitDeath(AttackType p_attackType)
		{
			if (!HasAttackType(p_attackType))
			{
				// TODO this.RegisterAttackHandler(p_attackType);
			}
		}

		private void RegisterAttackHandler(AttackType p_attackType)
		{
			if (HasAttackType(p_attackType))
			{
				Debug.LogError($"BossController::RegisterAttackHandler: Already has attack type [{p_attackType}]");
				return;
			}

			var l_handler = GetHandlerByType(p_attackType);

			l_handler.LastTimeCast = Time.time;
			l_handler.Stats = m_stats;
			m_attackHandlers.Add(l_handler);

			OnAttackTypeLearned?.Invoke(p_attackType);
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
			OnDefeated?.Invoke();
			CurrentLevel++;
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
	}
}