namespace TrashBoat.Core
{
	public enum PositionType
	{
		FRONT_LEFT,
		FRONT_RIGHT,
		BACK_LEFT,
		BACK_RIGHT
	}

	public static class PositionTypeHelper
	{
		public static PositionType[] GetAll()
		{
			return new[]
			{
				PositionType.FRONT_LEFT,
				PositionType.FRONT_RIGHT,
				PositionType.BACK_LEFT,
				PositionType.BACK_RIGHT
			};
		}

		public static PositionType[] GetFront()
		{
			return new[]
			{
				PositionType.FRONT_LEFT,
				PositionType.FRONT_RIGHT
			};
		}
	}
}