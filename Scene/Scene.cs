


using GLFW;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static OpenGLMystery.OpenGL.GL;

//using ImGuiNET;

namespace Engine
{
	class Scene : Game
	{
		public static Scene I { get; private set; }

		public string scenePath = "";
		public TransformHandle transformHandle;
		public Camera2D cam;
		private Camera camera
		{
			get { return Camera.I; }
		}
		public List<GameObject> gameObjects = new List<GameObject>();

		public event EventHandler<GameObject> GameObjectCreated;
		public event EventHandler<GameObject> GameObjectDestroyed;
		public event EventHandler SceneLoad;
		Serializer serializer;
		public event EventHandler<SceneData> SceneUpdated;

		Stopwatch updateStopwatch = new Stopwatch();
		Stopwatch renderStopwatch = new Stopwatch();

		public float updateTime = 0;
		public float renderTime = 0;
		public Scene(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle) : base(initialWindowWidth, initialWindowHeight, initialWindowTitle)
		{
			I = this;
			serializer = new Serializer();

			MouseInput.Mouse3Down += OnMouse3Clicked;
			MouseInput.Mouse3Up += OnMouse3Released;
		}
		private void CreateDefaultObjects()
		{
			cam = new Camera2D(Vector2.Zero, 1f);

			GameObject go = GameObject.Create();
			go.AddComponent<BoxShape>();
			go.AddComponent<BoxRenderer>();

			go.Awake();
			go.transform.scale = new Vector2(300, 100);

			/*CreateTransformHandle();
			var CameraGO = GameObject.Create(name: "Camera");
			CameraGO.AddComponent<Camera>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Awake();
			}*/
		}
		void CreateTransformHandle()
		{
			GameObject transformHandleGameObject = GameObject.Create(_silent: true);
			transformHandle = transformHandleGameObject.AddComponent<TransformHandle>();
			transformHandleGameObject.dynamicallyCreated = true;
			transformHandleGameObject.alwaysUpdate = true;
			transformHandleGameObject.Name = "Transform Handle";
			transformHandleGameObject.Active = false;
			transformHandleGameObject.Awake();
		}
		public void SelectGameObject(GameObject go)
		{
			if (go != null)
			{
				for (int i = 0; i < gameObjects.Count; i++)
				{
					if (gameObjects[i].ID != go.ID)
					{
						gameObjects[i].selected = false;
					}
				}
				go.selected = true;
			}

			transformHandle.SelectObject(go);
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
		public void SelectGameObject(int gameObjectIndex)
		{
			if (gameObjectIndex == -1)
			{
				SelectGameObject(null);
			}
			else
			{
				SelectGameObject(gameObjects[gameObjectIndex]);
			}
		}
		protected override void Initialize()
		{
			Physics.Init();

			CreateDefaultObjects();

			//Editor.I.Init();

			/*if (Serializer.lastScene != "" && File.Exists(Serializer.lastScene))
			{
				LoadScene(Serializer.lastScene);
			}*/
		}
		public SceneFile GetSceneFile()
		{
			SceneFile sf = new SceneFile();
			sf.Components = new List<Component>();
			sf.GameObjects = new List<GameObject>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].dynamicallyCreated) continue;

