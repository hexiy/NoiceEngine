using Microsoft.Xna.Framework;

namespace Engine
{
	public static class Time
	{
		public static float deltaTime = 0.01666666f;
		public static float fixedDeltaTime = 0.01666666f;
		public static float elapsedTime = 0f;
		public static ulong elapsedTicks = 0;
		public static ulong timeScale = 0;

		public static void Update(GameTime gameTime)
		{
			deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			elapsedTime += deltaTime;
			elapsedTicks++;

		}
	}
}
