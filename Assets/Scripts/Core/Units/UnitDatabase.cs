using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TrashBoat.Core.Units
{
    [CreateAssetMenu(fileName = "UnitDatabase", menuName = "BYOG/Create unit database", order = 0)]
    public class UnitDatabase : ScriptableObject
    {
        [SerializeField] private AttackStatsDictionary m_units;

        public UnitEntry GetUnityEntry(AttackType p_type)
        {
            return m_units[p_type];
        }
    }

    [Serializable]
    public class UnitEntry
    {
        public UnitStats statsAsset;
        public UnitBrain unitPrefab;
        public Sprite icon;
    }

    [Serializable]
    public class AttackStatsDictionary : SerializableDictionary<AttackType, UnitEntry> {}
}