using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Engine
{
	public class Texture
	{
		public string path;
		public int id;
		public Vector2 size;
		public Texture(string _path)
		{
			path = _path;
			id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Image<Rgba32> image = Image.Load<Rgba32>(path);
			image.Mutate(x => x.Flip(FlipMode.Vertical));
			var pixels = new List<byte>(4 * image.Width * image.Height);

			size = new Vector2(image.Width, image.Height);

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
			//GL.GenerateTextureMipmap(id);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}

		public void Use()
		{
			GL.BindTexture(TextureTarget.Texture2D, id);
		}
	}
}
