


using Scripts;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Engine
{
	public class Camera : Component
	{
		//[XmlIgnore] public RenderTarget2D renderTarget;

		public static Camera I { get; private set; }

		public override void Awake()
		{
			I = this;

			/*	renderTarget = new RenderTarget2D(
			  Scene.I.GraphicsDevice,
			  (int)Size.X,
			  (int)Size.Y,
			  false,
			  Scene.I.GraphicsDevice.PresentationParameters.BackBufferFormat,
			  DepthFormat.Depth24);*/
		}

		[ShowInEditor] public Color color = new Color(34, 34, 34);


		[ShowInEditor]
		public int AntialiasingStrength { get; set; } = 0;


		[ShowInEditor] public float Zoom { get; set; }

		[ShowInEditor] public Vector2 Size { get; set; } = new Vector2(700, 500);
		[ShowInEditor] public float CameraSize { get; set; } = 0.1f;

		public Matrix TransformMatrix
		{
			get
			{
				return
				Matrix.CreateScale(new Vector3(1 / CameraSize, 1 / CameraSize, 0.0000000001f)) *
				Matrix.CreateRotationX(transform.rotation.X) *
				Matrix.CreateRotationY(transform.rotation.Y) *
				Matrix.CreateRotationZ(transform.rotation.Z) *
				Matrix.CreateTranslation(-transform.position.X / CameraSize,
				   -transform.position.Y / CameraSize, 0);
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
		public Vector2 CenterOfScreenToWorld()
		{
			return ScreenToWorld(new Vector2(Size.X / 2, Size.Y / 2));
		}
		public override void Update()
		{
/*			if (Scene.I.GraphicsDevice.PresentationParameters.MultiSampleCount != AntialiasingStrength * 2)
			{
				Scene.I.graphics.PreferMultiSampling = AntialiasingStrength == 0 ? false : true;
				Scene.I.GraphicsDevice.PresentationParameters.MultiSampleCount = AntialiasingStrength * 2;
				Scene.I.graphics.ApplyChanges();
			}*/
		}
	}
}