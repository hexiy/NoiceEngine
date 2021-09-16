using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Design;
namespace Scripts
{
    public class Renderer : Component
    {
        public virtual void Draw(SpriteBatch batch) { }
        
        [ShowInEditor] public Color Color { get; set; } = Color.White;
        //[System.ComponentModel.Editor(typeof(Editor.EffectEditor), typeof(UITypeEditor))]
        [System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Effect effect { get; set; }
    }
}
