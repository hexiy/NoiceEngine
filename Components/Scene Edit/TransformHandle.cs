namespace Engine;

public class TransformHandle : Component
{
	//public new bool enabled { get { return true; } }
	/*public static TransformHandle instance;
	public static TransformHandle GetInstance()
	{
		return instance;
	}*/
	public enum Axis { X, Y, XY };
	public Axis? CurrentAxisSelected = null;
	public bool clicked = false;

	public static bool objectSelected;
	private Transform selectedTransform;

	public BoxShape boxColliderXY;
	public BoxShape boxColliderX;
	public BoxShape boxColliderY;
	public BoxRenderer boxRendererXY;
	public BoxRenderer boxRendererX;
	public BoxRenderer boxRendererY;

	public TransformHandle() : base()
	{
		//Awoken = false;
		//instance = this;
	}
	public override void Awake()
	{
		objectSelected = false;
		GameObject.updateWhenDisabled = true;

		GameObject.AddComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().isStatic = false;
		GetComponent<Rigidbody>().isButton = true;

		if (GetComponents<BoxRenderer>().Count > 2)
		{
			boxColliderXY = GetComponent<BoxShape>(0);
			boxColliderX = GetComponent<BoxShape>(1);
			boxColliderY = GetComponent<BoxShape>(2);

			boxRendererXY = GetComponent<BoxRenderer>(0);
			boxRendererX = GetComponent<BoxRenderer>(1);
			boxRendererY = GetComponent<BoxRenderer>(2);
		}
		else
		{
			boxColliderXY = GameObject.AddComponent<BoxShape>();
			boxColliderXY.size = new Vector2(15, 15);
			boxColliderXY.offset = new Vector2(5, 5);

			boxColliderX = GameObject.AddComponent<BoxShape>();
			boxColliderX.size = new Vector2(50, 5);
			//boxColliderX.offset = new Vector2(25, 2.5f);

			boxColliderY = GameObject.AddComponent<BoxShape>();
			boxColliderY.size = new Vector2(5, 50);
			//boxColliderY.offset = new Vector2(2.5f, 25);

			boxRendererXY = GameObject.AddComponent<BoxRenderer>();
			boxRendererX = GameObject.AddComponent<BoxRenderer>();
			boxRendererY = GameObject.AddComponent<BoxRenderer>();

			boxRendererXY.color = Color.Orange;
			boxRendererX.color = Color.Red;
			boxRendererY.color = Color.Cyan;

			boxRendererX.boxShape = boxColliderX;
			boxRendererXY.boxShape = boxColliderXY;
			boxRendererY.boxShape = boxColliderY;
		}
		base.Awake();
	}
	private void SetSelectedObjectRigidbodyAwake(bool tgl)
	{
		if (selectedTransform?.HasComponent<Rigidbody>() == true && selectedTransform?.GetComponent<Rigidbody>().body?.Awake == false)
		{
			selectedTransform.GetComponent<Rigidbody>().body.Awake = tgl;
		}
	}
	public override void Update()
	{
		transform.scale = Vector3.One * Camera.I.ortographicSize;

		if (MouseInput.ButtonReleased(MouseInput.Buttons.Left))
		{
			CurrentAxisSelected = null;
		}
		if (MouseInput.ButtonPressed(MouseInput.Buttons.Left))
		{
			clicked = false;
			if (MouseInput.WorldPosition.In(boxColliderX))
			{
				CurrentAxisSelected = Axis.X;
				clicked = true;
			}
			if (MouseInput.WorldPosition.In(boxColliderY))
			{
				CurrentAxisSelected = Axis.Y;
				clicked = true;
			}
			if (MouseInput.WorldPosition.In(boxColliderXY))
			{
				CurrentAxisSelected = Axis.XY;
				clicked = true;
			}
		}

		if (MouseInput.IsButtonDown(MouseInput.Buttons.Left) && GameObject.active && clicked)
		{
			SetSelectedObjectRigidbodyAwake(false);
			Move(MouseInput.WorldDelta);
		}
		else
		{
			SetSelectedObjectRigidbodyAwake(true);
		}

		if (objectSelected == false || selectedTransform == null)
		{
			//GameObject.Active = false;
			return;
		}
		else
		{
			//GameObject.Active = true;
		}

		transform.position = selectedTransform.position;
		if (MouseInput.WorldPosition.In(boxColliderX) || CurrentAxisSelected == Axis.X)
		{
			boxRendererX.color = Color.WhiteSmoke;
		}
		else
		{
			boxRendererX.color = Color.Red;
		}
		if (MouseInput.WorldPosition.In(boxColliderY) || CurrentAxisSelected == Axis.Y)
		{
			boxRendererY.color = Color.WhiteSmoke;
		}
		else
		{
			boxRendererY.color = Color.Cyan;
		}
		if (MouseInput.WorldPosition.In(boxColliderXY) || CurrentAxisSelected == Axis.XY)
		{
			boxRendererXY.color = Color.WhiteSmoke;
		}
		else
		{
			boxRendererXY.color = Color.Orange;
		}
		base.Update();
	}

	public void Move(Vector3 deltaVector)
	{
		Vector3 moveVector = Vector3.Zero;
		switch (CurrentAxisSelected)
		{
			case Axis.X:
				moveVector += deltaVector.VectorX();
				break;
			case Axis.Y:
				moveVector += deltaVector.VectorY();
				break;
			case Axis.XY:
				moveVector += deltaVector;
				break;
		}
		transform.position += moveVector;// we will grab it with offset, soe we want to move it only by change of mouse position
		selectedTransform.position = transform.position;

		for (int i = 0; i < selectedTransform.children.Count; i++)
		{
			selectedTransform.children[i].position += moveVector;
		}

		if (selectedTransform.HasComponent<Rigidbody>() && selectedTransform.GetComponent<Rigidbody>().isButton == false)
		{
			lock (Physics.World)
			{
				Rigidbody rigidbody = selectedTransform.GetComponent<Rigidbody>();
				rigidbody.Velocity = Vector2.Zero;
				rigidbody.body.Position = selectedTransform.position;
			}
		}

		if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.LeftShift))
		{
			switch (CurrentAxisSelected)
			{
				case Axis.X:
					selectedTransform.position = new Vector3(MouseInput.ScreenPosition.TranslateToGrid(10).X, selectedTransform.position.Y, 0);
					break;
				case Axis.Y:
					selectedTransform.position = new Vector3(selectedTransform.position.X, MouseInput.ScreenPosition.TranslateToGrid(10).Y, 0);
					break;
				case Axis.XY:
					selectedTransform.position = MouseInput.ScreenPosition.TranslateToGrid(10);
					break;
			}

		}
	}
	public void SelectObject(GameObject selectedGO)
	{
		GameObject.active = selectedGO != null;

		if (selectedGO == null)
		{
			objectSelected = false;
			return;
		}
		transform.position = selectedGO.transform.position;
		selectedTransform = selectedGO.transform;
		objectSelected = true;
	}
}
