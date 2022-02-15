/*

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Engine;
using System.Diagnostics;
using CircleF = MonoGame.Extended.CircleF;

namespace Scripts
{
	public class ParticleSystemRenderer : Renderer, ITexture
	{
		public new bool allowMultiple = false;

		[XmlIgnore] public SpriteBatch spriteBatch;

		[XmlIgnore] [ShowInEditor] public Texture2D texture { get; set; }
		public string texturePath { get; set; } = @"2D\rect.png";

		CircleF circle = new CircleF(new Vector2(0, 0), 10);
		public ParticleSystem particleSystem;
		public override void Awake()
		{
			if (texturePath != null)
			{
				LoadTexture(texturePath);
			}
			base.Awake();
		}
		public ParticleSystemRenderer()
		{
			spriteBatch = Scene.I.CreateSpriteBatch();
		}
		public void LoadTexture(string _texturePath)
		{
			if (File.Exists(_texturePath) == false) { return; }
			texturePath = _texturePath;
			texture = TextureCache.LoadTexture(_texturePath);
		}
		public override void Draw(SpriteBatch batch)
		{
			if (texture == null) { return; }
			if (particleSystem == null) { return; }
			CheckForSpriteBatch();

			for (int i = 0; i < particleSystem.particles.Count; i++)
			{

				if (particleSystem.particles.Count > i && particleSystem.particles[i] != null && particleSystem.particles[i].visible)
				{
					circle.Center = particleSystem.particles[i].worldPosition;

					circle.Radius = particleSystem.particles[i].radius * transform.scale.X;

					Vector2 pos = new Vector2(circle.Center.X - circle.Radius / 2, circle.Center.Y - circle.Radius / 2).TranslateToGrid(0.5f);
					SpriteBatchCache.GetSpriteBatch(texture.Name).Draw(texture, destinationRectangleFloat: new RectangleFloat(pos.X, pos.Y, circle.Radius, circle.Radius),
						sourceRectangleFloat: null,
							color: particleSystem.particles[i].color, rotation: 0, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1 / (Layer + 1));
				}
			}
		}
		public void CheckForSpriteBatch()
		{
			if (SpriteBatchCache.HasSpriteBatchForTexture(texture.Name) == false)
			{
				SpriteBatchCache.CreateBatchForTexture(texture, new DrawParameters()
				{
					transformMatrix = Camera.I.TransformMatrix,
					blendState = BlendState.NonPremultiplied,
					samplerState = SamplerState.PointClamp,
					depthStencilState = DepthStencilState.Default
				});
				SpriteBatchCache.BeginOne(texture.Name);
			}
		}
	}
}

*/