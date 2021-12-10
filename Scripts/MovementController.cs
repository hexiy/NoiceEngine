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
		private Vector2 targetVelocity = Vector2.Zero;
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
				input.X = -MoveSpeed * Time.deltaTime;
			}
			if (KeyboardInput.IsKeyDown(Keys.D))
			{
				input.X = MoveSpeed * Time.deltaTime;
			}
			if (jumpKeyDown == false && KeyboardInput.state.IsKeyDown(Keys.W))
			{
				transform.position.Y -= 10f;
				rb.Velocity = new Vector2(rb.Velocity.X, -450);
				//rb.ApplyVelocity(new Vector2(0, -450));
			}
			jumpKeyDown = KeyboardInput.state.IsKeyDown(Keys.W);

			targetVelocity = Vector2.Lerp(targetVelocity, input, Time.deltaTime * 5);

			rb.Velocity = new Vector2(targetVelocity.X, rb.Velocity.Y);
			base.Update();
		}
	}
}
