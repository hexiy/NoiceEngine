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
	public class ParticleSystemRenderer : Renderer
	{
		public new bool allowMultiple = false;

		[XmlIgnore] public SpriteBatch spriteBatch;
		[XmlIgnore] [ShowInEditor] public Texture2D circleTexture { get; set; }
		public string texturePath=@"2D\particle.png";

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
			Stream stream = TitleContainer.OpenStream(_texturePath);
			circleTexture = Texture2D.FromStream(Scene.I.GraphicsDevice, stream);
			stream.Close();
			texturePath = _texturePath;
		}
		public override void Draw(SpriteBatch batch)
		{
			if (circleTexture == null) { return; }
			if (particleSystem == null) { return; }
			
			spriteBatch.Begin(blendState: BlendState.Additive,sortMode:SpriteSortMode.Texture);
			//Parallel.For(0, particleSystem.particles.Count, (i) =>
			//{
			for (int i = 0; i < particleSystem.particles.Count; i++)
			{

				if (particleSystem.particles.Count > i && particleSystem.particles[i] != null && particleSystem.particles[i].visible)
				{
					circle.Center = particleSystem.particles[i].localPosition + transform.localPosition;

					circle.Radius = particleSystem.particles[i].radius*transform.scale.X;

					spriteBatch.Draw(circleTexture, destinationRectangle: new Rectangle((int)circle.Center.X - (int)circle.Radius / 2, (int)circle.Center.Y - (int)circle.Radius / 2, (int)circle.Radius, (int)circle.Radius),
							color: particleSystem.particles[i].color);
				}
			}
			spriteBatch.End();
			//});
		}
	}
}

