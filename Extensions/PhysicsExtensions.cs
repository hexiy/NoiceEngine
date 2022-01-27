
using Scripts;
using System;
using System.Collections.Generic;

namespace Engine
{
	public static class PhysicsExtensions
	{
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
					Vector2 boxPosition = boxCollider.transform.position;

					isIn = (point.X < boxCollider.offset.X + boxPosition.X + boxCollider.size.X / 2 &&
							point.X > boxCollider.offset.X + boxPosition.X - boxCollider.size.X / 2 &&
							point.Y < boxCollider.offset.Y + boxPosition.Y + boxCollider.size.Y / 2 &&
							point.Y > boxCollider.offset.Y + boxPosition.Y - boxCollider.size.Y / 2);
					break;
			}
			return isIn;
		}
	}
}
