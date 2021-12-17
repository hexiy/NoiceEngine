using Engine;
using Microsoft.Xna.Framework;

namespace Scripts
{
	public class BoxShape : Shape
	{
		public RectangleFloat rect;
		[ShowInEditor]
		public Vector2 Size { get { return rect.Size; } set { rect.Size = value; } }

		[ShowInEditor]
		public bool AutomaticSize { get; set; }


		public override void Update()
		{
			rect.Position = transform.position;
			rect.Offset(-rect.Width * transform.anchor.X, -rect.Height * transform.anchor.Y);
			base.Update();
		}
	}
}
