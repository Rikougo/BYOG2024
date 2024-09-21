using System;
using TrashBoat.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrashBoat.UI.Team
{
    public class TeamTileUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PositionType m_position;

        public event Action<PositionType> OnClicked;

        public void OnPointerClick(PointerEventData p_eventData)
        {
            Debug.Log($"[{name}] Clicked {m_position}");
            this.OnClicked?.Invoke(m_position);
        }
    }
}