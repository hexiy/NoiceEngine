

using Scripts;

namespace Engine
{
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
		public QuadRenderer boxRendererXY;
		public QuadRenderer boxRendererX;
		public QuadRenderer boxRendererY;

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

				boxRendererXY = GetComponent<QuadRenderer>(0);
				boxRendererX = GetComponent<QuadRenderer>(1);
				boxRendererY = GetComponent<QuadRenderer>(2);
			}
			else
			{
				boxColliderXY = GameObject.AddComponent<BoxShape>();
				boxColliderXY.size = new Vector2(20, 20);
				boxColliderXY.offset = new Vector2(10, 10);

				boxColliderX = GameObject.AddComponent<BoxShape>();
				boxColliderX.size = new Vector2(50, 5);
				boxColliderX.offset = new Vector2(25, 2.5f);

				boxColliderY = GameObject.AddComponent<BoxShape>();
				boxColliderY.size = new Vector2(5, 50);
				boxColliderY.offset = new Vector2(2.5f, 25);

				boxRendererXY = GameObject.AddComponent<QuadRenderer>();
				boxRendererX = GameObject.AddComponent<QuadRenderer>();
				boxRendererY = GameObject.AddComponent<QuadRenderer>();

				boxRendererXY.Color = Color.Orange;
				boxRendererX.Color = Color.Red;
				boxRendererY.Color = Color.Cyan;

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
			if (MouseInput.ButtonReleased(MouseInput.Buttons.Left))
			{
				CurrentAxisSelected = null;
			}
			if (MouseInput.ButtonPressed(MouseInput.Buttons.Left))
			{
				clicked = false;
				if (MouseInput.Position.In(boxColliderX))
				{
					CurrentAxisSelected = Axis.X;
					clicked = true;
				}
				if (MouseInput.Position.In(boxColliderY))
				{
					CurrentAxisSelected = Axis.Y;
					clicked = true;
				}
				if (MouseInput.Position.In(boxColliderXY))
				{
					CurrentAxisSelected = Axis.XY;
					clicked = true;
				}
			}

			if (MouseInput.IsButtonDown(MouseInput.Buttons.Left) && GameObject.Active && clicked)
			{
				SetSelectedObjectRigidbodyAwake(false);
				Move(MouseInput.Delta);
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
			if (MouseInput.Position.In(boxColliderX))
			{
				//boxRendererX.fill = true;
				boxRendererX.Color = Color.Black;
			}
			else
			{
				//boxRendererX.fill = false;
				boxRendererX.Color = Color.White;

			}
			if (MouseInput.Position.In(boxColliderY))
			{
				//boxRendererY.fill = true;
				boxRendererY.Color = Color.Black;

			}
			else
			{
				//boxRendererY.fill = false;
				boxRendererY.Color = Color.White;

			}
			if (MouseInput.Position.In(boxColliderXY))
			{
				//boxRendererXY.fill = true;
				boxRendererXY.Color = Color.Black;

			}
			else
			{
				//boxRendererXY.fill = false;
				boxRendererXY.Color = Color.White;

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

			for (int i = 0; i < Scene.I.gameObjects.Count; i++)
			{
				if (Scene.I.gameObjects[i].Parent == selectedTransform.GameObject)
				{
					Scene.I.gameObjects[i].transform.position += moveVector;
				}
			}

			if (selectedTransform.HasComponent<Rigidbody>() && selectedTransform.GetComponent<Rigidbody>().isButton == false)
			{
				lock (Physics.World)
				{
					Rigidbody rigidbody = selectedTransform.GetComponent<Rigidbody>();
					rigidbody.Velocity = Vector2.Zero;
					//rigidbody.body.Position = selectedTransform.position;
				}
			}

			if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.LeftShift))
			{
				switch (CurrentAxisSelected)
				{
					case Axis.X:
						selectedTransform.position = new Vector3(MouseInput.Position.TranslateToGrid(1).X, selectedTransform.position.Y, 0);
						break;
					case Axis.Y:
						selectedTransform.position = new Vector3(selectedTransform.position.Y, MouseInput.Position.TranslateToGrid(1).Y, 0);
						break;
					case Axis.XY:
						selectedTransform.position = MouseInput.Position.TranslateToGrid(1);
						break;
				}

			}
		}
		public void SelectObject(GameObject selectedGO)
		{
			GameObject.Active = selectedGO != null;

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
}
