

using Scripts;

namespace Engine
{
	public static class MouseInput
	{

		public delegate void MouseEvent();

		public static MouseEvent Mouse1Down;
		public static MouseEvent Mouse1Up;
		public static MouseEvent Mouse1;

		public static MouseEvent Mouse2Down;
		public static MouseEvent Mouse2Up;
		public static MouseEvent Mouse2;

		public static MouseEvent Mouse3Down;
		public static MouseEvent Mouse3Up;
		public static MouseEvent Mouse3;

		public static GLFW.InputState MouseButton1State;
		public static GLFW.InputState MouseButton2State;
		public static GLFW.InputState MouseButton3State;


		public static Vector2 Delta;
		public static Vector2 Position = Vector2.Zero;

		public static void Update()
		{
			// Middle Button
			GLFW.InputState state = GLFW.Glfw.GetMouseButton(DisplayManager.Window, GLFW.MouseButton.Middle);
			if (MouseButton3State == GLFW.InputState.Release && state == GLFW.InputState.Press)
			{
				Mouse3Down?.Invoke();
			}
			if (MouseButton3State == GLFW.InputState.Press && state == GLFW.InputState.Release)
			{
				Mouse3Up?.Invoke();
			}
			MouseButton3State = state;


			// Left Button
			state = GLFW.Glfw.GetMouseButton(DisplayManager.Window, GLFW.MouseButton.Left);
			if (MouseButton1State == GLFW.InputState.Release && state == GLFW.InputState.Press)
			{
				for (int i = 0; i < Scene.I.gameObjects.Count; i++)
				{
					if (Scene.I.gameObjects[i].Active)
					{
						Scene.I.gameObjects[i].mouseOver = MouseInput.Position.In(Scene.I.gameObjects[i].GetComponent<Shape>());
					}
				}

				Mouse1Down?.Invoke();
			}
			if (MouseButton1State == GLFW.InputState.Press && state == GLFW.InputState.Release)
			{
				Mouse1Up?.Invoke();
			}
			MouseButton1State = state;



			// Right Button
			state = GLFW.Glfw.GetMouseButton(DisplayManager.Window, GLFW.MouseButton.Right);
			if (MouseButton2State == GLFW.InputState.Release && state == GLFW.InputState.Press)
			{
				Mouse2Down?.Invoke();
			}
			if (MouseButton2State == GLFW.InputState.Press && state == GLFW.InputState.Release)
			{
				Mouse2Up?.Invoke();
			}
			MouseButton2State = state;

			if (MouseButton1State == GLFW.InputState.Press)
			{
				Mouse1?.Invoke();
			}
			if (MouseButton2State == GLFW.InputState.Press)
			{
				Mouse2?.Invoke();
			}



			//Delta = Editor.ScreenToWorld(state.Position.ToVector2()) - Position;
			//Position = Editor.ScreenToWorld(state.Position.ToVector2());
		}
	}
}
