using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Scripts
{
	public class SpriteRenderer : Renderer, ITexture
	{
		[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Texture2D texture { get; set; }

		public string texturePath { get; set; } = "";


		[LinkableComponent]
		public BoxShape boxShape;

		public override void Awake()
		{
			if (texturePath != null)
			{
				LoadTexture(texturePath);
			}
			base.Awake();
		}
		public void LoadTexture(string _texturePath)
		{
			if (File.Exists(_texturePath) == false) { return; }
			texturePath = _texturePath;
			Stream stream = TitleContainer.OpenStream(_texturePath);
			texture = Texture2D.FromStream(Scene.I.GraphicsDevice, stream);
			stream.Close();

			OnTextureLoaded(texture, _texturePath);
		}

		public override void Draw(SpriteBatch batch)
		{
			if (GameObject == null || texture == null) { return; }

			batch.Draw(
				texture: texture,
				position: transform.position,
				sourceRectangle: null,
				color: this.Color,
				rotation: -transform.rotation.Z,
				origin: new Vector2(transform.anchor.X * texture.Width, transform.anchor.Y * texture.Height),
				scale: transform.scale,
				effects: RenderingHelpers.GetSpriteFlipEffects(transform),
				layerDepth: Layer);

			base.Draw(batch);
		}
		public virtual void OnTextureLoaded(Texture2D _texture, string _path)
		{
		}
	}
}
