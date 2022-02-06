﻿using Engine;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Engine
{
	public static class TextureCache
	{
		private static Dictionary<int, Texture> cachedTextures = new Dictionary<int, Texture>();

		private static Texture LoadAndCreateTexture(string texturePath, bool flipX = true)
		{
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Image<Rgba32> image = Image.Load<Rgba32>(texturePath);
			if (flipX)
			{
				image.Mutate(x => x.Flip(FlipMode.Vertical));
			}

			var pixels = new List<byte>(4 * image.Width * image.Height);

			for (int y = 0; y < image.Height; y++)
			{
				var row = image.GetPixelRowSpan(y);
				for (int x = 0; x < image.Width; x++)
				{
					pixels.Add(row[x].R);
					pixels.Add(row[x].G);
					pixels.Add(row[x].B);
					pixels.Add(row[x].A);
				}
			}
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			Texture texture = new Texture();
			texture.id = id;
			texture.size = new Vector2(image.Width, image.Height);
			texture.loaded = true;
			texture.path = texturePath;

			cachedTextures.Add(GetHash(texturePath), texture);
			return texture;
		}
		public static Texture GetTexture(string texturePath, bool flipX = true)
		{
			if (cachedTextures.ContainsKey(GetHash(texturePath)) == false)
			{
				return LoadAndCreateTexture(texturePath, flipX);
			}
			else
			{
				return cachedTextures[GetHash(texturePath)];
			}
		}
		public static void DeleteTexture(string texturePath)
		{
			if (cachedTextures.ContainsKey(GetHash(texturePath)))
			{
				GL.DeleteTexture(cachedTextures[GetHash(texturePath)].id);

				cachedTextures.Remove(GetHash(texturePath));
			}
		}
		public static int GetHash(string texturePath)
		{
			return texturePath.GetHashCode();
		}
	}
}