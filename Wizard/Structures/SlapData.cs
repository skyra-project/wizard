namespace Wizard.Structures
{
	public readonly ref struct SlapData
	{
		public readonly string FirstUser;
		public readonly string SecondUser;

		public SlapData(string firstUser, string secondUser)
		{
			FirstUser = firstUser;
			SecondUser = secondUser;
		}
	}
}
