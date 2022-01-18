
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public interface ITexture
	{
		//[System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Texture2D texture { get; set; }
		public string texturePath { get; set; }
		public void LoadTexture(string _texturePath)
		{
		}
	}
}
