using System.Collections.Generic;
using System.Xml.Serialization;
using tainicom.Aether.Physics2D.Dynamics;

namespace Scripts;

public class Rigidbody : Component
{
	[XmlIgnore] public Body body;
	[XmlIgnore]
	public List<Rigidbody> touchingRigidbodies = new List<Rigidbody>();

	public new bool allowMultiple = false;

	[XmlIgnore]
	[LinkableComponent]
	public Shape shape;
	public bool useGravity = false;
	public bool isStatic = false;
	public bool isTrigger = false;
	public bool isButton = false;
	[XmlIgnore] public Vector2 Velocity { get { if (body == null) { return Vector2.Zero; } else { return body.LinearVelocity; } } set { if (body == null) { return; } else { body.LinearVelocity = value; } } }
	public float Mass { get; set; } = 100;
	public float velocityDrag = 0.99f;
	[Show] public float Bounciness { get; set; } = 0f;

	[Show] public float AngularVelocity { get; set; } = 0;
	public float angularDrag = 1f;

	public float friction = 1;

	public override void Awake()
	{
		//gameObject.OnComponentAdded += CheckForColliderAdded;
		if (isButton) { return; }

		CreateBody();

		base.Awake();
	}
	public void CreateBody()
	{
		lock (Physics.World)
		{
			body = Physics.World.CreateBody(transform.position, transform.rotation.Z, isStatic ? BodyType.Static : BodyType.Dynamic);
			//body.SleepingAllowed = false;

			if (GetComponent<BoxShape>() != null)
			{
				BoxShape boxShape = GetComponent<BoxShape>();
				var pfixture = body.CreateRectangle(boxShape.size.X * transform.scale.X, boxShape.size.Y * transform.scale.Y, 1, Vector2.Zero);
				// Give it some bounce and friction
				pfixture.Friction = 0.1f;
				body.LinearDamping = 0;
				//body.LinearDamping = 3;
			}
			body.AngularDamping = 0;
			body.Mass = Mass;
		}
	}
	public override void FixedUpdate()
	{
		if (isStatic || isButton)
		{
			return;
		}

		UpdateTransform();

		//lock (Physics.World)
		//{
		//	body.Mass = Mass;
		//}
	}

	public void UpdateTransform()
	{
		transform.position = new Vector2(body.Position.X, body.Position.Y);
		transform.rotation.Z = body.Rotation * Mathf.TwoPi * 2;
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
		for (int i = 0; i < GameObject.components.Count; i++)
		{
			if ((GameObject.components[i] is Rigidbody) == false)
			{
				GameObject.components[i].OnCollisionEnter(rigidbody);
			}
		}
	}
	public override void OnCollisionExit(Rigidbody rigidbody)
	{
		if (touchingRigidbodies.Contains(rigidbody))
		{
			touchingRigidbodies.Remove(rigidbody);
		}

		for (int i = 0; i < GameObject.components.Count; i++)
		{
			if ((GameObject.components[i] is Rigidbody) == false)
			{
				GameObject.components[i].OnCollisionExit(rigidbody);
			}
		}
	}
	public override void OnTriggerEnter(Rigidbody rigidbody)
	{
		touchingRigidbodies.Add(rigidbody);

		// Call callback on components that implement interface IPhysicsCallbackListener
		for (int i = 0; i < GameObject.components.Count; i++)
		{
			if ((GameObject.components[i] is Rigidbody) == false)
			{
				GameObject.components[i].OnTriggerEnter(rigidbody);
			}
		}
	}
	public override void OnTriggerExit(Rigidbody rigidbody)
	{
		if (touchingRigidbodies.Contains(rigidbody))
		{
			touchingRigidbodies.Remove(rigidbody);
		}

		for (int i = 0; i < GameObject.components.Count; i++)
		{
			if ((GameObject.components[i] is Rigidbody) == false)
			{
				GameObject.components[i].OnTriggerExit(rigidbody);
			}
		}
	}
}
