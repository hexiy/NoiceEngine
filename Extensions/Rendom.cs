using System;

namespace Engine
{
	public static class Rendom
	{
		public static Random rnd = new Random();

		public static int Range(int min, int max)
		{
			return rnd.Next(max - min) + min;
		}
		public static float Range(float max)
		{
			return (float)rnd.NextDouble() * max;
		}
		public static float Range(float min, float max)
		{
			return ((float)rnd.NextDouble() * (max - min)) + min;
		}
	}
}
