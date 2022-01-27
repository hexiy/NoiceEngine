using Scripts;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

		//public static GLFW.InputState MouseButton1State;
		//public static GLFW.InputState MouseButton2State;
		//public static GLFW.InputState MouseButton3State;

		public static Vector2 Delta;
		public static Vector2 Position = Vector2.Zero;

		//
		// Summary:
		//     Specifies the buttons of a mouse.
		public enum Buttons
		{
			//
			// Summary:
			//     The first button.
			Button1 = 0,
			//
			// Summary:
			//     The second button.
			Button2 = 1,
			//
			// Summary:
			//     The third button.
			Button3 = 2,
			//
			// Summary:
			//     The fourth button.
			Button4 = 3,
			//
			// Summary:
			//     The fifth button.
			Button5 = 4,
			//
			// Summary:
			//     The sixth button.
			Button6 = 5,
			//
			// Summary:
			//     The seventh button.
			Button7 = 6,
			//
			// Summary:
			//     The eighth button.
			Button8 = 7,
			//
			// Summary:
			//     The left mouse button. This corresponds to OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button1.
			Left = 0,
			//
			// Summary:
			//     The right mouse button. This corresponds to OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button2.
			Right = 1,
			//
			// Summary:
			//     The middle mouse button. This corresponds to OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button3.
			Middle = 2,
			//
			// Summary:
			//     The highest mouse button available.
			Last = 7
		}

		public static bool IsButtonDown(Buttons button)
		{
			return Scene.I.MouseState.IsButtonDown((MouseButton)button);
		}
		public static bool IsButtonUp(Buttons button)
		{
			return (Scene.I.MouseState.IsButtonDown((MouseButton)button) == false);
		}

		public static bool ButtonPressed(Buttons button)
		{
			return Scene.I.MouseState.WasButtonDown((MouseButton)button) == false && (Scene.I.MouseState.IsButtonDown((MouseButton)button));
		}
		public static bool ButtonReleased(Buttons button)
		{
			return Scene.I.MouseState.WasButtonDown((MouseButton)button) && (Scene.I.MouseState.IsButtonDown((MouseButton)button) == false);
		}
		public static void Update()
		{
			MouseState state = Scene.I.MouseState;
			// Middle Button
			/*if (MouseButton3State == GLFW.InputState.Release && state == GLFW.InputState.Press)
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
*/


			//Delta = Editor.ScreenToWorld(state.Position.ToVector2()) - Position;
			//Position = Editor.ScreenToWorld(state.Position.ToVector2());
			Delta = new Vector2(state.Delta.X, -state.Delta.Y);
			Position = new Vector2(Scene.I.MouseState.X, Camera.I.Size.Y - Scene.I.MouseState.Y);
		}
	}
}
