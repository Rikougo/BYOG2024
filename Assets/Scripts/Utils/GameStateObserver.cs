using UnityEngine;

namespace TrashBoat.Utils
{
	public class GameStateObserver : MonoBehaviour
	{
		[SerializeField] private GameManager m_gameManager;
		[SerializeField] private GameState m_gameState;

		private void Awake()
		{
			m_gameManager.GameStateChanged += UpdateObjectState;
			UpdateObjectStateInternal(m_gameManager.CurrentState);
		}

		private void UpdateObjectState(GameState p_new, GameState p_previous)
		{
			UpdateObjectStateInternal(p_new);
		}

		private void UpdateObjectStateInternal(GameState p_new)
		{
			gameObject.SetActive(m_gameState.HasFlag(p_new));
		}
	}
}