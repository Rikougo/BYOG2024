using System.Linq;
using TrashBoat.Core.Boss;
using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
	public class HealerImpl : UnitImpl
	{
		[SerializeField] private LineRenderer m_beamRenderer;
		[SerializeField] private float m_healTime = 1.18f;
		[SerializeField] private float m_healPerSecond = 5f;
		private UnitBrain m_currentTarget;

		private bool m_hasTarget;
		private float m_startHealTime;

		public override void Init()
		{
			base.Init();
			ClearTarget();
			UpdateBeam();
		}

		public override void Pause()
		{
			base.Pause();
			ClearTarget();
			UpdateBeam();
		}

		public override void DeathReset()
		{
			base.DeathReset();
			Debug.Log("[Healer] Death reset, clear target");
			ClearTarget();
		}


		public override void Tick(TeamController p_teamController, BossController p_bossController)
		{
			base.Tick(p_teamController, p_bossController);
			if (CanCast())
			{
				ComputeTarget(p_teamController);
				m_lastCastTime = Time.time;
				m_startHealTime = Time.time;
				OnAttack();
			}

			if (Time.time - m_startHealTime < m_healTime)
			{
				if (m_currentTarget != null) m_currentTarget.Heal(m_healPerSecond * Time.deltaTime);
			}
			else
			{
				ClearTarget();
				UpdateBeam();
			}

			if (m_hasTarget)
				m_beamRenderer
					.SetPosition(1, m_beamRenderer.transform.InverseTransformPoint(m_currentTarget.transform.position));
		}

		private void ComputeTarget(TeamController p_teamController)
		{
			if (m_hasTarget)
			{
				Debug.Log("[Healer] Clear target before compute");
				ClearTarget();
			}

			m_hasTarget = p_teamController.UnitSlots.Any(p_slot => p_slot.isActive && p_slot.unitInstance != Owner);

			if (m_hasTarget)
			{
				m_currentTarget = p_teamController.UnitSlots
					.Where(p_slot => p_slot.isActive && p_slot.unitInstance != Owner)
					.OrderBy(p_slot => p_slot.unitInstance.HealthPercent)
					.First().unitInstance;
				m_currentTarget.OnBeforeDeath += OnTargetDeath;
				Debug.Log($"[Healer] Found target {m_currentTarget.gameObject.name} and subscribed to death");
			}

			UpdateBeam();
		}

		private void ClearTarget()
		{
			if (m_currentTarget != null) m_currentTarget.OnBeforeDeath -= OnTargetDeath;

			m_hasTarget = false;
			m_currentTarget = null;
		}

		private void OnTargetDeath()
		{
			Debug.Log("[Healer] Target died, clear and update beam");
			ClearTarget();
			UpdateBeam();
		}

		private void UpdateBeam()
		{
			m_beamRenderer.enabled = m_hasTarget;
		}
	}
}