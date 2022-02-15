namespace Scripts;

public class AnimationController : Component
{
	private SpriteSheetRenderer spriteSheetRenderer;

	public Vector2 animRange_Idle = new Vector2(0, 0);
	public Vector2 animRange_Run = new Vector2(0, 0);
	public Vector2 animRange_Jump = new Vector2(0, 0);
	public Vector2 currentAnimRange = new Vector2(0, 0);

	public float animationSpeed = 1;
	private float timeOnCurrentFrame = 0;
	public bool jumping = false;

	private Action OnAnimationFinished = new Action(() => { });

	public override void Awake()
	{
		spriteSheetRenderer = GetComponent<SpriteSheetRenderer>();
		base.Awake();
	}
	public override void Start()
	{
		SetAnimation(animRange_Idle);

		base.Start();
	}
	public override void Update()
	{
		if (animationSpeed == 0) return;
		timeOnCurrentFrame += Time.deltaTime * animationSpeed;
		while (timeOnCurrentFrame > 1 / animationSpeed)
		{
			timeOnCurrentFrame -= 1 / animationSpeed;
			if (spriteSheetRenderer.currentSpriteIndex + 1 >= currentAnimRange.Y)
			{
				spriteSheetRenderer.currentSpriteIndex = (int)currentAnimRange.X;
				OnAnimationFinished?.Invoke();
			}
			else
			{
				spriteSheetRenderer.currentSpriteIndex++;
			}
		}
		base.Update();
	}
	public void ResetCurrentAnimation()
	{
		timeOnCurrentFrame = 0;
		spriteSheetRenderer.currentSpriteIndex = (int)currentAnimRange.X;
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
		SetAnimation(animRange_Jump);
		animationSpeed = 4.5f;
		OnAnimationFinished += () =>
		{
			SetAnimation(animRange_Run);
			animationSpeed = 3;

			jumping = false;
			OnAnimationFinished = new Action(() => { });
		};
	}
	public void SetAnimation(Vector2 animRange)
	{
		if (jumping && animRange != animRange_Jump) { return; }

		Vector2 oldAnim = currentAnimRange;

		currentAnimRange = animRange;

		if (oldAnim != currentAnimRange)
		{
			ResetCurrentAnimation();
		}
	}

}
