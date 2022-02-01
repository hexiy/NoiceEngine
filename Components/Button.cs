

using Scripts;
using Engine.UI;
using System.Xml.Serialization;

namespace Engine
{
	public class Button : Component
	{
		public delegate void MouseAction();
		[XmlIgnore] MouseAction onClickedAction;
		[XmlIgnore] public MouseAction onReleasedAction;

		[LinkableComponent]
		public Renderer renderer;
		[LinkableComponent]
		public Shape collider;
		bool mouseIsOver = false;
		bool clicked = false;
		public override void Awake()
		{
			onClickedAction += () => renderer.color = new Color(215, 125, 125);
			onReleasedAction += () => renderer.color = Color.White;
			base.Awake();


			GameObject.AddComponent<ButtonTween>().Awake();
		}

		public override void Update()
		{
			if (renderer == false || collider == false) { return; }
			if (MouseInput.ButtonPressed())
			{
				onClickedAction?.Invoke();
				clicked = true;

			}
			else if (MouseInput.ButtonReleased())
			{
				onReleasedAction?.Invoke();
				clicked = false;
			}
			mouseIsOver = Camera.I.ScreenToWorld(MouseInput.ScreenPosition).In(collider);

			if (clicked == false)
			{
				renderer.color = mouseIsOver ? Color.Gray : Color.White;
			}

			if (clicked && mouseIsOver == false) // up event when me move out of button bounds, even when clicked
			{
				onReleasedAction?.Invoke();
				clicked = false;
			}
		}
	}
}
