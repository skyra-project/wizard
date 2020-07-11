using System;

namespace Wizard.Structures
{
	public static class Utils
	{
		/// <summary>
		///     Converts from degrees to radians.
		/// </summary>
		/// <param name="degrees"> An angle in degrees.</param>
		/// <returns>An angle in radians.</returns>
		public static double DegreesToRadians(double degrees)
		{
			return Math.PI / 180 * degrees;
		}
	}
}
