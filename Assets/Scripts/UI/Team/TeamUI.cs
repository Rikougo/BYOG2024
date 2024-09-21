using TrashBoat.Core;
using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.UI.Team
{
    public class TeamUI : MonoBehaviour
    {
        [SerializeField] private TeamTileUI[] m_tiles;
        [SerializeField] private TeamController m_teamController;

        private bool m_hasClicked;
        private PositionType m_firstClicked;

        private void OnEnable()
        {
            foreach (TeamTileUI l_tile in m_tiles)
            {
                l_tile.OnClicked += this.OnTileClicked;
            }
        }

        private void OnDisable()
        {
            foreach (TeamTileUI l_tile in m_tiles)
            {
                l_tile.OnClicked -= this.OnTileClicked;
            }
        }

        private void OnTileClicked(PositionType p_position)
        {
            if (m_hasClicked)
            {
                m_hasClicked = false;
                if (p_position != m_firstClicked)
                {
                    Debug.Log($"Switch {m_firstClicked} & {p_position}");
                    m_teamController.SwitchPosition(m_firstClicked, p_position);
                }
            }
            else
            {
                m_hasClicked = true;
                m_firstClicked = p_position;
            }
        }
    }
}