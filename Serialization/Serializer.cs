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
			get { return PersistentData.GetString("lastOpenedScene", "scene1.scene"); }
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


				GameObject go = sceneFile.GameObjects[0];
				go.id = IDsManager.gameObjectNextID;
				IDsManager.gameObjectNextID++;

				for (int i = 0; i < go.components.Count; i++)
				{
					go.components[i].gameObjectID = go.id;
				}


				Scene.I.AddGameObjectToScene(go);

				go.Awake();

				return go;
			}
		}
		public void SaveGameObjects(SceneFile sceneFile, string scenePath)
		{
			lastScene = scenePath;

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
			return;
			for (int i = 0; i < gos.Length; i++)
			{
				for (int j = 0; j < gos[i].components.Count; j++)
				{
					gos[i].InitializeMemberComponents(gos[i].components[j]);

					gos[i].LinkComponents(gos[i], gos[i].components[j]);

					gos[i].components[j].GameObject = gos[i];
					gos[i].components[j].transform.GameObject = gos[i];

					gos[i].components[j].Awake();
					gos[i].components[j].awoken = true;

					gos[i].components[j].Start();


				}
			}
		}
	}
}
