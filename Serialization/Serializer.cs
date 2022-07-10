using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Engine;

public class Serializer
{
	private List<Type> SerializableTypes = new();

	public Serializer()
	{
		I = this;
	}

	public static string lastScene
	{
		get { return PersistentData.GetString("lastOpenedScene", "Assets/scene1.scene"); }
		set { PersistentData.Set("lastOpenedScene", value); }
	}
	public static Serializer I { get; set; }

	private void UpdateSerializableTypes()
	{
		SerializableTypes = new List<Type>();

		SerializableTypes.AddRange(typeof(GameObject).Assembly.GetTypes()
		                                             .Where(type => type.IsSubclassOf(typeof(Component))));

		SerializableTypes.AddRange(typeof(Component).Assembly.GetTypes()
		                                            .Where(type => type.IsSubclassOf(typeof(Component)) || type.IsSubclassOf(typeof(GameObject)))
		                                            .ToList());
	}

	public void SaveGameObject(GameObject go, string prefabPath)
	{
		go.isPrefab = true;
		var prefabSceneFile = SceneFile.CreateForOneGameObject(go);

		SaveGameObjects(prefabSceneFile, prefabPath);
	}

	public void SaveClipboardGameObject(GameObject go)
	{
		var prefabSceneFile = SceneFile.CreateForOneGameObject(go);

		SaveGameObjects(prefabSceneFile, Path.Combine("Temp", "clipboardGameObject"));
	}

	public GameObject LoadClipboardGameObject()
	{
		return LoadPrefab(Path.Combine("Temp", "clipboardGameObject"));
	}

	public GameObject LoadPrefab(string prefabPath)
	{
		using (var sr = new StreamReader(prefabPath))
		{
			UpdateSerializableTypes();

			var bb = SerializableTypes.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			var sceneFile = (SceneFile) xmlSerializer.Deserialize(sr);

			ConnectGameObjectsWithComponents(sceneFile);

			ConnectParentsAndChildren(sceneFile, true);

			var mainGo = new GameObject();
			for (var i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				for (var j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].gameObjectID = sceneFile.GameObjects[i].id;
				var go = sceneFile.GameObjects[i];

				if (i == 0)
				{
					mainGo = go;
				}

				Scene.I.AddGameObjectToScene(go);

				go.Awake();
			}

			return mainGo;
		}
	}

	public void SaveGameObjects(SceneFile sceneFile, string scenePath)
	{
		using (var sw = new StreamWriter(scenePath))
		{
			for (var i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				sceneFile.GameObjects[i].awoken = false;
				for (var j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].awoken = false;
			}

			UpdateSerializableTypes();

			var xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			xmlSerializer.Serialize(sw, sceneFile);

			for (var i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				sceneFile.GameObjects[i].awoken = true;
				for (var j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].awoken = true;
			}
		}
	}

	public SceneFile LoadGameObjects(string scenePath)
	{
		using (var sr = new StreamReader(scenePath))
		{
			UpdateSerializableTypes();

			var bb = SerializableTypes.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			var a = (SceneFile) xmlSerializer.Deserialize(sr);

			return a;
		}
	}

	public void ConnectParentsAndChildren(SceneFile sf, bool newIDs = false)
	{
		var gos = sf.GameObjects.ToArray();
		var comps = sf.Components.ToArray();

		var goIndexes = new int[gos.Length];
		for (var i = 0; i < goIndexes.Length; i++) goIndexes[i] = -1;

		var ogIDs = new int[gos.Length];
		for (var i = 0; i < ogIDs.Length; i++) ogIDs[i] = gos[i].id;
		for (var compIndex = 0; compIndex < comps.Length; compIndex++)
			if (comps[compIndex].GetType() == typeof(Transform))
			{
				var tr = comps[compIndex] as Transform;
				for (var goIndex = 0; goIndex < gos.Length; goIndex++)
					if (tr.parentID == ogIDs[goIndex]) // found child/parent pair
					{
						if (newIDs)
						{
							// we change ID of a parent, but if theres multiple children, we change it again? that dont work
							if (goIndexes[goIndex] == -1)
							{
								gos[goIndex].id = IDsManager.gameObjectNextID;
								IDsManager.gameObjectNextID++;

								goIndexes[goIndex] = gos[goIndex].id;
							}
							else
							{
								gos[goIndex].id = goIndexes[goIndex];
							}

							comps[compIndex].gameObject.id = IDsManager.gameObjectNextID;
							IDsManager.gameObjectNextID++;
						}

						(comps[compIndex] as Transform).SetParent(gos[goIndex].transform, false);
						(comps[compIndex] as Transform).parentID = gos[goIndex].id;
					}
			}

		for (var goIndex = 0; goIndex < gos.Length; goIndex++)
		{
			if (gos[goIndex].components.Count == 0)
			{
				continue;
			}

			if (goIndex == gos.Length - 1 && gos[goIndex].transform.children.Count == 0)
			{
				if (newIDs)
				{
					gos[goIndex].id = IDsManager.gameObjectNextID;
					IDsManager.gameObjectNextID++;
				}

				for (var i = 0; i < gos[goIndex].transform.children.Count; i++) gos[goIndex].transform.children[i].parentID = gos[goIndex].id;
			}
		}
	}

	public void ConnectGameObjectsWithComponents(SceneFile sf)
	{
		var gos = sf.GameObjects.ToArray();
		var comps = sf.Components.ToArray();

		for (var i = 0; i < gos.Length; i++)
		{
			for (var j = 0; j < comps.Length; j++)
				if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() == typeof(Transform)) // add transforms first
				{
					gos[i].AddExistingComponent(comps[j]);
					gos[i].LinkComponents(gos[i], comps[j]);
				}

			for (var j = 0; j < comps.Length; j++)
				if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() != typeof(Transform))
				{
					gos[i].AddExistingComponent(comps[j]);
					gos[i].LinkComponents(gos[i], comps[j]);
				}
		}
	}
}