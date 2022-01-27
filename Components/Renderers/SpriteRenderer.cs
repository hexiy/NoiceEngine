using Engine;


using System.IO;

namespace Scripts
{
	public class SpriteRenderer : Renderer, ITexture
	{
		//[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Texture2D texture { get; set; }

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
			/*texture = TextureCache.LoadTexture(_texturePath);
			OnTextureLoaded(texture, _texturePath);*/
		}

		public override void Render()
		{
			/*if (GameObject == null || texture == null) { return; }
			CheckForSpriteBatch();

			SpriteBatchCache.GetSpriteBatch(texture.Name).Draw(
				texture: texture,
				position: transform.position,
				sourceRectangle: null,
				color: this.Color,
				rotation: -transform.rotation.Z,
				origin: new Vector2(transform.anchor.X * texture.Width, transform.anchor.Y * texture.Height),
				scale: transform.scale,
				effects: RenderingHelpers.GetSpriteFlipEffects(transform),
				layerDepth: Layer);
*/
		}
/*		public virtual void OnTextureLoaded(Texture2D _texture, string _path)
		{
		}*/

	}
}
