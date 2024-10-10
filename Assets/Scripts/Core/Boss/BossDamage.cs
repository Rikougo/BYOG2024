using TMPro;
using TrashBoat.Core.Units;
using UnityEngine;

namespace TrashBoat.Core.Boss
{
	public class BossDamage : MonoBehaviour
	{
		[SerializeField] private BossController m_bossController;
		[SerializeField] private Transform m_rootText;

		private void OnEnable()
		{
			m_bossController.OnDamageReceived += OnBossDamageReceived;
		}

		private void OnBossDamageReceived(float p_amount, bool p_isArmor, AttackType p_type)
		{
			var l_lastChild = m_rootText.GetChild(m_rootText.childCount - 1);
			l_lastChild.gameObject.SetActive(true);
			var l_lastText = l_lastChild.GetComponent<TextMeshProUGUI>();
			l_lastText.text = $"- {p_amount:F2}";
			l_lastText.color = UnitBrain.DevGetColor(p_type);
			l_lastText.transform.SetSiblingIndex(0);
		}
	}
}