using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TextureCache
{
	private static Dictionary<int, Texture2D> cachedTextures = new Dictionary<int, Texture2D>();


	public static void AddTexture(Texture2D texture)
	{
		cachedTextures.Add(GetHash(texture.Name), texture);
	}
	public static Texture2D GetTexture(int hash)
	{
		if (cachedTextures.ContainsKey(hash) == false) return null;
		return cachedTextures[hash];
	}
	public static int GetHash(string textureName)
	{
		return textureName.GetHashCode();
	}

	public static Texture2D LoadTexture(string path)
	{
		string textureName = Path.GetFileName(path);

		bool hasTexture = cachedTextures.ContainsKey(GetHash(textureName));
		Texture2D texture;

		if (hasTexture)
		{
			texture = TextureCache.GetTexture(TextureCache.GetHash(textureName));
		}
		else
		{
			Stream stream = TitleContainer.OpenStream(path);

			texture = Texture2D.FromStream(Engine.Scene.I.GraphicsDevice, stream);
			texture.Name = textureName;
			TextureCache.AddTexture(texture);
			stream.Close();
		}
		return texture;
	}
}

