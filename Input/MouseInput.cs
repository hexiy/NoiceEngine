using Scripts;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine
{
	public static class MouseInput
	{

		public delegate void MouseEvent();

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

		public static bool IsButtonDown(Buttons button = MouseInput.Buttons.Left)
		{
			return Window.I.MouseState.IsButtonDown((MouseButton)button);
		}
		public static bool IsButtonUp(Buttons button = MouseInput.Buttons.Left)
		{
			return (Window.I.MouseState.IsButtonDown((MouseButton)button) == false);
		}

		public static bool ButtonPressed(Buttons button = MouseInput.Buttons.Left)
		{
			return Window.I.MouseState.WasButtonDown((MouseButton)button) == false && (Window.I.MouseState.IsButtonDown((MouseButton)button));
		}
		public static bool ButtonReleased(Buttons button = MouseInput.Buttons.Left)
		{
			return Window.I.MouseState.WasButtonDown((MouseButton)button) && (Window.I.MouseState.IsButtonDown((MouseButton)button) == false);
		}
		public static void Update()
		{
			MouseState state = Window.I.MouseState;

			Delta = new Vector2(state.Delta.X, -state.Delta.Y);
			Position = new Vector2(Window.I.MouseState.X, -Window.I.MouseState.Y + Camera.I.size.Y);
			//System.Diagnostics.Debug.WriteLine("mousePos:" + Position.X + ":" + Position.Y);
		}
	}
}
