using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Scripts;

namespace Engine
{
	public static class KeyboardInput
	{

		public static KeyboardState state;
		private static ulong lastTick = 0;

		private static KeyboardState GetState()
		{
			if (Time.elapsedTicks != lastTick)
			{
				state = Keyboard.GetState();
			}
			return state;
		}

		public static bool IsKeyDown(Keys key)
		{
			return GetState().IsKeyDown(key);
		}
		public static bool IsKeyUp(Keys key)
		{
			return GetState().IsKeyUp(key);
		}

	}
}
