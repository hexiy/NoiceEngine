using Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Engine
{
	public class Serializer
	{
		public static string lastScene
		{
			get { return PersistentData.GetString("lastOpenedScene", "Assets/scene1.scene"); }
			set { PersistentData.Set("lastOpenedScene", value); }
		}
		public static Serializer I { get; set; }
		List<Type> SerializableTypes = new List<Type>();
		public Serializer()
		{
			I = this;
		}
		void UpdateSerializableTypes()
		{
			SerializableTypes = new List<Type>();

			SerializableTypes.AddRange(typeof(Engine.GameObject).Assembly.GetTypes()
					 .Where(type => (type.IsSubclassOf(typeof(Scripts.Component)))));

			SerializableTypes.AddRange(typeof(Scripts.Component).Assembly.GetTypes()
				 .Where(type => (type.IsSubclassOf(typeof(Scripts.Component)) ||
				 (type.IsSubclassOf(typeof(GameObject)))))
				 .ToList());
		}
		public void SaveGameObject(GameObject go, string prefabPath)
		{
			SceneFile prefabSceneFile = SceneFile.CreateForOneGameObject(go);

			SaveGameObjects(prefabSceneFile, prefabPath);
		}
		public GameObject LoadPrefab(string prefabPath)
		{
			using (StreamReader sr = new StreamReader(prefabPath))
			{
				UpdateSerializableTypes();

				var bb = SerializableTypes.ToArray();

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

				var sceneFile = ((SceneFile)xmlSerializer.Deserialize(sr));

				ConnectGameObjectsWithComponents(sceneFile);

				ConnectParentsAndChildren(sceneFile, true);

				GameObject mainGo = new GameObject();
				for (int i = 0; i < sceneFile.GameObjects.Count; i++)
				{
					for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++)
					{
						sceneFile.GameObjects[i].components[j].gameObjectID = sceneFile.GameObjects[i].id;
					}
					GameObject go = sceneFile.GameObjects[i];

					if (i == 0) { mainGo = go; }

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
					for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++)
					{
						sceneFile.GameObjects[i].components[j].awoken = false;
					}
				}
				UpdateSerializableTypes();

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

				xmlSerializer.Serialize(sw, sceneFile);

				for (int i = 0; i < sceneFile.GameObjects.Count; i++)
				{
					sceneFile.GameObjects[i].awoken = true;
					for (int j = 0; j < sceneFile.GameObjects[i].components.Count; j++)
					{
						sceneFile.GameObjects[i].components[j].awoken = true;
					}
				}
			}
		}
		public SceneFile LoadGameObjects(string scenePath)
		{
			using (StreamReader sr = new StreamReader(scenePath))
			{
				UpdateSerializableTypes();

				var bb = SerializableTypes.ToArray();

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneFile), SerializableTypes.ToArray());

				var a = ((SceneFile)xmlSerializer.Deserialize(sr));

				return a;
			}
		}
		public void ConnectParentsAndChildren(SceneFile sf, bool newIDs = false)
		{
			GameObject[] gos = sf.GameObjects.ToArray();
			Component[] comps = sf.Components.ToArray();

			int[] goIndexes = new int[gos.Length];
			for (int i = 0; i < goIndexes.Length; i++)
			{
				goIndexes[i] = -1;
			}

			int[] ogIDs = new int[gos.Length];
			for (int i = 0; i < ogIDs.Length; i++)
			{
				ogIDs[i] = gos[i].id;
			}
			for (int compIndex = 0; compIndex < comps.Length; compIndex++)
			{
				if (comps[compIndex].GetType() == typeof(Transform))
				{
					Transform tr = comps[compIndex] as Transform;
					for (int goIndex = 0; goIndex < gos.Length; goIndex++)
					{

						if (tr.parentID == ogIDs[goIndex])// found child/parent pair
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

								comps[compIndex].GameObject.id = IDsManager.gameObjectNextID;
								IDsManager.gameObjectNextID++;
							}
							(comps[compIndex] as Transform).SetParent(gos[goIndex].transform, false);
							(comps[compIndex] as Transform).parentID = gos[goIndex].id;
							continue;
						}
					}
				}
			}
			for (int goIndex = 0; goIndex < gos.Length; goIndex++)
			{
				if (goIndex == gos.Length - 1 && gos[goIndex].transform.children.Count == 0)
				{
					if (newIDs)
					{
						gos[goIndex].id = IDsManager.gameObjectNextID;
						IDsManager.gameObjectNextID++;
					}

					for (int i = 0; i < gos[goIndex].transform.children.Count; i++)
					{
						gos[goIndex].transform.children[i].parentID = gos[goIndex].id;
					}
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
				{
					if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() == typeof(Transform)) // add transforms first
					{
						gos[i].AddExistingComponent(comps[j]);
						gos[i].LinkComponents(gos[i], comps[j]);
					}
				}
				for (int j = 0; j < comps.Length; j++)
				{
					if (comps[j].gameObjectID == gos[i].id && comps[j].GetType() != typeof(Transform))
					{
						gos[i].AddExistingComponent(comps[j]);
						gos[i].LinkComponents(gos[i], comps[j]);
					}
				}
			}
		}
	}
}
