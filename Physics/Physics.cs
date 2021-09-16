using Microsoft.Xna.Framework;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Engine
{
	public static class Physics
	{
		public static readonly Vector3 gravity = new Vector3(0, -200,0);
		public static List<Rigidbody> rigidbodies = new List<Rigidbody>();


		private static Task physicsTask;

		public static bool Running = false;
		public static void AddRigidbody(Rigidbody rb)
		{
			if (rigidbodies.Contains(rb) == false)
			{
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
				physicsTask = Task.Factory.StartNew(FixedUpdate);
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
		public static void CheckForCollisionOnNextFrame(Rigidbody rb1, Rigidbody rb2)
		{
			if (rb1 == null || rb2 == null ||
				rb1.shape == null || rb2.shape == null ||
				rb1.transform == null || rb2.transform == null ||
				rb1.GameObject == null || rb2.GameObject == null) { return; }
			if (rb1.Intersects(rb2, rb1.GetPositionOnNextFrame(), rb2.GetPositionOnNextFrame()).intersects)
			{
				if (rb1.IsTrigger == false && rb2.IsTrigger == false)
				{
					ApplyCorrectVelocities(rb1, rb2);
				}
				if (rb1.touchingRigidbodies.Contains(rb2) == false)
				{
					if (rb1.IsTrigger || rb2.IsTrigger)
					{
						rb1.OnTriggerEnter(rb2);
						rb2.OnTriggerEnter(rb1);
					}
					else
					{
						rb1.OnCollisionEnter(rb2);
						rb2.OnCollisionEnter(rb1);
					}
				}
			}
			else
			{
				if (rb1.touchingRigidbodies.Contains(rb2))
				{
					if (rb1.IsTrigger || rb2.IsTrigger)
					{
						rb1.OnTriggerExit(rb2);
						rb2.OnTriggerExit(rb1);
					}
					else
					{
						rb1.OnCollisionExit(rb2);
						rb2.OnCollisionExit(rb1);
					}
				}
			}
		}

		/// <summary>
		/// Sets rb's velocity to such velocity, where on next frame, they won't collide, TODO-distinguish between shapes too
		/// </summary>
		/// <param name="rb1">First rigidbody</param>
		/// <param name="rb2">Second rigidbody</param>
		private static void ApplyCorrectVelocities(Rigidbody rb1, Rigidbody rb2)
		{
			if (rb1.shape is CircleShape && rb2.shape is CircleShape)
			{

				rb2.Velocity = gravity * 0;
				rb1.Velocity = gravity * 0;

				Vector2 rb1OldVelocity = rb1.Velocity;
				Vector2 rb2OldVelocity = rb2.Velocity;

				Vector2 velocities = rb1OldVelocity + rb2OldVelocity;

				Vector2 rb1_NextFramePosition = rb1.GetPositionOnNextFrame();
				Vector2 rb2_NextFramePosition = rb2.GetPositionOnNextFrame();

				Vector2 from2to1 = rb2_NextFramePosition - rb1_NextFramePosition;
				from2to1 = from2to1 / (rb1.GetComponent<CircleShape>().Radius + rb2.GetComponent<CircleShape>().Radius);
				from2to1.Normalize();
				from2to1 = from2to1 * new Vector2(50 + velocities.Length() / 3, 0);

				rb1.AngularVelocity += -from2to1.X * 0.01f;
				rb2.AngularVelocity += from2to1.X * 0.01f;
			}
			else if (rb1.shape is CircleShape && rb2.shape is LineShape)
			{
				CorrectVelocityCircleLine(rb1, rb2);
			}
			else if (rb1.shape is LineShape && rb2.shape is CircleShape)
			{
				CorrectVelocityCircleLine(rb2, rb1);
			}
			else if (rb1.shape is BoxShape && rb2.shape is LineShape)
			{
				CorrectVelocityRectangleLine(rectangle: rb1, line: rb2);
			}
			else if (rb1.shape is BoxShape && rb2.shape is BoxShape)
			{
				CorrectVelocityRectangleRectangle(rectangle1: rb1, rectangle2: rb2);
			}
			else if (rb1.shape is LineShape && rb2.shape is BoxShape)
			{
				CorrectVelocityRectangleLine(rectangle: rb2, line: rb1);
			}
		}
		/// <summary>
		/// Rectangle1 is moving
		/// </summary>
		static void CorrectVelocityRectangleRectangle(Rigidbody rectangle1, Rigidbody rectangle2)
		{
			Rigidbody dynamicRB = rectangle1.Velocity.Length() != 0 ? rectangle1 : rectangle2;

			Vector2 oldVelocity = dynamicRB.Velocity;
			Vector2 newVelocity = oldVelocity;

			dynamicRB.Velocity = new Vector2(0, oldVelocity.Y);
			if (dynamicRB.Intersects(rectangle2, dynamicRB.GetPositionOnNextFrame(ignoreGravity: false), rectangle2.GetPositionOnNextFrame(ignoreGravity: false)).intersects)
			{
				newVelocity = new Vector2(newVelocity.X, 0);
			}
			else
			{
				dynamicRB.Velocity = new Vector2(oldVelocity.X, 0);
				if (dynamicRB.Intersects(rectangle2, dynamicRB.GetPositionOnNextFrame(ignoreGravity: false), rectangle2.GetPositionOnNextFrame(ignoreGravity: false)).intersects)
				{
					newVelocity = new Vector2(0, newVelocity.Y);
				}
			}
			dynamicRB.Velocity = newVelocity;
		}
		/// <summary>
		/// Bouncy
		/// </summary>
		static void CorrectVelocityRectangleLine(Rigidbody rectangle, Rigidbody line)
		{
			Vector2 oldVelocity = rectangle.Velocity;
			Vector2 circleDir = rectangle.Velocity;
			circleDir.Normalize();

			Vector2 rectangleNextFramePosition = rectangle.GetPositionOnNextFrame();

			var lineStart = line.GetComponent<LineShape>().GetLineStart();
			var lineEnd = line.GetComponent<LineShape>().GetLineEnd();

			var rect = rectangle.GetComponent<BoxShape>().rect;
			rect.Position = rectangleNextFramePosition;

			Vector2 onLine1 = PhysicsExtensions.ClosestPointOnLine(lineStart, lineEnd, rect.TopLeft);
			float dist = Vector2.Distance(onLine1, rect.TopLeft);
			Vector2 closestOnLine = onLine1;
			Vector2 closestVertex = rect.TopLeft;


			Vector2 onLine2 = PhysicsExtensions.ClosestPointOnLine(lineStart, lineEnd, rect.TopLeft + new Vector2(rect.Width, 0));
			var newDistance = Vector2.Distance(onLine2, rect.TopLeft + new Vector2(rect.Width, 0));
			if (newDistance < dist)
			{
				closestOnLine = onLine2;
				closestVertex = rect.TopLeft + new Vector2(rect.Width, 0);
			}

			Vector2 onLine3 = PhysicsExtensions.ClosestPointOnLine(lineStart, lineEnd, rect.BottomRight);
			newDistance = Vector2.Distance(onLine2, rect.BottomRight);
			dist = newDistance > dist ? newDistance : dist;
			if (newDistance < dist)
			{
				closestOnLine = onLine3;
				closestVertex = rect.BottomRight;
			}

			Vector2 onLine4 = PhysicsExtensions.ClosestPointOnLine(lineStart, lineEnd, rect.BottomRight + new Vector2(-rect.Width, 0));
			newDistance = Vector2.Distance(onLine2, rect.BottomRight + new Vector2(-rect.Width, 0));
			dist = newDistance > dist ? newDistance : dist;
			if (newDistance < dist)
			{
				closestOnLine = onLine4;
				closestVertex = rect.BottomRight + new Vector2(-rect.Width, 0);
			}

			Vector2 lineDir = Extensions.MaxY(lineStart, lineEnd) - Extensions.MinY(lineStart, lineEnd);
			if (lineDir != Vector2.Zero)
			{
				lineDir.Normalize();
			}

			Vector2 from2To1 = closestVertex - closestOnLine;

			Vector2 from2To1Dir = new Vector2(from2To1.X, from2To1.Y);
			from2To1Dir.Normalize();

			Vector2 reflected = Vector2.Reflect(oldVelocity, from2To1Dir);

			// move rb along collider
			rectangle.Velocity = reflected + gravity * Time.deltaTime;
		}
		static void CorrectVelocityCircleLine(Rigidbody circle, Rigidbody line)
		{
			return;
			/*Vector3 oldVelocity = circle.Velocity;
			Vector3 circleDir = circle.Velocity;
			circleDir.Normalize();

			Vector2 circleRB_NextFramePosition = circle.GetPositionOnNextFrame();

			var lineStart = line.GetComponent<LineShape>().GetLineStart();
			var lineEnd = line.GetComponent<LineShape>().GetLineEnd();
			Vector2 onLine = PhysicsExtensions.ClosestPointOnLine(lineStart, lineEnd, circleRB_NextFramePosition);

			Vector2 lineDir = Extensions.MaxY(lineStart, lineEnd) - Extensions.MinY(lineStart, lineEnd);
			if (lineDir != Vector2.Zero)
			{
				lineDir.Normalize();
			}


			Vector2 from2To1 = circle.transform.Position - onLine;


			Vector2 from2To1Dir = new Vector2(from2To1.X, from2To1.Y);
			from2To1Dir.Normalize();
			Vector2 reflected = Vector2.Reflect(oldVelocity, from2To1Dir);

			circle.AngularVelocity = lineDir.X;

			// move rb along collider
			//circle.Velocity = lineDir * Time.deltaTime;

			circle.Velocity = gravity * Time.fixedDeltaTime + ((circle.transform.Position * lineDir * lineDir.Y) - circle.transform.Position) * Time.deltaTime;

			//circle.Velocity =gravity * Time.deltaTime + new Vector2(0, -lineDir.Y * 100);
*/

		}
		public static void FixedUpdate()
		{
			ResetVelocities();
			Running = true;
			while (Running)
			{
				Wait(Time.fixedDeltaTime);

				for (int i = 0; i < rigidbodies.Count; i++)
				{
					rigidbodies[i].FixedUpdatePreCollisions();
				}
				for (int i = 0; i < rigidbodies.Count; i++)
				{
					for (int j = i; j < rigidbodies.Count; j++)
					{
						if (j == i || rigidbodies[i].enabled == false || rigidbodies[i].GameObject.Active == false ||
							rigidbodies[j].enabled == false || rigidbodies[j].GameObject.Active == false) { continue; }

						CheckForCollisionOnNextFrame(rigidbodies[i],
						   rigidbodies[j]);
					}
				}
				for (int i = 0; i < rigidbodies.Count; i++)
				{
					rigidbodies[i].FixedUpdatePostCollisions();
				}
			}
		}
	}
}