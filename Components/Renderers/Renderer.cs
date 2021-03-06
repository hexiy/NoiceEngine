using System.Numerics;
using System.Xml.Serialization;
using Vector2 = Engine.Vector2;
using Vector4 = Engine.Vector4;

namespace Scripts;

public class Renderer : Component, IComparable<Renderer>
{
	[LinkableComponent] public BoxShape boxShape;
	public Color color = Color.White;

	private float layer;

	[Hide] public float layerFromHierarchy = 0;

	[Show] public Material material;
	internal bool onScreen = true;

	public float Layer
	{
		get { return layer; }
		set
		{
			layer = value;
			Scene.I?.RenderQueueChanged();
		}
	}
	[XmlIgnore] public Matrix4x4 LatestModelViewProjection { get; private set; }

	public int CompareTo(Renderer comparePart)
	{
		// A null value means that this object is greater.
		if (comparePart == null)
		{
			return 1;
		}

		return Layer.CompareTo(comparePart.Layer + comparePart.layerFromHierarchy);
	}

	public override void Awake()
	{
		CreateMaterial();

		base.Awake();
	}

	public virtual void CreateMaterial()
	{
	}

	private Matrix4x4 GetModelViewProjection()
	{
		Vector2 pivotOffset = -(boxShape.size * transform.scale) / 2 + new Vector2(boxShape.size.X * transform.scale.X * transform.pivot.X, boxShape.size.Y * transform.scale.Y * transform.pivot.Y);
		Matrix4x4 _translation = Matrix4x4.CreateTranslation(transform.position + boxShape.offset * transform.scale - pivotOffset);

		Matrix4x4 _rotation = Matrix4x4.CreateFromYawPitchRoll(transform.rotation.Y / 180 * Mathf.Pi * 4,
		                                                       transform.rotation.X / 180 * Mathf.Pi * 4,
		                                                       transform.rotation.Z / 180 * Mathf.Pi * 4);
		Matrix4x4 _scale = Matrix4x4.CreateScale(boxShape.size.X * transform.scale.X, boxShape.size.Y * transform.scale.Y, 1);

		return _scale * Matrix4x4.Identity * _rotation * _translation * Camera.I.viewMatrix * Camera.I.projectionMatrix;
	}

	public Vector4 GetSize()
	{
		return new Vector4(boxShape.size.X * transform.scale.X, boxShape.size.Y * transform.scale.Y, 1, 1);
	}

	public override void Update()
	{
		if (boxShape == null)
		{
			return;
		}
		//if (Time.elapsedTicks % 10 == 0) onScreen = Camera.I.RectangleVisible(boxShape);

		if (onScreen)
		{
			LatestModelViewProjection = GetModelViewProjection();
		}

		base.Update();
	}

	public virtual void Render()
	{
	}
}