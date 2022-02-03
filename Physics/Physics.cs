
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Engine
{
	public static class Physics
	{
		public static World World;

		public static readonly Vector2 gravity = new Vector2(0, -9);

		public static bool Running = true;

		public static Task PhysicsTask;

		public static void Init()
		{
			World = new World(gravity);

			PhysicsTask = Task.Run(PhysicsLoop);
		}
		public static void PhysicsLoop()
		{
			while (true)
			{
				while (Running && Global.GameRunning)
				{
					Step();
					Wait(Time.fixedDeltaTime);
				}
				Wait(30); // wait if physics is disabled
			}
		}
		public static void Step()
		{
			lock (World)
			{
				World.Step(Time.fixedDeltaTime);
			}
		}
		private static Stopwatch sw = new Stopwatch();
		private static void Wait(double milliseconds)
		{
			sw.Restart();

			while (sw.ElapsedMilliseconds < milliseconds)
			{

			}
		}
		public static void StartPhysics()
		{
			Running = true;
		}
		public static void StopPhysics()
		{
			Running = false;
		}
	}
}