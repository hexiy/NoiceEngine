using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
	public class AnimationController : Component
	{
		private SpriteSheetRenderer spriteSheetRenderer;
		public enum AnimState { Idle, Run };
		[ShowInEditor] public Vector2 AnimRange_Idle { get; set; } = new Vector2(0, 0);
		[ShowInEditor] public Vector2 AnimRange_Run { get; set; } = new Vector2(0, 0);

		[ShowInEditor] public Vector2 CurrentAnimRange { get; set; } = new Vector2(0, 0);

		public override void Awake()
		{
			spriteSheetRenderer = GetComponent<SpriteSheetRenderer>();
			base.Awake();
		}
		public override void Start()
		{
			SetAnimRange(AnimState.Idle);

			base.Start();
		}
		public override void Update()
		{
			//spriteSheetRenderer.FrameRange = CurrentAnimRange;

			base.Update();
		}

		public void Turn(Vector2 direction)
		{
			if (direction == Vector2.Right)
			{
				transform.rotation.Y = 0;
			}
			else
			{
				transform.rotation.Y = 180;
			}
		}
		public void SetAnimRange(AnimState state)
		{
			Vector2 oldAnim = CurrentAnimRange;
			switch (state)
			{
				case AnimState.Idle:
					CurrentAnimRange = AnimRange_Idle;
					break;
				case AnimState.Run:
					CurrentAnimRange = AnimRange_Run;
					break;
				default:
					break;
			}
			spriteSheetRenderer.FrameRange = CurrentAnimRange;

			if (oldAnim != CurrentAnimRange)
			{
				spriteSheetRenderer.ResetCurrentAnimation();
			}
		}

	}
}
