using System.Collections.Generic;

namespace Scripts;

public class Pool
{
	public Stack<GameObject> freeObjects = new();
	public GameObject model;
	public Stack<GameObject> usedObjects = new();

	private void AddNewObject()
	{
		var gameObject = GameObject.Create(name: "Pooled object");
		for (var i = 0; i < model.components.Count; i++) gameObject.AddComponent(model.components[i].GetType());
		gameObject.Awake();
		freeObjects.Push(gameObject);
	}

	public GameObject Request()
	{
		if (freeObjects.Count == 0) AddNewObject();
		var gameObject = freeObjects.Pop();
		gameObject.activeSelf = true;
		usedObjects.Push(gameObject);
		return gameObject;
	}

	public void Return(GameObject gameObject)
	{
		gameObject.activeSelf = false;
		freeObjects.Push(gameObject);
	}
}