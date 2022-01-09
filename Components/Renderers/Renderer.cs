using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Scripts
{
	public class Renderer : Component, IComparable<Renderer>
	{
		public virtual void Draw(SpriteBatch batch) { }

		[ShowInEditor] public Color Color { get; set; } = Color.White;
		[ShowInEditor] public float Layer { get; set; } = 1;
		[XmlIgnore]public BlendState blendState;
		//[System.ComponentModel.Editor(typeof(Editor.EffectEditor), typeof(UITypeEditor))]
		[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Effect effect { get; set; }



		public int CompareTo(Renderer comparePart)
		{
			// A null value means that this object is greater.
			if (comparePart == null)
				return 1;

			else
				return this.Layer.CompareTo(1/comparePart.Layer);
		}
	}
}
