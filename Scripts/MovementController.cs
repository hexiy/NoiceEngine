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
		[ShowInEditor] public float MoveSpeed { get; set; } = 10;
		[ShowInEditor] public float JumpForce { get; set; } = 10000;
		[LinkableComponent]
		private Rigidbody rb;
		bool jumpKeyDown = false;

		[LinkableComponent] private AnimationController animationController;
		public override void Awake()
		{
			rb = GetComponent<Rigidbody>();
			animationController = GetComponent<AnimationController>();

			base.Awake();
		}

		public override void FixedUpdate()
		{
			if (rb == null) return;

			bool pressedLeft = KeyboardInput.IsKeyDown(Keys.A);
			bool pressedRight = KeyboardInput.IsKeyDown(Keys.D);

			if (pressedLeft || pressedRight)
			{
				animationController.SetAnimRange(AnimationController.AnimState.Run);
			}
			else
			{
				animationController.SetAnimRange(AnimationController.AnimState.Idle);
			}

			Vector2 input = Vector2.Zero;
			if (pressedLeft)
			{
				input.X = -MoveSpeed;
				animationController.Turn(Vector2.Left);
			}
			else if (pressedRight)
			{
				input.X = MoveSpeed;
				animationController.Turn(Vector2.Right);
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
