using UnityEngine;

namespace TrashBoat
{
	[CreateAssetMenu(fileName = "GameController", menuName = "BYOG2024/Create game controller", order = 0)]
	public class GameManager : ScriptableObject
	{
		public delegate void GameStateChangedHandler(GameState p_new, GameState p_previous);

		private GameState m_currentState;

		public GameState CurrentState
		{
			get => m_currentState;
			set
			{
				if (value != m_currentState)
				{
					var l_previous = m_currentState;
					m_currentState = value;
					Debug.Log($"[GameManager] Switched to {m_currentState}");
					GameStateChanged?.Invoke(m_currentState, l_previous);
				}
			}
		}

		public event GameStateChangedHandler GameStateChanged;
	}
}