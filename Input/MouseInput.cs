using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

		public static ButtonState MouseButton1State;
		public static ButtonState MouseButton2State;
		public static ButtonState MouseButton3State;


		public static Vector2 Delta;
		public static Vector2 Position = Vector2.Zero;
		
		public static void Update(MouseState state)
		{

			// Middle Button
			if (MouseButton3State == ButtonState.Released && state.MiddleButton == ButtonState.Pressed)
			{
				Mouse3Down?.Invoke();
			}
			if (MouseButton3State == ButtonState.Pressed && state.MiddleButton == ButtonState.Released)
			{
				Mouse3Up?.Invoke();
			}
			MouseButton3State = state.MiddleButton;

			// Left Button
			if (MouseButton1State == ButtonState.Released && state.LeftButton == ButtonState.Pressed)
			{

				for (int i = 0; i < Scene.I.gameObjects.Count; i++)
				{
					if (Scene.I.gameObjects[i].Active)
					{
						Scene.I.gameObjects[i].mouseOver = MouseInput.Position.In(Scene.I.gameObjects[i].GetComponent<Shape>()).intersects;

					}
				}

				Mouse1Down?.Invoke();
			}
			if (MouseButton1State == ButtonState.Pressed && state.LeftButton == ButtonState.Released)
			{
				Mouse1Up?.Invoke();
			}
			MouseButton1State = state.LeftButton;

			// Right Button
			if (MouseButton2State == ButtonState.Released && state.RightButton == ButtonState.Pressed)
			{
				Mouse2Down?.Invoke();
			}
			if (MouseButton2State == ButtonState.Pressed && state.RightButton == ButtonState.Released)
			{
				Mouse2Up?.Invoke();
			}
			MouseButton2State = state.RightButton;

			if (MouseButton1State == ButtonState.Pressed)
			{
				Mouse1?.Invoke();
			}
			if (MouseButton2State == ButtonState.Pressed)
			{
				Mouse2?.Invoke();
			}



			Delta = state.Position.ToVector2() - Position;

			Position = Editor.ScreenToViewport(state.Position.ToVector2());
		}
	}
}
