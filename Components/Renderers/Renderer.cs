using Engine;
using System;
using System.Drawing.Design;
using System.Numerics;
using System.Xml.Serialization;
using Vector2 = Engine.Vector2;
using Vector3 = Engine.Vector3;

namespace Scripts
{
	public class Renderer : Component, IComparable<Renderer>
	{
		internal int vao;
		internal int vbo;
		[XmlIgnore] public Shader shader;

		[LinkableComponent]
		public BoxShape boxShape;


		public Color color = Color.White;
		public float layer = 1;
		[Hide] public float layerFromHierarchy = 0;

		public override void Awake()
		{
			base.Awake();
		}
		public Matrix4x4 GetModelViewProjection()
		{
			Vector2 pivotOffset = -(boxShape.size * transform.scale) / 2 + new Vector2((boxShape.size.X * transform.scale.X) * transform.pivot.X, (boxShape.size.Y * transform.scale.Y) * transform.pivot.Y);
			Matrix4x4 _translation = Matrix4x4.CreateTranslation(transform.position + (boxShape.offset*transform.scale) - pivotOffset);

			Matrix4x4 _rotation = Matrix4x4.CreateFromYawPitchRoll(transform.rotation.Y / 180 * Mathf.Pi * 4,
				transform.rotation.X / 180 * Mathf.Pi * 4,
				transform.rotation.Z / 180 * Mathf.Pi * 4);
			Matrix4x4 _scale = Matrix4x4.CreateScale(boxShape.size.X * transform.scale.X, boxShape.size.Y * transform.scale.Y, 1);

			Matrix4x4 _model = Matrix4x4.Identity;

			Matrix4x4 _view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			Matrix4x4 _projection = Camera.I.GetProjectionMatrix();

			return _scale * _model * _rotation * _translation * _view * _projection;
		}
		public int CompareTo(Renderer comparePart)
		{
			// A null value means that this object is greater.
			if (comparePart == null)
				return 1;

			else
				return this.layer.CompareTo((comparePart.layer + comparePart.layerFromHierarchy));
		}

		public virtual void Render() { }

	}
}
