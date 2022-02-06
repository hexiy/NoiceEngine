using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
	public class BlendingLightAnimation : Component
	{
		public float speed = 1;
		public override void Update()
		{
			//transform.position = Vector2.Lerp(transform.position, MouseInput.WorldPosition + new Vector2((float)Math.Cos(Time.elapsedTime * speed), (float)Math.Sin(Time.elapsedTime * speed)) * (MathF.Cos(Time.elapsedTime + speed) * 25 + 50), Time.deltaTime*3);
			transform.position = MouseInput.WorldPosition + new Vector2((float)Math.Cos(Time.elapsedTime * speed), (float)Math.Sin(Time.elapsedTime * speed)) * (MathF.Cos(Time.elapsedTime + speed) * 25 + 50);

			base.Update();
		}
	}
}
