using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SpriteBatchCache
{
	private static Dictionary<int, (SpriteBatch spriteBatch, DrawParameters drawParameters)> cachedSpriteBatches = new Dictionary<int, (SpriteBatch spriteBatch, DrawParameters drawParameters)>();


	public static SpriteBatch CreateBatchForTexture(Texture2D texture, DrawParameters drawParameters)
	{
		SpriteBatch spriteBatch = new SpriteBatch(Scene.I.GraphicsDevice);

		cachedSpriteBatches.Add(GetHash(texture.Name), (spriteBatch, drawParameters));

		return spriteBatch;
	}
	public static SpriteBatch GetSpriteBatch(string textureName)
	{
		if (cachedSpriteBatches.ContainsKey(GetHash(textureName)) == false) return null;
		return cachedSpriteBatches[GetHash(textureName)].Item1;
	}
	public static int GetHash(string textureName)
	{
		return textureName.GetHashCode();
	}
	public static bool HasSpriteBatchForTexture(string textureName)
	{
		return cachedSpriteBatches.ContainsKey(GetHash(textureName));
	}
	public static void BeginOne(string textureName)
	{
		cachedSpriteBatches[GetHash(textureName)].spriteBatch.Begin(
				cachedSpriteBatches[GetHash(textureName)].drawParameters.sortMode,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.blendState,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.samplerState,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.depthStencilState,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.rasterizerState,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.effect,
				cachedSpriteBatches[GetHash(textureName)].drawParameters.transformMatrix);
	}
	public static void BeginAll()
	{
		for (int i = 0; i < cachedSpriteBatches.Count; i++)
		{
			cachedSpriteBatches.ElementAt(i).Value.spriteBatch.Begin(
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.sortMode,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.blendState,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.samplerState,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.depthStencilState,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.rasterizerState,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.effect,
				cachedSpriteBatches.ElementAt(i).Value.drawParameters.transformMatrix);
		}
	}
	public static void EndAll()
	{
		for (int i = 0; i < cachedSpriteBatches.Count; i++)
		{
			cachedSpriteBatches.ElementAt(i).Value.spriteBatch.End();
		}
	}
	public static void UpdateAllTransformMatrices()
	{
		for (int i = 0; i < cachedSpriteBatches.Count; i++)
		{
			cachedSpriteBatches.ElementAt(i).Value.drawParameters.transformMatrix = Camera.I.TransformMatrix;
		}
	}
}

