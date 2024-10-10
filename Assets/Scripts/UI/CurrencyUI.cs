using TMPro;
using TrashBoat.Core;
using UnityEngine;

namespace TrashBoat.UI
{
	public class CurrencyUI : MonoBehaviour
	{
		[SerializeField] private GameController m_gameController;
		[SerializeField] private TextMeshProUGUI m_currencyText;

		private void Update()
		{
			m_currencyText.text = $"{m_gameController.Currency:F0}";
		}
	}
}