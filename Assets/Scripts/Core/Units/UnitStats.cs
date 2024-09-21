using System;
using UnityEngine;

namespace TrashBoat.Core.Units
{
    [CreateAssetMenu(menuName = "BYOG/Create unit asset", fileName = "UnitAsset")]
    public class UnitStats : ScriptableObject
    {
        [SerializeField] private float m_health;
        [SerializeField] private AttackType m_attackType;

        public float Health => m_health;
        public AttackType AttackType => m_attackType;
    }
}