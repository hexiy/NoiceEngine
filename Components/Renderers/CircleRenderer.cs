using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Engine;
namespace Scripts
{
	public class CircleRenderer : Renderer
	{
		CircleF circle = new CircleF(new Vector2(0, 0), 0);
		public int? Sides = null;

		[LinkableComponent]
		public CircleShape circleShape;
		[ShowInEditor] public bool Fill { get; set; } = false;

		public override void Draw(SpriteBatch batch)
		{
			if (GameObject == null || circleShape == null) { return; }
			circle.Radius = circleShape.Radius * Extensions.MaxVectorMember(transform.scale);
			circle.Center = new Point2((int)transform.position.X, (int)transform.position.Y);
			if (Fill)
			{
				batch.DrawCircle(circle, Sides != null ? (int)Sides : (int)(circle.Radius), Color, circle.Radius);

			}
			else
			{
				batch.DrawCircle(circle, (int)(circle.Radius), Color, 1);
			}
			//batch.DrawLine(point: circle.Center, length: circle.Radius, angle: transform.Rotation, color: Color, thickness: 1);

			base.Draw(batch);
		}
	}
}
