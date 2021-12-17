using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
	public class CharacterController : Component
	{
		[ShowInEditor] public float MoveSpeed { get; set; } = 10;
		[ShowInEditor] public float JumpForce { get; set; } = 10000;
		[LinkableComponent]
		private Rigidbody rb;
		bool jumpKeyDown = false;
		public override void Awake()
		{
			rb = GetComponent<Rigidbody>();
			base.Awake();
		}

		public override void FixedUpdate()
		{
			if (rb == null) return;
			Vector2 input = Vector2.Zero;
			if (KeyboardInput.IsKeyDown(Keys.A))
			{
				input.X = -MoveSpeed;
			}
			if (KeyboardInput.IsKeyDown(Keys.D))
			{
				input.X = MoveSpeed;
			}

			if (jumpKeyDown == false && KeyboardInput.state.IsKeyDown(Keys.W))
			{
				rb.body.ApplyForce(new Vector2(0, -JumpForce));
			}
			jumpKeyDown = KeyboardInput.state.IsKeyDown(Keys.W);
			rb.body.ApplyForce(new Vector2(input.X, 0));


			base.Update();
		}
	}
}
