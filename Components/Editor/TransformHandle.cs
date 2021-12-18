﻿using Microsoft.Xna.Framework;
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
				boxColliderXY.rect = new RectangleFloat(0, 0, 2, 2);

				boxColliderX = GameObject.AddComponent<BoxShape>();
				boxColliderX.rect = new RectangleFloat(0, 0, 5, 0.5f);

				boxColliderY = GameObject.AddComponent<BoxShape>();
				boxColliderY.rect = new RectangleFloat(0, 0, 0.5f, 5);

				boxRendererXY = GameObject.AddComponent<BoxRenderer>();
				boxRendererX = GameObject.AddComponent<BoxRenderer>();
				boxRendererY = GameObject.AddComponent<BoxRenderer>();

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
			//Debug.Console.Log("TransformHandle.GameObject.Active: " + GameObject.Active);

			if (MouseInput.MouseButton1State == ButtonState.Pressed && GameObject.Active && clicked)
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
			if (KeyboardInput.IsKeyDown(Keys.Space))
			{
				
				var a = 1212;
			}
			if (MouseInput.Position.In(boxColliderX))
			{
				boxRendererX.Fill = true;
			}
			else
			{
				boxRendererX.Fill = false;
			}
			if (MouseInput.Position.In(boxColliderY))
			{
				boxRendererY.Fill = true;
			}
			else
			{
				boxRendererY.Fill = false;
			}
			if (MouseInput.Position.In(boxColliderXY))
			{
				boxRendererXY.Fill = true;
			}
			else
			{
				boxRendererXY.Fill = false;
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

			if (selectedTransform.HasComponent<Rigidbody>() && selectedTransform.GetComponent<Rigidbody>().IsButton == false)
			{
				lock (Physics.World)
				{
					Rigidbody rigidbody = selectedTransform.GetComponent<Rigidbody>();
					rigidbody.Velocity = Vector2.Zero;
					rigidbody.body.Position = selectedTransform.position;
				}
			}
			KeyboardState state = Keyboard.GetState();

			if (KeyboardInput.IsKeyDown(Keys.LeftShift))
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
