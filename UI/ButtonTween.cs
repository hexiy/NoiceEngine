using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

using Scripts;
namespace Engine.UI
{
	public class ButtonTween : Component
	{
		private bool clicked = false;
		private float scaleSpeed = 20;
		private float scaleTarget = 0.9f;
		private bool needToScale = false;
		public override void Awake()
		{
			Button btn = GetComponent<Button>();

			base.Awake();
		}
		public override void Update()
		{
			if (needToScale == false) { return; }

			if (MouseInput.ButtonPressed())
			{
				clicked = true;
				needToScale = true;
			}
			else if (MouseInput.ButtonReleased())
			{
				transform.scale = Vector3.One * scaleTarget;

				clicked = false;
				needToScale = true;
			}

			if (clicked)
			{
				transform.scale = Vector3.Lerp(transform.scale, Vector3.One * scaleTarget, Time.deltaTime * scaleSpeed);
				if (transform.scale == Vector3.One * scaleTarget)
				{
					needToScale = false;
				}
			}
			else
			{
				transform.scale = Vector3.Lerp(transform.scale, Vector3.One, Time.deltaTime * scaleSpeed);
				if (transform.scale == Vector3.One)
				{
					needToScale = false;
				}
			}
		}
	}
}
