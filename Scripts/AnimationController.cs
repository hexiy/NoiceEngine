using Engine;

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

		[ShowInEditor] public Vector2 AnimRange_Idle { get; set; } = new Vector2(0, 0);
		[ShowInEditor] public Vector2 AnimRange_Run { get; set; } = new Vector2(0, 0);
		[ShowInEditor] public Vector2 AnimRange_Jump { get; set; } = new Vector2(0, 0);
		private Action OnAnimationFinished = new Action(() => { });
		[ShowInEditor] public Vector2 CurrentAnimRange { get; set; } = new Vector2(0, 0);
		[ShowInEditor]
		public float AnimationSpeed { get; set; } = 1;
		private float timeOnCurrentFrame = 0;


		public bool jumping = false;

		public override void Awake()
		{
			spriteSheetRenderer = GetComponent<SpriteSheetRenderer>();
			base.Awake();
		}
		public override void Start()
		{
			SetAnimation(AnimRange_Idle);

			base.Start();
		}
		public override void Update()
		{
			if (AnimationSpeed == 0) return;
			timeOnCurrentFrame += Time.deltaTime * AnimationSpeed;
			while (timeOnCurrentFrame > 1 / AnimationSpeed)
			{
				timeOnCurrentFrame -= 1 / AnimationSpeed;
				if (spriteSheetRenderer.CurrentSpriteIndex + 1 >= CurrentAnimRange.Y)
				{
					spriteSheetRenderer.CurrentSpriteIndex = (int)CurrentAnimRange.X;
					OnAnimationFinished?.Invoke();
				}
				else
				{
					spriteSheetRenderer.CurrentSpriteIndex++;
				}
			}
			base.Update();
		}
		public void ResetCurrentAnimation()
		{
			timeOnCurrentFrame = 0;
			spriteSheetRenderer.CurrentSpriteIndex = (int)CurrentAnimRange.X;
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
		public void Jump()
		{
			jumping = true;
			SetAnimation(AnimRange_Jump);
			AnimationSpeed = 4.5f;
			OnAnimationFinished += () =>
			{
				SetAnimation(AnimRange_Run);
				AnimationSpeed = 3;

				jumping = false;
				OnAnimationFinished = new Action(() => { });
			};
		}
		public void SetAnimation(Vector2 animRange)
		{
			if (jumping && animRange != AnimRange_Jump) { return; }

			Vector2 oldAnim = CurrentAnimRange;

			CurrentAnimRange = animRange;

			if (oldAnim != CurrentAnimRange)
			{
				ResetCurrentAnimation();
			}
		}

	}
}
