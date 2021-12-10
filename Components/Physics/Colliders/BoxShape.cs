using Engine;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Scripts
{
	public class BoxShape : Shape
	{
		public RectangleF rect;
		[ShowInEditor]
		public Vector2 Size { get { return rect.Size; } set { rect.Size = value; } }

		[ShowInEditor]
		public bool AutomaticSize { get; set; }


		public override void Update()
		{
			rect.Position = transform.position.ToPoint2();
			rect.Offset(-rect.Width * transform.anchor.X, -rect.Height * transform.anchor.Y);
			base.Update();
		}
	}
}
