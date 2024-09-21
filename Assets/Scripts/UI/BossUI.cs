using TMPro;
using TrashBoat.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TrashBoat.UI
{
    public class BossUI : MonoBehaviour
    {
        [SerializeField] private BossController m_bossController;
        
        [SerializeField] private Image m_healthFill;
        [SerializeField] private Image m_armorFill;
        [SerializeField] private TextMeshProUGUI m_healthText;

        private void Start()
        {
            m_bossController.StatsUpdated += this.BossStatsUpdated;
        }

        private void BossStatsUpdated(BossStats p_current, BossStats p_ref)
        {
            float l_healthPercent = p_current.health / p_ref.health;
            float l_armorPercent = p_current.armor / p_ref.health;

            m_healthFill.fillAmount = l_healthPercent;
            m_armorFill.fillAmount = l_armorPercent;

            m_healthText.text = l_armorPercent > 0 ? 
                                    $"{Mathf.Max(p_current.health, 0.0f):F0} (+{p_current.armor:F0})" : 
                                    $"{Mathf.Max(p_current.health, 0.0f):F0}";
        }
    }
}