using System;

namespace TrashBoat
{
	/// <summary>
	///   Flag type enum to specify behaviour on multiple gamestate
	/// </summary>
	[Flags]
	public enum GameState
	{
		MAIN_MENU = 1 << 0,
		GAME = 1 << 1,
		SELECTION = 1 << 2,
		GAME_OVER = 1 << 3
	}
}