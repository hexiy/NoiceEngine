using System.Numerics;
using System.Xml.Serialization;

namespace Engine;

public class Camera : Component
{
	public int antialiasingStrength = 0;
	public Color color = new(34, 34, 34);
	public float ortographicSize = 2;
	//public float cameraSize = 0.1f;

	[XmlIgnore] public Matrix4x4 projectionMatrix;

	public Vector2 size = new(1200, 500);
	[XmlIgnore] public Matrix4x4 viewMatrix;
	//[XmlIgnore] public RenderTarget2D renderTarget;

	public static Camera I { get; private set; }

	public override void Awake()
	{
		I = this;
		if (Global.EditorAttached == false)
		{
			size = new Vector2(Window.I.ClientSize.X, Window.I.ClientSize.Y);
		}

		projectionMatrix = GetProjectionMatrix();
		viewMatrix = GetViewMatrix();
		/*	renderTarget = new RenderTarget2D(
		  Scene.I.GraphicsDevice,
		  (int)Size.X,
		  (int)Size.Y,
		  false,
		  Scene.I.GraphicsDevice.PresentationParameters.BackBufferFormat,
		  DepthFormat.Depth24);*/
	}

	public override void Update()
	{
		projectionMatrix = GetProjectionMatrix();

		base.Update();
	}

	private Matrix4x4 GetViewMatrix()
	{
		var _view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
		return _view;
	}

	private Matrix4x4 GetProjectionMatrix()
	{
		var left = -size.X / 2;
		var right = size.X / 2;
		var bottom = -size.Y / 2;
		var top = size.Y / 2;

		var orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.00001f, 10000000f);

		return GetTranslationMatrix() * orthoMatrix * GetScaleMatrix();
	}

	public Matrix4x4 GetTranslationMatrix()
	{
		var translationMatrix = Matrix4x4.CreateTranslation(-transform.position);
		return translationMatrix;
	}

	private Matrix4x4 GetScaleMatrix()
	{
		var scaleMatrix = Matrix4x4.CreateScale(1 / ortographicSize);
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
		                         Matrix.Invert(GetProjectionMatrix()))
		     - size * ortographicSize / 2;
	}

	public Vector2 CenterOfScreenToWorld()
	{
		return ScreenToWorld(new Vector2(size.X / 2, size.Y / 2));
	}

	public bool RectangleVisible(BoxShape shape)
	{
		var isIn = Vector2.Distance(shape.transform.position, transform.position) < size.X * 1.1f * (ortographicSize / 2) + shape.size.X / 2 * shape.transform.scale.MaxVectorMember();

		return isIn;
	}
}