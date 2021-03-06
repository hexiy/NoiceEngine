using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

		// delegates
		//SerializableTypes.AddRange(typeof(GameObject).Assembly.GetTypes()
		//                                             .Where(type => { return type.GetCustomAttribute<SerializableType>() != null; }));

		SerializableTypes.AddRange(typeof(Component).Assembly.GetTypes()
		                                            .Where(type => type.IsSubclassOf(typeof(Component)) || type.IsSubclassOf(typeof(GameObject)))
		                                            .ToList());
	}

	public void SaveGameObject(GameObject go, string prefabPath)
	{
		go.isPrefab = true;
		SceneFile prefabSceneFile = SceneFile.CreateForOneGameObject(go);

		SaveGameObjects(prefabSceneFile, prefabPath);
	}

	public void SaveClipboardGameObject(GameObject go)
	{
		SceneFile prefabSceneFile = SceneFile.CreateForOneGameObject(go);

		SaveGameObjects(prefabSceneFile, Path.Combine("Temp", "clipboardGameObject"));
	}

	public GameObject LoadClipboardGameObject()
	{
		return LoadPrefab(Path.Combine("Temp", "clipboardGameObject"));
	}

	public GameObject LoadPrefab(string prefabPath)
	{
		using (StreamReader sr = new StreamReader(prefabPath))
		{
			UpdateSerializableTypes();

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			SceneFile sceneFile = (SceneFile) xmlSerializer.Deserialize(sr);

			ConnectGameObjectsWithComponents(sceneFile);

			ConnectParentsAndChildren(sceneFile, true);

			GameObject mainGo = new GameObject();
			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].gameObjectID = sceneFile.GameObjects[i].id;
				GameObject go = sceneFile.GameObjects[i];

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
		using (StreamWriter sw = new StreamWriter(scenePath))
		{
			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				sceneFile.GameObjects[i].awoken = false;
				for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].awoken = false;
			}

			UpdateSerializableTypes();

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			xmlSerializer.Serialize(sw, sceneFile);

			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				sceneFile.GameObjects[i].awoken = true;
				for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++) sceneFile.GameObjects[i].components[j].awoken = true;
			}
		}
	}

	public SceneFile LoadGameObjects(string scenePath)
	{
		using (StreamReader sr = new StreamReader(scenePath))
		{
			UpdateSerializableTypes();

			Type[] bb = SerializableTypes.ToArray();

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

			SceneFile a = (SceneFile) xmlSerializer.Deserialize(sr);

			return a;
		}
	}

	public void ConnectParentsAndChildren(SceneFile sf, bool newIDs = false)
	{
		GameObject[] gos = sf.GameObjects.ToArray();
		Component[] comps = sf.Components.ToArray();

		int[] goIndexes = new int[gos.Length];
		for (int i = 0; i < goIndexes.Length; i++) goIndexes[i] = -1;

		int[] ogIDs = new int[gos.Length];
		for (int i = 0; i < ogIDs.Length; i++) ogIDs[i] = gos[i].id;
		for (int compIndex = 0; compIndex < comps.Length; compIndex++)
			if (comps[compIndex].GetType() == typeof(Transform))
			{
				Transform tr = comps[compIndex] as Transform;
				for (int goIndex = 0; goIndex < gos.Length; goIndex++)
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

		for (int goIndex = 0; goIndex < gos.Length; goIndex++)
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

				for (int i = 0; i < gos[goIndex].transform.children.Count; i++) gos[goIndex].transform.children[i].parentID = gos[goIndex].id;
			}
		}
	}

	public void ConnectGameObjectsWithComponents(SceneFile sf)
	{
		GameObject[] gos = sf.GameObjects.ToArray();
		Component[] comps = sf.Components.ToArray();

		for (int i = 0; i < gos.Length; i++)
		{
			for (int j = 0; j < comps.Length; j++)
				if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() == typeof(Transform)) // add transforms first
				{
					gos[i].AddExistingComponent(comps[j]);
					gos[i].LinkComponents(gos[i], comps[j]);
				}

			for (int j = 0; j < comps.Length; j++)
				if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() != typeof(Transform))
				{
					gos[i].AddExistingComponent(comps[j]);
					gos[i].LinkComponents(gos[i], comps[j]);
				}
		}

		sf.GameObjects = gos.ToList();
		sf.Components = comps.ToList();
	}
}