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

	public static void AddGameObjectToBatcher(int textureID, SpriteRenderer renderer)
	{
		if (batchers.ContainsKey(textureID) == false)
		{
			CreateBatcherForTexture(renderer.material, renderer.texture);
		}

		batchers[textureID].AddGameObject(renderer.gameObjectID);
	}

	public static void UpdateAttribs(int textureID, GameObject go) // gameObject reference or index? maybe have secondary array to keep track of index
	{
		if (batchers.ContainsKey(textureID) == false)
		{
			return;
		}

		float[] attribs = new float[] {go.transform.position.X, go.transform.position.Y, go.GetComponent<BoxShape>().size.X, go.GetComponent<BoxShape>().size.Y};
		//float[] attribs = new float[] {0, 0, 2000, 2000};
		batchers[textureID].SetAttribs(go.id, attribs);
	}

	public static void RenderAllBatchers()
	{
		for (int i = 0; i < batchers.Count; i++)
		{
			batchers.ElementAt(i).Value.Render();
		}
	}
}