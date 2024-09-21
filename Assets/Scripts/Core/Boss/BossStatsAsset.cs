using UnityEngine;

namespace TrashBoat.Core
{
    [CreateAssetMenu(fileName="BossStats", menuName = "BYOG/Create boss stats")]
    public class BossStatsAsset : ScriptableObject
    {
        [Header("Base stats")]
        [SerializeField] private float m_baseHealth;
        [SerializeField] private float m_baseDamage;
        [SerializeField] private float m_baseHaste;
        [Header("Growth stats")]
        [SerializeField] private float m_healthGrowth;
        [SerializeField] private float m_damageGrowth;
        [SerializeField] private float m_hasteGrowth;

        public BossStats ComputeStats(int p_level)
        {
            return new BossStats()
            {
                health = m_baseHealth + p_level * m_healthGrowth,
                damage = m_baseDamage + p_level * m_damageGrowth,
                haste = m_baseHaste + p_level * m_hasteGrowth,
                armor = 0.0f,
                hasShield = false
            };
        }
    }

    public struct BossStats
    {
        public float health;
        public float armor;
        public bool hasShield;
        public float damage;
        public float haste;
    }
}