				sf.Components.AddRange(gameObjects[i].Components);
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
		public int GetGameObjectIndex(int ID)
		{
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].ID == ID)
				{
					return i;
				}
			}
			return -1;
		}
		public GameObject GetGameObject(int ID)
		{
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].ID == ID)
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
				if (gameObjects[i].parentID == go.ID)
				{
					children.Add(gameObjects[i]);
				}
			}
			return children;
		}
		public void OnGameObjectCreated(GameObject gameObject)
		{
			gameObjects.Add(gameObject);

			GameObjectCreated?.Invoke(this, gameObject);
		}
		public bool LoadScene(string path = null)
		{
			//Add method to clean scene
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Destroy();
			}
			gameObjects.Clear();

			//Physics.rigidbodies.Clear();

			gameObjects = new List<GameObject>();
			SceneFile sceneFile = Serializer.I.LoadGameObjects(path);

			Serializer.I.ConnectGameObjectsWithComponents(sceneFile);
			IDsManager.gameObjectNextID = sceneFile.gameObjectNextID;

			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				Scene.I.OnGameObjectCreated(sceneFile.GameObjects[i]);

				sceneFile.GameObjects[i].Awake();
			}

			CreateTransformHandle();

			SceneLoad?.Invoke(this, null);

			scenePath = path;

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
			GameObjectDestroyed?.Invoke(this, gameObject);
		}

		private void OnMouse3Clicked()
		{
		}
		private void OnMouse3Released()
		{
		}


		protected override void Update()
		{
			Time.Update();
			MouseInput.Update();

			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (Global.GameRunning || gameObjects[i].alwaysUpdate)
				{
					gameObjects[i].transform.rotation.Z += Time.deltaTime*10;
					gameObjects[i].Update();
					gameObjects[i].FixedUpdate();
				}
			}

			SceneUpdated?.Invoke(this, new SceneData() { gameObjects = this.gameObjects });
		}
		protected override void Render()
		{
			glClearColor(0.11f, 0.11f, 0.11f, 0);
			glClear(GL_COLOR_BUFFER_BIT);

			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Draw();
			}

			Glfw.SwapBuffers(DisplayManager.Window);
		}

		protected override void LoadContent()
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
		//  mgremoval       
		//  mgremoval       
		//  mgremoval       	for (int i = 0; i < gameObjects.Count; i++)
		//  mgremoval       	{
		//  mgremoval       		if (Global.GameRunning || gameObjects[i].alwaysUpdate)
		//  mgremoval       		{
		//  mgremoval       			gameObjects[i].Update();
		//  mgremoval       			gameObjects[i].FixedUpdate();
		//  mgremoval       		}
		//  mgremoval       	}
		//  mgremoval       
		//  mgremoval       	SceneUpdated?.Invoke(this, new SceneData() { gameObjects = this.gameObjects });
		//  mgremoval       
		//  mgremoval       	Editor.I.Update();
		//  mgremoval       }
		//  mgremoval       protected override void Draw(GameTime gameTime)
		//  mgremoval       {
		//  mgremoval       	if (camera?.renderTarget == null)
		//  mgremoval       	{
		//  mgremoval       		return;
		//  mgremoval       	}
		//  mgremoval       	renderStopwatch.Start();
		//  mgremoval       	DrawSceneToTarget();
		//  mgremoval       
		//  mgremoval       	GraphicsDevice.Clear(new Color(33, 36, 38));
		//  mgremoval       
		//  mgremoval       	spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: Camera.I.effect);
		//  mgremoval       
		//  mgremoval       	spriteBatch.Draw(texture: camera.renderTarget, destinationRectangleFloat: new RectangleFloat(Editor.gameViewPosition.X, Editor.gameViewPosition.Y, camera.renderTarget.Width, camera.renderTarget.Height), color: Color.White);
		//  mgremoval       
		//  mgremoval       	spriteBatch.End();
		//  mgremoval       
		//  mgremoval       	Editor.I.Draw(gameTime);
		//  mgremoval       
		//  mgremoval       	base.Draw(gameTime);
		//  mgremoval       
		//  mgremoval       	renderStopwatch.Stop();
		//  mgremoval       	renderTime = renderStopwatch.ElapsedMilliseconds;
		//  mgremoval       	renderStopwatch.Reset();
		//  mgremoval       }
		//  mgremoval       void DrawSceneToTarget()
		//  mgremoval       {
		//  mgremoval       	GraphicsDevice.SetRenderTarget(camera.renderTarget);
		//  mgremoval       	GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
		//  mgremoval       
		//  mgremoval       	GraphicsDevice.Clear(camera.color);
		//  mgremoval       
		//  mgremoval       	SpriteBatchCache.UpdateAllTransformMatrices();
		//  mgremoval       	SpriteBatchCache.BeginAll();
		//  mgremoval       	spriteBatch.Begin(transformMatrix: camera.TransformMatrix, sortMode: SpriteSortMode.FrontToBack, blendState: null, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.DepthRead);
		//  mgremoval       
		//  mgremoval       	for (int i = 0; i < gameObjects.Count; i++)
		//  mgremoval       	{
		//  mgremoval       		gameObjects[i].Draw(spriteBatch);
		//  mgremoval       	}
		//  mgremoval       	if (transformHandle.GameObject != null)
		//  mgremoval       	{
		//  mgremoval       		transformHandle.GameObject.Draw(spriteBatch);
		//  mgremoval       	}
		//  mgremoval       	SpriteBatchCache.EndAll();
		//  mgremoval       	spriteBatch.End();
		//  mgremoval       
		//  mgremoval       	GraphicsDevice.SetRenderTarget(null);
		//  mgremoval       }


	}
}