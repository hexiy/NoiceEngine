


using Scripts;
using System.Collections.Generic;
using System.Numerics;
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

		public Color color = new Color(34, 34, 34);


		public int antialiasingStrength = 0;


		public float zoom;

		public Vector2 size = new Vector2(600, 500);
		public float cameraSize = 0.1f;

		public Matrix4x4 GetProjectionMatrix()
		{
			float left = 0;
			float right = size.X;
			float bottom = 0;
			float top = size.Y;

			Matrix4x4 orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.00001f, 10000000f);

			return orthoMatrix;
		}

		public Matrix TransformMatrix
		{
			get
			{
				return
				Matrix.CreateScale(new Vector3(1 / cameraSize, 1 / cameraSize, 0.0000000001f)) *
				Matrix.CreateRotationX(transform.rotation.X) *
				Matrix.CreateRotationY(transform.rotation.Y) *
				Matrix.CreateRotationZ(transform.rotation.Z) *
				Matrix.CreateTranslation(-transform.position.X / cameraSize,
				   -transform.position.Y / cameraSize, 0);
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
			return ScreenToWorld(new Vector2(size.X / 2, size.Y / 2));
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