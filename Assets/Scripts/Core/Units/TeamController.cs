using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace TrashBoat.Core.Units
{
    public class TeamController : MonoBehaviour
    {
        public class UnitSlot
        {
            public bool isActive;
            public UnitBrain unitInstance;

            public UnitSlot()
            {
                this.isActive = false;
                this.unitInstance = null;
            }
        }
        
        [Header("Dev")] 
        [SerializeField] private UnitBrain m_defaultUnitPrefab;
        [SerializeField] private UnitDatabase m_unitDatabase;

        [Header("World references")] 
        [SerializeField] private Bounds m_spawnArea;
        [SerializeField] private Transform m_slotFrontLeft;
        [SerializeField] private Transform m_slotFrontRight;
        [SerializeField] private Transform m_slotBackLeft;
        [SerializeField] private Transform m_slotBackRight;

        [SerializeField] private InputAction m_action;

        private Dictionary<PositionType, UnitSlot> m_units;

        public UnitSlot this[PositionType p_pos] => m_units[p_pos];
        public UnitSlot[] UnitSlots => m_units.Values.ToArray();

        public event Action<AttackType> OnUnitDie;
        public event Action OnUnitsUpdate;

        private void Start()
        {
            m_action.Enable();
            m_action.started += (p_ctx) =>
            {
                foreach (var (l_positionType, l_unitSlot) in m_units)
                {
                    Debug.Log(!l_unitSlot.isActive
                                  ? $"[TeamController] {l_positionType} : {l_unitSlot.isActive}"
                                  : $"[TeamController] {l_positionType} : {l_unitSlot.isActive} -> {l_unitSlot.unitInstance.name}");
                }
            };
        }
        
        private void Update()
        {
        }
        
        public void InitRoaster(AttackType p_frontLeftType, 
                                AttackType p_frontRightType, 
                                AttackType p_backLeftType,
                                AttackType p_backRightType)
        {
            m_units = new Dictionary<PositionType, UnitSlot>
            {
                { PositionType.FRONT_LEFT, new UnitSlot() },
                { PositionType.FRONT_RIGHT, new UnitSlot() },
                { PositionType.BACK_LEFT, new UnitSlot() },
                { PositionType.BACK_RIGHT, new UnitSlot() }
            };

            this.InstantiateUnit(p_frontLeftType, PositionType.FRONT_LEFT);
            this.InstantiateUnit(p_frontRightType, PositionType.FRONT_RIGHT);
            this.InstantiateUnit(p_backLeftType, PositionType.BACK_LEFT);
            this.InstantiateUnit(p_backRightType, PositionType.BACK_RIGHT);
            
            this.OnUnitsUpdate?.Invoke();
        }

        public void Reset()
        {
            foreach (UnitSlot l_slot in m_units.Values)
            {
                if (l_slot.isActive)
                {
                    l_slot.unitInstance.Reset();
                }
            }
        }

        public void Pause()
        {
            foreach (UnitSlot l_slot in m_units.Values)
            {
                if (l_slot.isActive)
                {
                    l_slot.unitInstance.Pause();
                }
            }
        }

        public void SwitchPosition(PositionType p_first, PositionType p_second)
        {
            if (m_units[p_first].isActive && m_units[p_second].isActive)
            {
                m_units[p_first].unitInstance.SetPosition(p_second, this.GetSlot(p_second).position);
                m_units[p_second].unitInstance.SetPosition(p_first, this.GetSlot(p_first).position);
                (m_units[p_first].unitInstance, m_units[p_second].unitInstance) = (m_units[p_second].unitInstance, m_units[p_first].unitInstance);
                m_units[p_first].unitInstance.Reset();
                m_units[p_second].unitInstance.Reset();
            } else if (m_units[p_first].isActive)
            {
                m_units[p_first].isActive = false;
                m_units[p_second].isActive = true;
                m_units[p_second].unitInstance = m_units[p_first].unitInstance;
                m_units[p_second].unitInstance.SetPosition(p_second, this.GetSlot(p_second).position);
                m_units[p_second].unitInstance.Reset();
                m_units[p_first].unitInstance = null;
            }
            else if (m_units[p_second].isActive)
            {
                m_units[p_second].isActive = false;
                m_units[p_first].isActive = true;
                m_units[p_first].unitInstance = m_units[p_second].unitInstance;
                m_units[p_first].unitInstance.SetPosition(p_first, this.GetSlot(p_first).position);
                m_units[p_first].unitInstance.Reset();
                m_units[p_second].unitInstance = null;
            }
            
            this.OnUnitsUpdate?.Invoke();
        }
        
        public void Tick(BossController p_bossController)
        {
            foreach (UnitSlot l_slot in m_units.Values)
            {
                if (l_slot.isActive)
                {
                    l_slot.unitInstance.Tick(this, p_bossController);
                }
            }
        }

        public void DamageTeamMember(PositionType[] p_positions, DamagePayload p_payload)
        {
            foreach (PositionType l_positionType in p_positions)
            {
                this.DamageTeamMember(l_positionType, p_payload);
            }
        }

        public void DamageTeamMember(PositionType p_position, DamagePayload p_payload)
        {
            UnitSlot l_unit = m_units[p_position];

            if (!l_unit.isActive)
            {
                if (this.HasFallbackStrategy(p_position, out PositionType l_fallbackPos))
                {
                    this.DamageTeamMember(l_fallbackPos, p_payload);
                }
            }
            else
            {
                l_unit.unitInstance.TakeDamage(p_payload);
            }
        }

        private Transform GetSlot(PositionType p_position)
        {
            switch (p_position)
            {
                case PositionType.FRONT_LEFT:
                    return m_slotFrontLeft;
                case PositionType.FRONT_RIGHT:
                    return m_slotFrontRight;
                case PositionType.BACK_LEFT:
                    return m_slotBackLeft;
                case PositionType.BACK_RIGHT:
                    return m_slotBackRight;
                default:
                    return null;
            }
        }

        private void InstantiateUnit(AttackType p_type, PositionType p_position)
        {
            UnitSlot l_slot = m_units[p_position];

            UnitEntry l_entry = m_unitDatabase.GetUnityEntry(p_type);
            UnitBrain l_instance = Instantiate(l_entry.unitPrefab, RandomPointInBounds(m_spawnArea), Quaternion.identity, this.GetSlot(p_position));
            l_instance.Init(l_entry.statsAsset);
            l_instance.SetPosition(p_position, this.GetSlot(p_position).position);
            l_instance.OnDeath += this.OnUnitDeath;
            l_slot.unitInstance = l_instance;
            l_slot.isActive = true;
        }

        private void OnUnitDeath(PositionType p_position)
        {
            UnitSlot l_slot = m_units[p_position];

            if (!l_slot.isActive)
            {
                Debug.LogError($"TeamController::OnUnitDeath : [{p_position}] unit death message received but slot is not used.");
                return;
            }

            l_slot.isActive = false;
            this.OnUnitDie?.Invoke(l_slot.unitInstance.Type);
            Destroy(l_slot.unitInstance.gameObject);
            l_slot.unitInstance = null;

            this.OnUnitsUpdate?.Invoke();
        }

        private bool HasFallbackStrategy(PositionType p_position, out PositionType p_outputPos)
        {
            if (p_position == PositionType.FRONT_LEFT || p_position == PositionType.FRONT_RIGHT)
            {
                p_outputPos = p_position == PositionType.FRONT_LEFT ? PositionType.BACK_LEFT : PositionType.BACK_RIGHT;
                return true;
            }

            p_outputPos = default;
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(m_slotFrontLeft.position, Vector3.one);
            Gizmos.DrawWireCube(m_slotFrontRight.position, Vector3.one);
            Gizmos.DrawWireCube(m_slotBackLeft.position, Vector3.one);
            Gizmos.DrawWireCube(m_slotBackRight.position, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(m_spawnArea.center, m_spawnArea.extents);
        }
        
        public static Vector3 RandomPointInBounds(Bounds p_bounds) {
            return new Vector3(
                Random.Range(p_bounds.min.x, p_bounds.max.x),
                Random.Range(p_bounds.min.y, p_bounds.max.y),
                Random.Range(p_bounds.min.z, p_bounds.max.z)
            );
        }
    }
}