using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Engine;
using System.Diagnostics;

namespace Scripts
{
	public class ParticleSystemRenderer : Renderer, ITexture
	{
		public new bool allowMultiple = false;

		[XmlIgnore] public SpriteBatch spriteBatch;

		[XmlIgnore] [ShowInEditor] public Texture2D texture { get; set; }
		public string texturePath { get; set; } = @"2D\particle.png";

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
			Stream stream = TitleContainer.OpenStream(_texturePath);
			texture = Texture2D.FromStream(Scene.I.GraphicsDevice, stream);
			stream.Close();
		}
		public override void Draw(SpriteBatch batch)
		{
			if (texture == null) { return; }
			if (particleSystem == null) { return; }

			spriteBatch.Begin(blendState: BlendState.NonPremultiplied, sortMode: SpriteSortMode.Texture);
			//Parallel.For(0, particleSystem.particles.Count, (i) =>
			//{
			for (int i = 0; i < particleSystem.particles.Count; i++)
			{

				if (particleSystem.particles.Count > i && particleSystem.particles[i] != null && particleSystem.particles[i].visible)
				{
					circle.Center = particleSystem.particles[i].worldPosition;

					circle.Radius = particleSystem.particles[i].radius * transform.scale.X;

					spriteBatch.Draw(texture, destinationRectangle: new Rectangle((int)circle.Center.X - (int)circle.Radius / 2, (int)circle.Center.Y - (int)circle.Radius / 2, (int)circle.Radius, (int)circle.Radius),
							color: particleSystem.particles[i].color);
				}
			}
			spriteBatch.End();
			//});
		}
	}
}

