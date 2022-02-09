namespace Scripts;

public class CharacterController : Component
{
	public float moveSpeed = 10;
	public float jumpForce = 10000;

	// LINKABLECOMPONENT PURGE [LinkableComponent]
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
		if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.A))
		{
			input.X = -moveSpeed;
		}
		if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.D))
		{
			input.X = moveSpeed;
		}

		if (jumpKeyDown == false && KeyboardInput.IsKeyDown(KeyboardInput.Keys.W))
		{
			//rb.body.ApplyForce(new Vector2(0, -JumpForce));
		}
		jumpKeyDown = KeyboardInput.IsKeyDown(KeyboardInput.Keys.W);
		//rb.body.ApplyForce(new Vector2(input.X, 0));
		base.Update();
	}
}
