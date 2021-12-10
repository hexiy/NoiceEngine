using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

			GameObject.AddComponent<Rigidbody>().IsStatic = true;
			GetComponent<Rigidbody>().IsButton = true;

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
				boxColliderXY.rect = new Rectangle(0, 0, 20, 20);

				boxColliderX = GameObject.AddComponent<BoxShape>();
				boxColliderX.rect = new Rectangle(0, 0, 50, 5);

				boxColliderY = GameObject.AddComponent<BoxShape>();
				boxColliderY.rect = new Rectangle(0, 0, 5, 50);

				boxRendererXY = GameObject.AddComponent<BoxRenderer>();
				boxRendererX = GameObject.AddComponent<BoxRenderer>();
				boxRendererY = GameObject.AddComponent<BoxRenderer>();

				boxRendererXY.Color = Color.Orange;
				boxRendererX.Color = Color.Red;
				boxRendererY.Color = Color.Cyan;

				boxRendererX.boxCollider = boxColliderX;
				boxRendererXY.boxCollider = boxColliderXY;
				boxRendererY.boxCollider = boxColliderY;
			}
			base.Awake();
		}
		public override void Update()
		{
			//Debug.Console.Log("TransformHandle.GameObject.Active: " + GameObject.Active);
			
			if (MouseInput.MouseButton1State == ButtonState.Pressed)
			{
				if (GameObject.Active)
				{
					if (clicked)
					{
						Move(MouseInput.Delta);
					}
				}

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

			if (MouseInput.Position.In(boxColliderX).intersects)
			{
				boxRendererX.Color = Color.AntiqueWhite;
			}
			else
			{
				boxRendererX.Color = Color.Red;
			}
			if (MouseInput.Position.In(boxColliderY).intersects)
			{
				boxRendererY.Color = Color.AntiqueWhite;
			}
			else
			{
				boxRendererY.Color = Color.Cyan;
			}
			if (MouseInput.Position.In(boxColliderXY).intersects)
			{
				boxRendererXY.Color = Color.AntiqueWhite;
			}
			else
			{
				boxRendererXY.Color = Color.Orange;
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
			KeyboardState state = Keyboard.GetState();

			if (KeyboardInput.IsKeyDown(Keys.LeftShift))
			{
				switch (CurrentAxisSelected)
				{
					case Axis.X:
						selectedTransform.position = new Vector3(MouseInput.Position.TranslateToGrid(25).X, selectedTransform.position.Y,0);
						break;
					case Axis.Y:
						selectedTransform.position = new Vector3(selectedTransform.position.Y, MouseInput.Position.TranslateToGrid(25).Y,0);
						break;
					case Axis.XY:
						selectedTransform.position = MouseInput.Position.TranslateToGrid(25);
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
