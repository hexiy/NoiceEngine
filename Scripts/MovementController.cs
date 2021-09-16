using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Engine;
using KeyboardInput = Engine.KeyboardInput;

namespace Scripts
{
	public class MovementController : Component
	{
		[ShowInEditor]
		public float MoveSpeed { get; set; } = 10;
		[LinkableComponent]
		private Rigidbody rb;

		public override void Awake()
		{
			rb = GetComponent<Rigidbody>();
			base.Awake();
		}

		public override void Update()
		{
			if (rb == null) return;
			Vector2 input = Vector2.Zero;
			if (KeyboardInput.IsKeyDown(Keys.A))
			{
				input.X = -MoveSpeed * Time.deltaTime;
			}
			if (KeyboardInput.IsKeyDown(Keys.D))
			{
				input.X = MoveSpeed * Time.deltaTime;
			}
			rb.Velocity = new Vector2(input.X, rb.Velocity.Y);
			base.Update();
		}
	}
}
