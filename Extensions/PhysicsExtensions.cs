using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Scripts;
using System;
using System.Collections.Generic;

namespace Engine
{
	public static class PhysicsExtensions
	{
		public static float AngleBetween(Vector2 vector1, Vector2 vector2)
		{
			float returnAngle = (float)Math.Acos((vector1.Dot(vector2)) / (vector1.Length() * vector2.Length()));

			if (returnAngle == float.NaN)
			{
				returnAngle = 0;
			}
			return returnAngle;
		}

		// Kód z odpovede na StackOverflow - https://stackoverflow.com/a/9557244/4405279
		public static Vector2 ClosestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			Vector2 AP = point - lineStart;       //Vector from A to P   
			Vector2 AB = lineEnd - lineStart;       //Vector from A to B  

			float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
			float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
			float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

			if (distance < 0)     //Check if P projection is over vectorAB     
			{
				return lineStart;

			}
			else if (distance > 1)
			{
				return lineEnd;
			}
			else
			{
				return lineStart + AB * distance;
			}
		}
		public static float DistanceFromLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			return Vector2.Distance(ClosestPointOnLine(lineStart, lineEnd, point), point);
		}
		public static (bool intersects, float distance) In(this Vector3 point, Scripts.Shape shape)
		{
			bool isIn = false;
			float distance = 0;
			switch (shape)
			{
				case LineShape lineCollider:
					if ((distance = DistanceFromLine(lineCollider.GetLineStart(), lineCollider.GetLineEnd(), point)) <
						lineCollider.GetComponent<LineRenderer>().StrokeSize)
					{
						isIn = true;
					}
					break;
				case CircleShape circleCollider:
					if ((distance = Vector2.Distance(circleCollider.transform.position.ToVector2(), point)) < circleCollider.Radius)
					{
						isIn = true;
					}
					break;
				case BoxShape boxCollider:
					var boxPosition = boxCollider.rect.Center;

					Vector2 boxCenter = boxCollider.rect.Center;
					RectangleF rect = boxCollider.rect;
					rect.Position = boxCollider.transform.position.ToVector2();

					isIn = (point.X < rect.Right &&
							point.X > rect.Left &&
							point.Y < rect.Bottom &&
							point.Y > rect.Top);
					distance = rect.DistanceTo(point.ToVector2());

					break;
			}
			return (isIn, distance);
		}
		public static (bool intersects, float distance) In(this Vector2 point, Scripts.Shape shape)
		{
			bool isIn = false;
			float distance = 0;
			switch (shape)
			{
				case LineShape lineCollider:
					if ((distance = DistanceFromLine(lineCollider.GetLineStart(), lineCollider.GetLineEnd(), point)) <
						lineCollider.GetComponent<LineRenderer>().StrokeSize)
					{
						isIn = true;
					}
					break;
				case CircleShape circleCollider:
					if ((distance = Vector2.Distance(circleCollider.transform.position.ToVector2(), point)) < circleCollider.Radius)
					{
						isIn = true;
					}
					break;
				case BoxShape boxCollider:
					var boxPosition = boxCollider.rect.Center;

					Vector2 boxCenter = boxCollider.rect.Center;
					RectangleF rect = boxCollider.rect;
					rect.Position = boxCollider.transform.position.ToVector2()- rect.Size.ToVector2()*boxCollider.transform.anchor;

					isIn = (point.X < rect.Right &&
							point.X > rect.Left &&
							point.Y < rect.Bottom &&
							point.Y > rect.Top);
					distance = rect.DistanceTo(point);

					break;
			}
			return (isIn, distance);
		}

		public static (bool intersects, float distance) Intersects(this Rigidbody rb1, Rigidbody rb2, Vector3? rb1Pos = null, Vector3? rb2Pos = null)
		{
			Vector3 rb1Position;
			Vector3 rb2Position;

			if (rb1Pos == null || rb2Pos == null)
			{
				rb1Position = rb1.transform.position;
				rb2Position = rb1.transform.position;
			}
			else
			{
				rb1Position = (Vector3)rb1Pos;
				rb2Position = (Vector3)rb2Pos;
			}

			if (rb1.shape is CircleShape && rb2.shape is CircleShape)
			{
				float dist = Vector3.Distance(rb1Position, rb2Position);
				return (dist < rb1.GetComponent<CircleShape>().Radius + rb2.GetComponent<CircleShape>().Radius, dist);
			}
			else if (rb1.shape is BoxShape && rb2.shape is BoxShape)
			{
				RectangleF newRect1 = ((BoxShape)rb1.shape).rect;
				newRect1.Size *= rb1.transform.scale.ToVector2();

				RectangleF newRect2 = ((BoxShape)rb2.shape).rect;
				newRect2.Size *= rb2.transform.scale.ToVector2();
				return (newRect1.Intersects(newRect2), 0);
			}
			else if (rb1.shape is CircleShape && rb2.shape is LineShape)
			{
				var dist = Vector2.Distance(ClosestPointOnLine(rb2.GetComponent<LineShape>().GetLineStart(), rb2.GetComponent<LineShape>().GetLineEnd(), rb1Position), rb1Position);
				return (dist <= rb1.GetComponent<CircleShape>().Radius, dist);
			}
			else if (rb2.shape is CircleShape && rb1.shape is LineShape)
			{
				var dist = Vector2.Distance(ClosestPointOnLine(rb1.GetComponent<LineShape>().GetLineStart(), rb1.GetComponent<LineShape>().GetLineEnd(), rb2Position), rb2Position);
				return (dist <= rb2.GetComponent<CircleShape>().Radius, dist);
			}

			else if ((rb2.shape is BoxShape && rb1.shape is LineShape) || rb2.shape is LineShape && rb1.shape is BoxShape)
			{
				BoxShape boxCollider = (BoxShape)(rb2.shape is BoxShape ? rb2.shape : rb1.shape);

				LineShape lineCollider = (LineShape)(rb2.shape is LineShape ? rb2.shape : rb1.shape);

				List<Vector2[]> lines = new List<Vector2[]>() {
					new Vector2[2] { boxCollider.transform.position, boxCollider.transform.position + new Vector2(boxCollider.rect.Width, 0) }, // UP
                    new Vector2[2] { boxCollider.transform.position, boxCollider.transform.position + new Vector2(0, boxCollider.rect.Height) }, // LEFT
                    new Vector2[2] { boxCollider.transform.position + new Vector2(0, boxCollider.rect.Height), boxCollider.transform.position + new Vector2(boxCollider.rect.Width, boxCollider.rect.Height) }, // BOTTOM
                    new Vector2[2] { boxCollider.transform.position + new Vector2(boxCollider.rect.Width, 0), boxCollider.transform.position + new Vector2(boxCollider.rect.Width, boxCollider.rect.Height) }  // RIGHT
                };
				for (int i = 0; i < lines.Count; i++)
				{
					if (LinesIntersecting((lines[i])[0], (lines[i])[1], lineCollider.GetLineStart(), lineCollider.GetLineEnd()))
					{
						return (true, 0);
					}
				}
			}

			else if (rb2.shape is LineShape && rb1.shape is LineShape)
			{
				return (LinesIntersecting(((LineShape)rb1.shape).GetLineStart(), ((LineShape)rb1.shape).GetLineEnd(),
					((LineShape)rb2.shape).GetLineStart(), ((LineShape)rb2.shape).GetLineEnd()), 0);



			}
			return (false, 0);
		}
		public static (bool intersects, float distance) Intersects2D(this Rigidbody rb1, Rigidbody rb2, Vector2? rb1Pos = null, Vector2? rb2Pos = null)
		{
			Vector2 rb1Position;
			Vector2 rb2Position;

			if (rb1Pos == null || rb2Pos == null)
			{
				rb1Position = rb1.transform.position;
				rb2Position = rb1.transform.position;
			}
			else
			{
				rb1Position = (Vector2)rb1Pos;
				rb2Position = (Vector2)rb2Pos;
			}

			if (rb1.shape is CircleShape && rb2.shape is CircleShape)
			{
				float dist = Vector2.Distance(rb1Position, rb2Position);
				return (dist < rb1.GetComponent<CircleShape>().Radius + rb2.GetComponent<CircleShape>().Radius, dist);
			}
			else if (rb1.shape is BoxShape && rb2.shape is BoxShape)
			{
				RectangleF newRect1 = ((BoxShape)rb1.shape).rect;
				newRect1.Size *= rb1.transform.scale.ToVector2();

				RectangleF newRect2 = ((BoxShape)rb2.shape).rect;
				newRect2.Size *= rb2.transform.scale.ToVector2();
				return (newRect1.Intersects(newRect2), 0);
			}
			else if (rb1.shape is CircleShape && rb2.shape is LineShape)
			{
				var dist = Vector2.Distance(ClosestPointOnLine(rb2.GetComponent<LineShape>().GetLineStart(), rb2.GetComponent<LineShape>().GetLineEnd(), rb1Position), rb1Position);
				return (dist <= rb1.GetComponent<CircleShape>().Radius, dist);
			}
			else if (rb2.shape is CircleShape && rb1.shape is LineShape)
			{
				var dist = Vector2.Distance(ClosestPointOnLine(rb1.GetComponent<LineShape>().GetLineStart(), rb1.GetComponent<LineShape>().GetLineEnd(), rb2Position), rb2Position);
				return (dist <= rb2.GetComponent<CircleShape>().Radius, dist);
			}

			else if ((rb2.shape is BoxShape && rb1.shape is LineShape) || rb2.shape is LineShape && rb1.shape is BoxShape)
			{
				BoxShape boxCollider = (BoxShape)(rb2.shape is BoxShape ? rb2.shape : rb1.shape);

				LineShape lineCollider = (LineShape)(rb2.shape is LineShape ? rb2.shape : rb1.shape);

				List<Vector2[]> lines = new List<Vector2[]>() {
					new Vector2[2] { boxCollider.transform.position, boxCollider.transform.position + new Vector2(boxCollider.rect.Width, 0) }, // UP
                    new Vector2[2] { boxCollider.transform.position, boxCollider.transform.position + new Vector2(0, boxCollider.rect.Height) }, // LEFT
                    new Vector2[2] { boxCollider.transform.position + new Vector2(0, boxCollider.rect.Height), boxCollider.transform.position + new Vector2(boxCollider.rect.Width, boxCollider.rect.Height) }, // BOTTOM
                    new Vector2[2] { boxCollider.transform.position + new Vector2(boxCollider.rect.Width, 0), boxCollider.transform.position + new Vector2(boxCollider.rect.Width, boxCollider.rect.Height) }  // RIGHT
                };
				for (int i = 0; i < lines.Count; i++)
				{
					if (LinesIntersecting((lines[i])[0], (lines[i])[1], lineCollider.GetLineStart(), lineCollider.GetLineEnd()))
					{
						return (true, 0);
					}
				}
			}

			else if (rb2.shape is LineShape && rb1.shape is LineShape)
			{
				return (LinesIntersecting(((LineShape)rb1.shape).GetLineStart(), ((LineShape)rb1.shape).GetLineEnd(),
					((LineShape)rb2.shape).GetLineStart(), ((LineShape)rb2.shape).GetLineEnd()), 0);



			}
			return (false, 0);
		}

		// Kód z odpovede na StackOverflow - https://gamedev.stackexchange.com/a/26022
		public static bool LinesIntersecting(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		{
			float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
			float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
			float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

			// Detect coincident lines (has a problem, read below)
			if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

			float r = numerator1 / denominator;
			float s = numerator2 / denominator;

			return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
		}
	}
}
