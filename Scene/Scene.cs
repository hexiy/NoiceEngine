using OpenTK.Windowing.Common;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Dear_ImGui_Sample;

namespace Engine
{
	class Scene
	{
		public static Scene I { get; private set; }

		public string scenePath = "";
		public TransformHandle transformHandle;
		private Camera camera
		{
			get { return Camera.I; }
		}
		public List<GameObject> gameObjects = new List<GameObject>();
		//public event EventHandler SceneLoad;
		public event EventHandler<SceneData> SceneUpdated;

		public float updateTime = 0;
		public float renderTime = 0;

		public Scene()
		{
			I = this;
		}
		private void CreateDefaultObjects()
		{
			var camGO = GameObject.Create(name: "Camera");
			camGO.AddComponent<Camera>();
			camGO.Awake();

			for (int i = 0; i < 2; i++)
			{
				GameObject go2 = GameObject.Create(name: "sprite " + i);
				go2.AddComponent<SpriteRenderer>();
				go2.AddComponent<BoxShape>().size = new Vector2(288, 180);

				go2.Awake();
				go2.transform.position = new Vector2(300 + i * 10, 300 + i * 10);
				go2.transform.pivot = new Vector2(0.5f, 0.5f);
			}


			CreateTransformHandle();

		}
		void CreateTransformHandle()
		{
			GameObject transformHandleGameObject = GameObject.Create(_silent: true);
			transformHandle = transformHandleGameObject.AddComponent<TransformHandle>();
			transformHandleGameObject.dynamicallyCreated = true;
			transformHandleGameObject.alwaysUpdate = true;
			transformHandleGameObject.name = "Transform Handle";
			transformHandleGameObject.active = false;
			transformHandleGameObject.Awake();
		}
		public void Start()
		{
			//string str = "purpleblossom";
			//PersistentData.Set("mysteryKey", str);
			string hmmm = PersistentData.GetString("mysteryKey");
			Physics.Init();

			CreateDefaultObjects();



			if (Serializer.lastScene != "" && File.Exists(Serializer.lastScene))
			{
				LoadScene(Serializer.lastScene);
			}

		}
		public void Update()
		{
			Time.Update();
			MouseInput.Update();

			if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.LeftControl) && KeyboardInput.IsKeyDown(KeyboardInput.Keys.S))
			{
				SaveScene();
			}
			if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.LeftControl) && KeyboardInput.IsKeyDown(KeyboardInput.Keys.R))
			{
				LoadScene(Serializer.lastScene);
			}

			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].indexInHierarchy = i;
				if (Global.GameRunning || gameObjects[i].alwaysUpdate)
				{
					gameObjects[i].Update();
					gameObjects[i].FixedUpdate();
				}
			}

			SceneUpdated?.Invoke(this, new SceneData() { gameObjects = this.gameObjects });
		}
		List<Renderer> renderQueue;

		public void Render()
		{
			GL.ClearColor(Camera.I.color.ToOtherColor());
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			renderQueue = new List<Renderer>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].active)
				{
					renderQueue.AddRange(gameObjects[i].GetComponents<Renderer>());
				}
			}
			for (int i = 0; i < renderQueue.Count; i++)
			{
				renderQueue[i].layerFromHierarchy += renderQueue[i].GameObject.indexInHierarchy * 0.0001f;
			}
			renderQueue.Sort();

			for (int i = 0; i < renderQueue.Count; i++)
			{
				renderQueue[i].layerFromHierarchy -= renderQueue[i].GameObject.indexInHierarchy * 0.0001f;

				if (renderQueue[i].enabled && renderQueue[i].awoken)
				{
					renderQueue[i].Render();
				}
			}

			if (transformHandle.GameObject != null)
			{
				transformHandle.GameObject.Render();
			}
		}


		public void SelectGameObject(GameObject go)
		{
			if (go != null)
			{
				for (int i = 0; i < gameObjects.Count; i++)
				{
					if (gameObjects[i].id != go.id)
					{
						gameObjects[i].selected = false;
					}
				}
				go.selected = true;
			}
			if (go != camera.GameObject && go != transformHandle.GameObject)
			{
				transformHandle.SelectObject(go);
				PersistentData.Set("lastSelectedGameObjectId", go.id);
			}
			else
			{
				transformHandle.SelectObject(null);
			}
		}
		public void SelectGameObject(int id)
		{
			if (id == -1)
			{
				SelectGameObject(null);
			}
			else
			{
				if (GetGameObjectIndexInHierarchy(id) == -1) { return; }
				SelectGameObject(gameObjects[GetGameObjectIndexInHierarchy(id)]);
			}
		}
		public int GetGameObjectIndexInHierarchy(int id)
		{
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].id == id) { return i; }
			}
			return -1;
		}
		public List<GameObject> GetSelectedGameObjects()
		{
			List<GameObject> selectedGameObjects = new List<GameObject>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].selected) selectedGameObjects.Add(gameObjects[i]);
			}
			return selectedGameObjects;
		}
		public GameObject GetSelectedGameObject()
		{
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].selected) return gameObjects[i];
			}
			return null;
		}
		public SceneFile GetSceneFile()
		{
			SceneFile sf = new SceneFile();
			sf.Components = new List<Component>();
			sf.GameObjects = new List<GameObject>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].dynamicallyCreated) continue;

				sf.Components.AddRange(gameObjects[i].components);
				sf.GameObjects.Add(gameObjects[i]);
			}
			sf.gameObjectNextID = IDsManager.gameObjectNextID;
			return sf;
		}
		public GameObject FindGameObject(Type type)
		{
			foreach (var gameObject in gameObjects)
			{
				var bl = gameObject.GetComponent(type);
				if (bl != null)
				{
					return gameObject;
				}
			}
			return null;
		}
		public List<T> FindComponentsInScene<T>() where T : Component
		{
			List<T> components = new List<T>();
			foreach (var gameObject in gameObjects)
			{
				T bl = gameObject.GetComponent<T>();
				if (bl != null)
				{
					components.Add(bl);
				}
			}
			return components;
		}
		public GameObject GetGameObject(int ID)
		{
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].id == ID)
				{
					return gameObjects[i];
				}
			}
			return null;
		}
		public List<GameObject> GetChildrenOfGameObject(GameObject go)
		{
			List<GameObject> children = new List<GameObject>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].parentID == go.id)
				{
					children.Add(gameObjects[i]);
				}
			}
			return children;
		}
		public void AddGameObjectToScene(GameObject gameObject)
		{
			gameObjects.Add(gameObject);
		}
		public bool LoadScene(string path = null)
		{
			//Add method to clean scene
			while (gameObjects.Count > 0)
			{
				gameObjects[0].Destroy();
			}
			gameObjects.Clear();

			//Physics.rigidbodies.Clear();

			gameObjects = new List<GameObject>();
			SceneFile sceneFile = Serializer.I.LoadGameObjects(path);

			Serializer.I.ConnectGameObjectsWithComponents(sceneFile);
			IDsManager.gameObjectNextID = sceneFile.gameObjectNextID;

			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				Scene.I.AddGameObjectToScene(sceneFile.GameObjects[i]);

				sceneFile.GameObjects[i].Awake();
			}

			CreateTransformHandle();

			scenePath = path;

			int lastSelectedGameObjectId = PersistentData.GetInt("lastSelectedGameObjectId", 0);
			SelectGameObject(lastSelectedGameObjectId);
			EditorWindow_Hierarchy.I.SelectGameObject(lastSelectedGameObjectId);

			return true;
		}

		public void SaveScene(string path = null)
		{
			path = path ?? Serializer.lastScene;

			Serializer.I.SaveGameObjects(GetSceneFile(), path);
		}
		public void OnGameObjectDestroyed(GameObject gameObject)
		{
			if (gameObjects.Contains(gameObject))
			{
				gameObjects.Remove(gameObject);
			}
		}

		private void OnMouse3Clicked()
		{
		}
		private void OnMouse3Released()
		{
		}
		//  mgremoval       protected override void Update()
		//  mgremoval       {
		//  mgremoval       
		//  mgremoval       	Time.Update(gameTime);
		//  mgremoval       
		//  mgremoval       
		//  mgremoval       	MouseInput.Update(Mouse.GetState());
		//  mgremoval       
		//  mgremoval       	if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyDown(Keys.S))
		//  mgremoval       	{
		//  mgremoval       		SaveScene();
		//  mgremoval       	}
		//  mgremoval       	if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyDown(Keys.R))
		//  mgremoval       	{
		//  mgremoval       		LoadScene(Serializer.lastScene);
		//  mgremoval       	}
		//  mgremoval       	if (KeyboardInput.IsKeyDown(Keys.O) && GetSelectedGameObjects() != null)
		//  mgremoval       	{
		//  mgremoval       		serializer.SaveGameObject(GetSelectedGameObject(), "Prefabs/yo.prefab");
		//  mgremoval       	}
		//  mgremoval       	if (KeyboardInput.IsKeyDown(Keys.P))
		//  mgremoval       	{
		//  mgremoval       		GameObject go = serializer.LoadGameObject("Prefabs/yo.prefab");
		//  mgremoval       	}


	}
}