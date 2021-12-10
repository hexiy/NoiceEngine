using Microsoft.Xna.Framework;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Engine
{
	public static class Physics
	{
		public static World World;

		public static readonly Vector3 gravity = new Vector3(0, -200, 0);
		public static List<Rigidbody> rigidbodies = new List<Rigidbody>();

		public static bool Running = false;

		public static void Init()
		{
			World = new World(new Vector2(0, -200));
		}
		public static void Step()
		{
			World.Step(Time.elapsedTime);
		}
		public static void AddRigidbody(Rigidbody rb)
		{
			if (rigidbodies.Contains(rb) == false)
			{
				Body body = World.CreateBody(rb.transform.position, 0, BodyType.Dynamic);
				var pfixture = body.CreateRectangle(100, 100, 1, Vector2.Zero);

				// Give it some bounce and friction
				pfixture.Restitution = 0.3f;
				pfixture.Friction = 0.5f;

				rigidbodies.Add(rb);
			}
		}
		private static void Wait(double durationSeconds)
		{
			var durationTicks = Math.Round(durationSeconds * Stopwatch.Frequency);
			var sw = Stopwatch.StartNew();

			while (sw.ElapsedTicks < durationTicks)
			{

			}
		}
		public static void StartPhysics()
		{
			if (Running == false)
			{
			}
		}
		private static void ResetVelocities()
		{
			for (int i = 0; i < rigidbodies.Count; i++)
			{
				rigidbodies[i].Velocity = Vector3.Zero;
			}
		}
		public static void StopPhysics()
		{
			Running = false;
		}
	}
}