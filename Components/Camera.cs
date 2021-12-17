
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scripts;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Engine
{
	public class Camera : Component
	{
		[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Effect effect { get; set; }
		[XmlIgnore] public RenderTarget2D renderTarget;
		//public new bool enabled { get { return true; } }

		public static Camera I { get; private set; }

		public override void Awake()
		{
			I = this;

			renderTarget = new RenderTarget2D(
		  Scene.I.GraphicsDevice,
		  (int)Size.X,
		  (int)Size.Y,
		  false,
		  Scene.I.GraphicsDevice.PresentationParameters.BackBufferFormat,
		  DepthFormat.Depth24);
		}

		[ShowInEditor] public Color color = new Color(34, 34, 34);


		[ShowInEditor]
		public int AntialiasingStrength { get; set; } = 0;


		[ShowInEditor] public float Zoom { get; set; }

		[ShowInEditor] public Vector2 Size { get; set; } = new Vector2(600, 500);
		[ShowInEditor] public float CameraSize { get; set; } = 0.1f;

		public Matrix TransformMatrix
		{
			get
			{
				return
				Matrix.CreateScale(Vector3.One / CameraSize) *
				Matrix.CreateRotationX(transform.rotation.X) *
				Matrix.CreateRotationY(transform.rotation.Y) *
				Matrix.CreateRotationZ(transform.rotation.Z) *
				Matrix.CreateTranslation(-(int)transform.position.X,
				   -(int)transform.position.Y, 0);
				//Matrix.CreateRotationZ(Rotation);
			}
		}

		public void Move(Vector2 moveVector)
		{
			transform.position += moveVector;
		}

		public Vector2 WorldToScreen(Vector2 worldPosition)
		{
			return Vector2.Transform(worldPosition, TransformMatrix);
		}

		public Vector2 ScreenToWorld(Vector2 screenPosition)
		{
			return Vector2.Transform(screenPosition,
				Matrix.Invert(TransformMatrix));
		}
		public override void Update()
		{
			if (Scene.I.GraphicsDevice.PresentationParameters.MultiSampleCount != AntialiasingStrength * 2)
			{
				Scene.I.graphics.PreferMultiSampling = AntialiasingStrength == 0 ? false : true;
				Scene.I.GraphicsDevice.PresentationParameters.MultiSampleCount = AntialiasingStrength * 2;
				Scene.I.graphics.ApplyChanges();
			}
			//if (Scene.Instance.graphics.PreferredBackBufferWidth != Size.X || Scene.Instance.graphics.PreferredBackBufferHeight != Size.Y)
			//{
			//      Size = new Vector2(Scene.Instance.graphics.PreferredBackBufferWidth, Scene.Instance.graphics.PreferredBackBufferHeight);
			//       Scene.Instance.graphics.ApplyChanges();
			//}


			/*Vector2 cameraMovement = Vector2.Zero;
			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				cameraMovement.X = -1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				cameraMovement.X = 1;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				cameraMovement.Y = -1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				cameraMovement.Y = 1;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
			{
				AdjustZoom(0.1f);
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
			{
				AdjustZoom(-0.1f);
			}

			if (cameraMovement != Vector2.Zero)
			{
				cameraMovement.Normalize();
			}
			cameraMovement *= 10f;

			Move(cameraMovement);*/
		}
	}
}