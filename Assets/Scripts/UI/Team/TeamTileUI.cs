using System;
using TrashBoat.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrashBoat.UI.Team
{
    public class TeamTileUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PositionType m_position;
        [SerializeField] private Image m_iconImage;

        public PositionType Position => m_position;
        
        public event Action<PositionType> OnClicked;

        public void OnPointerClick(PointerEventData p_eventData)
        {
            Debug.Log($"[{name}] Clicked {m_position}");
            this.OnClicked?.Invoke(m_position);
        }

        public void SetSprite(Sprite p_icon)
        {
            if (p_icon == null)
            {
                m_iconImage.enabled = false;
            }
            else
            {
                m_iconImage.enabled = true;
                m_iconImage.sprite = p_icon;
            }
        }
    }
}