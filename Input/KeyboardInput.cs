using Scripts;

namespace Engine
{
	public static class KeyboardInput
	{
		public static bool IsKeyDown(GLFW.Keys key)
		{
			return GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.S) == GLFW.InputState.Press;
		}
		public static bool IsKeyUp(GLFW.Keys key)
		{
			return GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.S) == GLFW.InputState.Release;
		}
	}
}
