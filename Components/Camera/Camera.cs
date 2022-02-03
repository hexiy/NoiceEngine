


using Scripts;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
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


		public float ortographicSize = 1;

		public Vector2 size = new Vector2(600, 500);
		//public float cameraSize = 0.1f;

		public Matrix4x4 GetProjectionMatrix()
		{
			float left = -size.X / 2;
			float right = size.X / 2;
			float bottom = -size.Y / 2;
			float top = size.Y / 2;

			Matrix4x4 orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.00001f, 10000000f);

			return GetTranslationMatrix() * orthoMatrix * GetScaleMatrix();
		}
		public Matrix4x4 GetTranslationMatrix()
		{
			Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(-transform.position);
			return translationMatrix;
		}
		private Matrix4x4 GetScaleMatrix()
		{
			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1 / ortographicSize);
			return scaleMatrix;
		}
		public void Move(Vector2 moveVector)
		{
			transform.position += moveVector;
		}

		public Vector2 WorldToScreen(Vector2 worldPosition)
		{
			return Vector2.Transform(worldPosition, GetTranslationMatrix());
		}

		public Vector2 ScreenToWorld(Vector2 screenPosition)
		{
			return Vector2.Transform(screenPosition / size * 2,
				 Matrix.Invert(GetProjectionMatrix())) - size * ortographicSize / 2;
		}
		public Vector2 CenterOfScreenToWorld()
		{
			return ScreenToWorld(new Vector2(size.X / 2, size.Y / 2));
		}
	}
}