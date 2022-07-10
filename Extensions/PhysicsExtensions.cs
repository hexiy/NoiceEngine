namespace Engine;

public static class PhysicsExtensions
{
	public static bool In(this Vector2 point, Shape shape)
	{
		var isIn = false;
		float distance = 0;
		switch (shape)
		{
			case CircleShape circleCollider:
				if ((distance = Vector2.Distance(circleCollider.transform.position.ToVector2(), point)) < circleCollider.radius)
				{
					isIn = true;
				}

				break;
			case BoxShape boxCollider:
				Vector2 boxPosition = boxCollider.transform.position;

				//float boxEndX = boxPosition.X + boxCollider.offset.X + (boxCollider.size.X / 2) * boxCollider.transform.pivot.X;

				var start = boxPosition + boxCollider.offset * boxCollider.transform.scale + boxCollider.size * boxCollider.transform.pivot;
				var end = boxPosition + boxCollider.offset * boxCollider.transform.scale + (boxCollider.size + boxCollider.size * boxCollider.transform.pivot) * boxCollider.transform.scale;
				isIn = point.X < end.X && point.X > start.X && point.Y < end.Y && point.Y > start.Y;
				break;
		}

		return isIn;
	}
}