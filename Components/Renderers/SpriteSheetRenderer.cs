
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scripts
{
	public class SpriteSheetRenderer : SpriteRenderer
	{
		private Vector2 spritesCount = new Vector2(1, 1);

		[ShowInEditor]
		public int MissingFrames { get; set; } = 0;

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
				MaxFrame = (int)(spritesCount.X * spritesCount.Y);
			}
		}
		//[ShowInEditor]
		public int MaxFrame { get; set; } = 1;

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
				if (CurrentSpriteIndex + 1 >= MaxFrame-MissingFrames)
				{
					CurrentSpriteIndex = 0;
				}
				else
				{
					CurrentSpriteIndex++;
				}
			}
			base.Update();
		}
		public override void Draw(SpriteBatch batch)
		{
			if (GameObject == null || texture == null) { return; }
			batch.Draw(texture: texture,
				destinationRectangle: new Rectangle((int)transform.position.X - (int)(transform.anchor.X * SpriteSize.X * transform.scale.Abs().X), (int)transform.position.Y - (int)(transform.anchor.Y * SpriteSize.Y * transform.scale.Abs().X), (int)(SpriteSize.X * transform.scale.Abs().X), (int)(SpriteSize.Y * transform.scale.Abs().Y)),
				sourceRectangle: new Rectangle((int)SpriteSize.X * (int)(CurrentSpriteIndex % SpritesCount.X), (int)SpriteSize.Y * (int)(CurrentSpriteIndex / (SpritesCount.X)), (int)SpriteSize.X, (int)SpriteSize.Y),
				color: Color.White);// effects: RenderingHelpers.GetSpriteFlipEffects(transform), rotation: transform.Rotation
		}
		public override void OnTextureLoaded(Texture2D _texture, string _path)
		{
			SpriteSize = new Vector2(_texture.Width / SpritesCount.X, _texture.Height / SpritesCount.Y);

			base.OnTextureLoaded(_texture, _path);
		}
	}
}
