using System.Collections.Generic;
using System.Linq;

namespace Engine;

public class BatchingManager
{
	private static Dictionary<int, Batcher> batchers = new Dictionary<int, Batcher>(); // textureID

	private static void CreateBatcherForTexture(Material material, Texture texture)
	{
		Batcher batcher = new Batcher(10000, material, texture);
		batchers.Add(texture.id, batcher);
	}

	public static void AddGameObjectToBatcher(int textureID, SpriteRenderer renderer, int instanceIndex = 0)
	{
		if (batchers.ContainsKey(textureID) == false)
		{
			CreateBatcherForTexture(renderer.material, renderer.texture);
		}

		batchers[textureID].AddGameObject(renderer.gameObjectID, instanceIndex);
	}

	private static float[] attribsSkeleton = new float[]{0, 0, 0, 0, 0,0,0,0};
	public static void UpdateAttribs(int textureID, int gameObjectID, Vector2 position, Vector2 size, Color color, int instanceIndex = 0) //  use instanceIndex for particles-when we use single gameObject
	{
		if (batchers.ContainsKey(textureID) == false)
		{
			return;
		}

		attribsSkeleton[0] = position.X;
		attribsSkeleton[1] = position.Y;
		attribsSkeleton[2] = size.X;
		attribsSkeleton[3] = size.Y;
		attribsSkeleton[4] = ((float)color.R)/255f;
		attribsSkeleton[5] = ((float)color.G)/255f;
		attribsSkeleton[6] = ((float)color.B)/255f;
		attribsSkeleton[7] = ((float)color.A)/255f;
		/*attribsSkeleton = new float[]
		                  {
			                  position.X, position.Y, size.X , size.Y, color.R/255f,color.G/255f,color.B/255f,color.A
		                  };*/
		//float[] attribs = new float[] {0, 0, 2000, 2000};
		batchers[textureID].SetAttribs(gameObjectID, attribsSkeleton, instanceIndex);
	}

	public static void RenderAllBatchers()
	{
		for (int i = 0; i < batchers.Count; i++)
		{
			batchers.ElementAt(i).Value.Render();
		}
	}
}