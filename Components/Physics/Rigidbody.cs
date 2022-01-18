﻿using Engine;

using System.Collections.Generic;
using System.Xml.Serialization;
using tainicom.Aether.Physics2D.Dynamics;

namespace Scripts
{
	public class Rigidbody : Component
	{
		[XmlIgnore] public Body body;
		[XmlIgnore]
		public List<Rigidbody> touchingRigidbodies = new List<Rigidbody>();

		public new bool allowMultiple = false;

		[XmlIgnore]
		[LinkableComponent]
		public Shape shape;
		[ShowInEditor] public bool useGravity = false;
		[ShowInEditor] public bool isStatic = false;
		[ShowInEditor] public bool isTrigger = false;
		[ShowInEditor] public bool isButton = false;
		[XmlIgnore] [ShowInEditor] public Vector2 Velocity { get { if (body == null) { return Vector2.Zero; } else { return body.LinearVelocity; } } set { if (body == null) { return; } else { body.LinearVelocity = value; } } }
		[ShowInEditor] public float Mass { get; set; } = 1;


		public float velocityDrag = 0.99f;
		[ShowInEditor] public float Bounciness { get; set; } = 0f;

		[ShowInEditor] public float AngularVelocity { get; set; } = 0;
		public float angularDrag = 1f;

		public float friction = 1;
		public float mass = 1;

		public override void Awake()
		{
			//gameObject.OnComponentAdded += CheckForColliderAdded;
			if (isButton) { return; }

			Physics.CreateBody(this);
		}
		public override void FixedUpdate()
		{
			if (isStatic || isButton)
			{
				return;
			}

			UpdateTransform();

		}

		public void UpdateTransform()
		{
			//transform.position = body.Position;
		}
		public void TranslateAngularRotationToTransform()
		{
			transform.rotation.Z = body.Rotation;
		}
		public override void OnDestroyed()
		{
			if (body != null)
			{
				lock (Physics.World)
				{
					body.Enabled = false;
					Physics.World.Remove(body);
				}
			}
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
