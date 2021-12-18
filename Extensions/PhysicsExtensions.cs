using Microsoft.Xna.Framework;
using Scripts;
using System;
using System.Collections.Generic;

namespace Engine
{
	public static class PhysicsExtensions
	{
		public static bool In(this Vector3 point, Scripts.Shape shape)
		{
			bool isIn = false;
			float distance = 0;
			switch (shape)
			{
				case CircleShape circleCollider:
					if ((distance = Vector2.Distance(circleCollider.transform.position.ToVector2(), point)) < circleCollider.Radius)
					{
						isIn = true;
					}
					break;
				case BoxShape boxCollider:
					var boxPosition = boxCollider.rect.Center;

					Vector2 boxCenter = boxCollider.rect.Center;
					RectangleFloat rect = boxCollider.rect;
					rect.Position = boxCollider.transform.position.ToVector2();

					isIn = (point.X < rect.Right &&
							point.X > rect.Left &&
							point.Y < rect.Bottom &&
							point.Y > rect.Top);
					break;
			}
			return isIn;
		}
		public static bool In(this Vector2 point, Scripts.Shape shape)
		{
			bool isIn = false;
			float distance = 0;
			switch (shape)
			{
				case CircleShape circleCollider:
					if ((distance = Vector2.Distance(circleCollider.transform.position.ToVector2(), point)) < circleCollider.Radius)
					{
						isIn = true;
					}
					break;
				case BoxShape boxCollider:
					var boxPosition = boxCollider.rect.Center;

					Vector2 boxCenter = boxCollider.rect.Center;
					RectangleFloat rect = boxCollider.rect;
					rect.Position = boxCollider.transform.position - rect.Size * boxCollider.transform.anchor;

					isIn = (point.X < rect.Right &&
							point.X > rect.Left &&
							point.Y < rect.Bottom &&
							point.Y > rect.Top);
					break;
			}
			return isIn;
		}
	}
}
