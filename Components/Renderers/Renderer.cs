using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Scripts
{
	public class Renderer : Component
	{
		public virtual void Draw(SpriteBatch batch) { }

		[ShowInEditor] public Color Color { get; set; } = Color.White;
		[ShowInEditor] public int Layer { get; set; } = 0;
		[XmlIgnore]public BlendState blendState;
		//[System.ComponentModel.Editor(typeof(Editor.EffectEditor), typeof(UITypeEditor))]
		[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Effect effect { get; set; }
	}
}
