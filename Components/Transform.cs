using Engine;

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Scripts
{
	public class Transform : Component
	{

		//public new bool enabled { get { return true; } }

		public Vector3 scale = Vector3.One;
		public Vector3 rotation = Vector3.Zero;

		public Vector3 pivot = new Vector3(0, 0, 0);
		public Vector3 position = Vector3.Zero;
		[Hide] public Vector3 localPosition { get { return position - GetParentPosition(); } set { position = GetParentPosition() + value; } }
		[Hide] public Vector3 initialAngleDifferenceFromParent = Vector3.Zero;

		/*[ShowInEditor]
		public Vector3 LocalPosition
		{
			get { return transform.position - GetParentPosition(); }
			set
			{
				position = value + GetParentPosition();
				localPosition = value;
			}
		}*/
		public Vector3 GetParentPosition()
		{
			if (GameObject?.Parent != null)
			{
				return GameObject.Parent.transform.position;
			}
			else
			{
				return Vector3.Zero;
			}
		}

		public Vector3 TransformVector(Vector3 vec)
		{
			//float sin = (float)Math.Sin(transform.Rotation.X);
			//float cos = (float)Math.Cos(transform.Rotation.X);
			//var zRotation = new Vector3(direction.Y * sin - direction.X * cos, direction.X * sin + direction.Y * cos, transform.Rotation);
			//return zRotation;


			var a = Quaternion.CreateFromRotationMatrix(
				Matrix.CreateRotationX(90 * (float)Math.PI / 180) *
				Matrix.CreateRotationY(0) *
				Matrix.CreateRotationZ(0));
			var b = Matrix.CreateFromQuaternion(a);

			var q = Quaternion.CreateFromRotationMatrix(
				Matrix.CreateRotationX(transform.rotation.X) *
				Matrix.CreateRotationY(transform.rotation.Y) *
				Matrix.CreateRotationZ(transform.rotation.Z));


			// Matrix rotation = Matrix.CreateFromYawPitchRoll(transform.rotation.Y, transform.Rotation, transform.rotation.X);
			//Vector3 translation = Vector3.Transform(vec, rotation);
			return transform.position + Matrix.CreateFromQuaternion(q).Backward;

		}
		public Vector3 up { get { return position + TransformVector(new Vector3(0, 1, 0)); } }
	}
}
