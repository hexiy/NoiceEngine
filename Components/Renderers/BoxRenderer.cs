using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Scripts
{
	public class BoxRenderer : Renderer
	{
		public float StrokeSize { get; set; } = 1;
		[LinkableComponent] public BoxShape boxCollider;

		[ShowInEditor] public bool Fill { get; set; } = false;

		public override void Draw (SpriteBatch batch)
		{
			if (GameObject == null || boxCollider == null)
			{
				return;
			}
			RectangleF drawRect = new RectangleF (Vector2.Zero, boxCollider.rect.Size * transform.scale.ToVector2 ());

			drawRect.Offset (-boxCollider.rect.Size.Width * transform.anchor.X, -boxCollider.rect.Size.Height * transform.anchor.Y);
			//drawRect.Position = Vector2.Zero;

			batch.End ();

			batch.Begin (transformMatrix: Matrix.CreateRotationZ (transform.rotation.Z) * Matrix.CreateTranslation (transform.position));

			if (Fill)
			{
				batch.FillRectangle (drawRect, Color);
			}
			else
			{
				batch.DrawRectangle (drawRect, Color, StrokeSize);
			}
			batch.End ();
			batch.Begin (SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: Camera.Instance.effect);

			base.Draw (batch);
		}
	}
}