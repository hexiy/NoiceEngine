using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scripts
{
	public class SpriteSheetRenderer : SpriteRenderer
	{
		private Vector2 spritesCount = new Vector2(1, 1);

		[ShowInEditor] public Vector2 FrameRange { get; set; } = new Vector2(0, 0);

		[ShowInEditor]
		public Vector2 SpritesCount
		{
			get => spritesCount;
			set
			{
				spritesCount = value;
				if (texture != null)
				{
					SpriteSize = new Vector2(texture.Width / SpritesCount.X, texture.Height / SpritesCount.Y);
				}
			}
		}
		[ShowInEditor]
		public float AnimationSpeed { get; set; } = 1;
		[ShowInEditor] public int CurrentSpriteIndex { get; set; }

		[ShowInEditor] public Vector2 SpriteSize { get; set; }
		private float timeOnCurrentFrame = 0;
		public override void Update()
		{
			if (AnimationSpeed == 0) return;
			timeOnCurrentFrame += Time.deltaTime * AnimationSpeed;
			while (timeOnCurrentFrame > 1 / AnimationSpeed)
			{
				timeOnCurrentFrame -= 1 / AnimationSpeed;
				if (CurrentSpriteIndex + 1 >= FrameRange.Y)
				{
					CurrentSpriteIndex = (int)FrameRange.X;
				}
				else
				{
					CurrentSpriteIndex++;
				}
			}
			base.Update();
		}
		public void ResetCurrentAnimation()
		{
			timeOnCurrentFrame = 0;
			CurrentSpriteIndex = (int)FrameRange.X;
		}
		public override void Draw(SpriteBatch batch)
		{
			if (GameObject == null || texture == null) { return; }
			batch.Draw(texture: texture,
				destinationRectangleFloat: new RectangleFloat(transform.position.X - (transform.anchor.X * SpriteSize.X * transform.scale.Abs().X), transform.position.Y - (transform.anchor.Y * SpriteSize.Y * transform.scale.Abs().X), (SpriteSize.X * transform.scale.Abs().X), (SpriteSize.Y * transform.scale.Abs().Y)),
				sourceRectangleFloat: new RectangleFloat(SpriteSize.X * (int)(CurrentSpriteIndex % SpritesCount.X), SpriteSize.Y * (int)(CurrentSpriteIndex / (SpritesCount.X)), SpriteSize.X, SpriteSize.Y),
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				effects: RenderingHelpers.GetSpriteFlipEffects(transform),
				layerDepth: 0);
		}
		public override void OnTextureLoaded(Texture2D _texture, string _path)
		{
			SpriteSize = new Vector2(_texture.Width / SpritesCount.X, _texture.Height / SpritesCount.Y);

			base.OnTextureLoaded(_texture, _path);
		}
	}
}
