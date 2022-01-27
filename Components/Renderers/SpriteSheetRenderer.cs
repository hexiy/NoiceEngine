using Engine;



namespace Scripts
{
	public class SpriteSheetRenderer : SpriteRenderer
	{
		private Vector2 spritesCount = new Vector2(1, 1);

		[ShowInEditor]
		public Vector2 SpritesCount
		{
			get => spritesCount;
			set
			{
				spritesCount = value;
				/*if (texture != null)
				{
					SpriteSize = new Vector2(texture.Width / SpritesCount.X, texture.Height / SpritesCount.Y);
				}*/
			}
		}

		[ShowInEditor] public int CurrentSpriteIndex { get; set; }

		[ShowInEditor] public Vector2 SpriteSize { get; set; }



		public override void Render()
		{
			/*if (GameObject == null || texture == null) { return; }
			CheckForSpriteBatch();

			SpriteBatchCache.GetSpriteBatch(texture.Name).Draw(
				texture: texture,
				destinationRectangleFloat: new RectangleFloat(transform.position.X - (transform.anchor.X * SpriteSize.X * transform.scale.Abs().X), transform.position.Y - (transform.anchor.Y * SpriteSize.Y * transform.scale.Abs().X), (SpriteSize.X * transform.scale.Abs().X), (SpriteSize.Y * transform.scale.Abs().Y)),
				sourceRectangleFloat: new RectangleFloat(SpriteSize.X * (int)(CurrentSpriteIndex % SpritesCount.X), SpriteSize.Y * (int)(CurrentSpriteIndex / (SpritesCount.X)), SpriteSize.X, SpriteSize.Y),
				color: Color,
				rotation: 0,
				origin: Vector2.Zero,
				effects: RenderingHelpers.GetSpriteFlipEffects(transform),
				layerDepth: Layer);*/
		}
		/*public override void OnTextureLoaded(Texture2D _texture, string _path)
		{
			SpriteSize = new Vector2(_texture.Width / SpritesCount.X, _texture.Height / SpritesCount.Y);

			base.OnTextureLoaded(_texture, _path);
		}*/
	}
}
