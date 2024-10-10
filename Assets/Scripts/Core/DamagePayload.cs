namespace TrashBoat.Core
{
	public struct DamagePayload
	{
		public bool isBoss;
		public AttackType damageType;
		public float damageAmount;

		public override string ToString()
		{
			return $"(DamagePayload:{isBoss};{damageAmount};{damageType})";
		}
	}
}