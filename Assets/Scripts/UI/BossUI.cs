﻿using TMPro;
using TrashBoat.Core.Boss;
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
		[SerializeField] private TextMeshProUGUI m_levelText;

		private void Start()
		{
			m_bossController.StatsUpdated += BossStatsUpdated;
		}

		private void Update()
		{
			m_levelText.text = $"{m_bossController.CurrentLevel}";
		}

		private void BossStatsUpdated(BossStats p_current, BossStats p_ref)
		{
			var l_healthPercent = p_current.health / p_ref.health;
			var l_armorPercent = p_current.armor / p_ref.health;

			m_healthFill.fillAmount = l_healthPercent;
			m_armorFill.fillAmount = l_armorPercent;

			m_healthText.text = l_armorPercent > 0
				                    ? $"{Mathf.Max(p_current.health, 0.0f):F0} (+{p_current.armor:F0})"
				                    : $"{Mathf.Max(p_current.health, 0.0f):F0}";
		}
	}
}