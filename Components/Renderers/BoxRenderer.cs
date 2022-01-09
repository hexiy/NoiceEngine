using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
namespace Scripts
{
	public class BoxRenderer : Renderer
	{
		[ShowInEditor] public float StrokeSize { get; set; } = 0.1f;
		[System.Xml.Serialization.XmlIgnore]
		public BoxShape boxShape;

		[ShowInEditor] public bool Fill { get; set; } = false;

		public override void Awake()
		{
			if (boxShape == null)
			{
				boxShape = GetComponent<BoxShape>();
			}
		}
		public override void Draw(SpriteBatch batch)
		{
			if (GameObject == null || boxShape == null)
			{
				return;
			}

			MonoGame.Extended.RectangleF drawRect = new MonoGame.Extended.RectangleF(Vector2.Zero, boxShape.rect.Size * transform.scale.ToVector2());

			drawRect.Offset(-boxShape.rect.Size.X * transform.anchor.X + ((boxShape.rect.Size.X * transform.anchor.X) * (1 - transform.scale.X)) + transform.position.X,
				-boxShape.rect.Size.Y * transform.anchor.Y + ((boxShape.rect.Size.Y * transform.anchor.Y) * (1 - transform.scale.Y)) + transform.position.Y);

			//Vector2 anchorOffset = ()
			//drawRect.Position = Vector2.Zero;

			//batch.End ();

			//batch.Begin (transformMatrix: Matrix.CreateRotationZ (transform.rotation.Z) * Matrix.CreateTranslation (transform.position));

			if (Fill)
			{
				batch.FillRectangle(drawRect, Color, layerDepth: (int)(1 / (Layer + 1)));
			}
			else
			{
				batch.DrawRectangle(drawRect, Color, StrokeSize, layerDepth: (int)(1 / (Layer + 1)));
			}
			//batch.End ();
			//batch.Begin (SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: Camera.Instance.effect);

			base.Draw(batch);
		}
	}
}