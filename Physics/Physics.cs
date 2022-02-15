using Genbox.VelcroPhysics.Dynamics;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
namespace Engine;

public static class Physics
{
	public static World World;

	public static readonly Vector2 gravity = new Vector2(0, -900);

	public static bool Running = true;

	static Task PhysicsTask;

	public static void Init()
	{
		World = new World(gravity);

		//World.ContactManager.VelocityConstraintsMultithreadThreshold = 256;
		//World.ContactManager.PositionConstraintsMultithreadThreshold = 256;
		//World.ContactManager.CollideMultithreadThreshold = 256;
		//Thread physicsThread = new Thread(
		//new ThreadStart(PhysicsLoop));
		//
		//physicsThread.Name = "NoiceEngine Physics";
		//physicsThread.IsBackground = true;
		//physicsThread.Start();
		PhysicsTask = Task.Run(PhysicsLoop);
	}
	public static void PhysicsLoop()
	{
		while (true)
		{
			while (Running && Global.GameRunning)
			{
				Step();
				Wait(Time.deltaTime*0.05f); // if update took 5 ms, and deltaTime is 15 ms, only wait for 10 ms
			}
			Wait(0.3f); // wait if physics is disabled
		}
	}
	private static void Step()
	{
		lock (World)
		{
			World.Step(Time.deltaTime);
		}
	}
	private static Stopwatch sw = new Stopwatch();
	private static void Wait(double seconds)
	{
		if (seconds < 0) return;
		Thread.Sleep((int)(seconds * 1000f));
		//sw.Restart();
		//
		//while (sw.ElapsedMilliseconds < milliseconds)
		//{
		//
		//}
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
