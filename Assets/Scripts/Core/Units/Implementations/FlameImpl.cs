using TrashBoat.Core.Boss;
using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
	public class FlameImpl : UnitImpl
	{
		[SerializeField] private float m_flameDuration = 1.5f;
		[SerializeField] private float m_damagePerSecond = 100.0f;
		[SerializeField] private float m_tickTime;
		[SerializeField] private LineRenderer m_flameRenderer;

		private bool m_attacking;
		private float m_attackTime;
		private float m_tickingTime;

		public override void Init()
		{
			base.Init();
			m_attacking = false;
			m_flameRenderer.enabled = false;
		}

		public override void Pause()
		{
			base.Pause();
			m_flameRenderer.enabled = false;
		}

		public override void Tick(TeamController p_teamController, BossController p_bossController)
		{
			base.Tick(p_teamController, p_bossController);

			if (CanCast())
			{
				m_lastCastTime = Time.time;

				m_attacking = true;
				m_attackTime = Time.time;
				m_tickingTime = Time.time - m_tickTime;
				m_flameRenderer.enabled = true;
				OnAttack();
			}

			if (m_attacking)
			{
				if (Time.time - m_tickingTime > m_tickTime)
				{
					p_bossController.Damage(CreateDamagePayload(m_damagePerSecond * m_tickTime), Owner.Position);
					m_tickingTime = Time.time;
				}

				if (Time.time - m_attackTime > m_flameDuration)
				{
					m_attacking = false;
					m_flameRenderer.enabled = false;
				}
			}
		}
	}
}