using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;

namespace Scripts
{
	public class Rigidbody : Component
	{
		[System.Xml.Serialization.XmlIgnore] public Body body;
		[System.Xml.Serialization.XmlIgnore]
		public List<Rigidbody> touchingRigidbodies = new List<Rigidbody>();

		public new bool allowMultiple = false;

		[System.Xml.Serialization.XmlIgnore]
		[LinkableComponent]
		public Shape shape;
		[ShowInEditor] public bool UseGravity { get; set; } = true;
		[ShowInEditor] public bool IsStatic { get; set; } = false;
		[ShowInEditor] public bool IsTrigger { get; set; } = false;
		[ShowInEditor] public bool IsButton { get; set; } = false;
		[ShowInEditor] public Vector3 Velocity { get; set; } = new Vector3(0, 0, 0);

		[ShowInEditor] public float Mass { get; set; } = 1f;

		public float velocityDrag = 0.99f;
		[ShowInEditor] public float Bounciness { get; set; } = 0f;

		[ShowInEditor] public float AngularVelocity { get; set; } = 0;
		public float angularDrag = 1f;

		public float friction = 1;
		public float mass = 1;

		public override void Awake()
		{
			//gameObject.OnComponentAdded += CheckForColliderAdded;
			if (IsButton) { return; }

			Physics.AddRigidbody(this);

		}
		public void FixedUpdatePostCollisions()
		{
			if (Scene.I?.GraphicsDevice == null || IsStatic || IsButton)
			{
				return;
			}

			TranslateVelocityToTransform();

		}


		public void ApplyVelocity(Vector2 vel)
		{
			if (IsStatic == false)
			{
				Velocity += vel;
			}
		}
		public void TranslateVelocityToTransform()
		{
			transform.position += Velocity * Time.fixedDeltaTime;
		}
		public void TranslateAngularRotationToTransform()
		{
			transform.rotation.Z = body.Rotation;
		}
		public override void OnDestroyed()
		{
			for (int i = 0; i < touchingRigidbodies.Count; i++)
			{
				touchingRigidbodies[i].OnCollisionExit(this);
				OnCollisionExit(touchingRigidbodies[i]);
			}
		}

		public override void OnCollisionEnter(Rigidbody rigidbody) // TODO-TRANSLATE CURRENT VELOCITY TO COLLIDED RIGIDBODY, ADD FORCE (MassRatio2/MassRatio1)
		{
			touchingRigidbodies.Add(rigidbody);

			// Call callback on components that implement interface IPhysicsCallbackListener
			for (int i = 0; i < GameObject.Components.Count; i++)
			{
				if ((GameObject.Components[i] is Rigidbody) == false)
				{
					GameObject.Components[i].OnCollisionEnter(rigidbody);
				}
			}
		}
		public override void OnCollisionExit(Rigidbody rigidbody)
		{
			if (touchingRigidbodies.Contains(rigidbody))
			{
				touchingRigidbodies.Remove(rigidbody);
			}

			for (int i = 0; i < GameObject.Components.Count; i++)
			{
				if ((GameObject.Components[i] is Rigidbody) == false)
				{
					GameObject.Components[i].OnCollisionExit(rigidbody);
				}
			}
		}
		public override void OnTriggerEnter(Rigidbody rigidbody)
		{
			touchingRigidbodies.Add(rigidbody);

			// Call callback on components that implement interface IPhysicsCallbackListener
			for (int i = 0; i < GameObject.Components.Count; i++)
			{
				if ((GameObject.Components[i] is Rigidbody) == false)
				{
					GameObject.Components[i].OnTriggerEnter(rigidbody);
				}
			}
		}
		public override void OnTriggerExit(Rigidbody rigidbody)
		{
			if (touchingRigidbodies.Contains(rigidbody))
			{
				touchingRigidbodies.Remove(rigidbody);
			}

			for (int i = 0; i < GameObject.Components.Count; i++)
			{
				if ((GameObject.Components[i] is Rigidbody) == false)
				{
					GameObject.Components[i].OnTriggerExit(rigidbody);
				}
			}
		}
	}
}
