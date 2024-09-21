using System.Linq;
using UnityEngine;

namespace TrashBoat.Core.Units.Implementations
{
    public class HealerImpl : UnitImpl
    {
        [SerializeField] private LineRenderer m_beamRenderer;
        [SerializeField] private float m_healPerSecond = 5f;
        
        private bool m_hasTarget;
        private UnitBrain m_currentTarget;
        
        public override void Init()
        {
            base.Init();
            this.ClearTarget();
            this.UpdateBeam();
        }

        public override void Pause()
        {
            base.Pause();
            this.ClearTarget();
            this.UpdateBeam();
        }
        
        public override void DeathReset()
        {
            base.DeathReset();
            Debug.Log("[Healer] Death reset, clear target");
            this.ClearTarget();
        }
        

        public override void Tick(TeamController p_teamController, BossController p_bossController)
        {
            base.Tick(p_teamController, p_bossController);
            if (this.CanCast())
            {
                this.ComputeTarget(p_teamController);
                m_lastCastTime = Time.time;
            }

            if (m_currentTarget != null)
            {
                m_currentTarget.Heal(m_healPerSecond * Time.deltaTime);
            }
            
            if (m_hasTarget)
            {
                m_beamRenderer.SetPosition(1, this.transform.InverseTransformPoint(m_currentTarget.GetComponent<Renderer>().bounds.center));
            }
        }

        private void ComputeTarget(TeamController p_teamController)
        {
            if (m_hasTarget)
            {
                Debug.Log("[Healer] Clear target before compute");
                this.ClearTarget();
            }
            
            m_hasTarget = p_teamController.UnitSlots.Any(p_slot => p_slot.isActive && p_slot.unitInstance != this.Owner);

            if (m_hasTarget)
            {
                m_currentTarget = p_teamController.UnitSlots
                    .Where(p_slot => p_slot.isActive && p_slot.unitInstance != this.Owner)
                    .OrderBy(p_slot => p_slot.unitInstance.HealthPercent)
                    .First().unitInstance;
                m_currentTarget.OnPredeath += this.OnTargetDeath;
                Debug.Log($"[Healer] Found target {m_currentTarget.gameObject.name} and subscribed to death");
            }

            this.UpdateBeam();
        }

        private void ClearTarget()
        {
            if (m_currentTarget != null)
            {
                m_currentTarget.OnPredeath -= this.OnTargetDeath;
            }  
            
            m_hasTarget = false;
            m_currentTarget = null;
        }

        private void OnTargetDeath()
        {
            Debug.Log("[Healer] Target died, clear and update beam");
            this.ClearTarget();
            this.UpdateBeam();
        }

        private void UpdateBeam()
        {
            m_beamRenderer.enabled = m_hasTarget;
        }
    }
}