using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ImGuiNET;

namespace Engine
{
	public class Scene : Game
	{
		public static Scene I { get; private set; }

		public string scenePath = "";
		public TransformHandle transformHandle;
		public SpriteFont spriteFont;
		private Camera camera
		{
			get { return Camera.I; }
		}
		public List<GameObject> gameObjects = new List<GameObject>();

		public GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteBatch uiBatch;
		public event EventHandler<GameObject> GameObjectCreated;
		public event EventHandler<GameObject> GameObjectDestroyed;
		public event EventHandler SceneLoad;
		Serializer serializer;
		public event EventHandler<SceneData> SceneUpdated;

		Stopwatch updateStopwatch = new Stopwatch();
		Stopwatch renderStopwatch = new Stopwatch();

		public float updateTime = 0;
		public float renderTime = 0;

		public Scene()
		{
			I = this;
			serializer = new Serializer();
			IsMouseVisible = true;

			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1600,
				PreferredBackBufferHeight = 720,
				//PreferMultiSampling = true,
				SynchronizeWithVerticalRetrace = false,

				GraphicsProfile = GraphicsProfile.HiDef
			};
			graphics.ApplyChanges();
			this.IsFixedTimeStep = false;
			//Window.IsBorderless = true;
			Window.Position = new Point(0, 100);

			MouseInput.Mouse1Down += OnMouse1Clicked;
			MouseInput.Mouse1Up += OnMouse1Released;

			MouseInput.Mouse3Down += OnMouse3Clicked;
			MouseInput.Mouse3Up += OnMouse3Released;

			Content.RootDirectory = "Content";
		}
		private void CreateDefaultObjects()
		{
			//colliderEditor = new ColliderEditor();

			CreateTransformHandle();
			var CameraGO = GameObject.Create(name: "Camera");
			CameraGO.AddComponent<Camera>();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Awake();
			}
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
		public SpriteBatch CreateSpriteBatch()
		{
			return new SpriteBatch(GraphicsDevice);
		}
		protected override void Initialize()
		{

			Physics.Init();

			CreateDefaultObjects();


			TargetElapsedTime = TimeSpan.FromMilliseconds(15);

			Window.AllowUserResizing = true;
			spriteBatch = new SpriteBatch(GraphicsDevice);
			uiBatch = new SpriteBatch(GraphicsDevice);

			Editor.I.Init();

			if (Serializer.lastScene != "" && File.Exists(Serializer.lastScene))
			{
				LoadScene(Serializer.lastScene);
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteFont = Content.Load<SpriteFont>("font_Borda");
		}
		public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
		{
			//initialize a texture
			var texture = new Texture2D(device, width, height);

			//the array holds the color for each pixel in the texture
			Color[] data = new Color[width * height];
			for (var pixel = 0; pixel < data.Length; pixel++)
			{
				//the function applies the color according to the specified pixel
				data[pixel] = paint(pixel);
			}

			//set the color
			texture.SetData(data);

			return texture;
		}
		protected override void UnloadContent()
		{
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
			SceneFile sceneFile = Serializer.GetInstance().LoadGameObjects(path);

			Serializer.GetInstance().ConnectGameObjectsWithComponents(sceneFile);
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

			Serializer.GetInstance().SaveGameObjects(GetSceneFile(), path);
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

		private void OnMouse1Released()
		{
			//if (ColliderEditor.editing == true)
			//{
			//	return;
			//}
			if (true) //Tools.CurrentTool == Tools.ToolTypes.Select)
			{
				transformHandle.CurrentAxisSelected = null;
			}
		}
		private void OnMouse1Clicked()
		{
			//if (ColliderEditor.editing == true)
			//{
			//	return;
			//}
			//float minDistance = float.PositiveInfinity;
			//GameObject closestGameObject = null;
			//for (int i = 0; i < gameObjects.Count; i++)
			//{
			//	if (gameObjects[i] == transformHandle.GameObject || gameObjects[i].Active == false)
			//	{
			//		continue;
			//	}
			//	(bool intersects, float distance) detection = MouseInput.Position.In(gameObjects[i].GetComponent<Shape>());
			//	if (detection.distance < minDistance && detection.intersects)
			//	{
			//		closestGameObject = gameObjects[i];
			//		minDistance = detection.distance;
			//	}
			//	gameObjects[i].selected = false;
			//}
			transformHandle.clicked = false;
			if (MouseInput.Position.In(transformHandle.boxColliderX))
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.X;
				transformHandle.clicked = true;
			}
			if (MouseInput.Position.In(transformHandle.boxColliderY))
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.Y;
				transformHandle.clicked = true;
			}
			if (MouseInput.Position.In(transformHandle.boxColliderXY))
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.XY;
				transformHandle.clicked = true;
			}

			//if (closestGameObject != null && minDistance < 100)
			//{
			//	SelectGameObject(closestGameObject);
			//}
			//else if (transformHandle.clicked == false)
			//{
			//	// todo uncomment
			//	//transformHandle.SelectObject(null);
			//}
		}
		protected override void Update(GameTime gameTime)
		{
			//updateStopwatch.Start ();

			Time.Update(gameTime);
			//Physics.Step();

			MouseInput.Update(Mouse.GetState());

			if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyDown(Keys.S))
			{
				SaveScene();
			}
			if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyDown(Keys.R))
			{
				LoadScene(Serializer.lastScene);
			}

			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (Global.GameRunning || gameObjects[i].alwaysUpdate)
				{
					gameObjects[i].Update();
					gameObjects[i].FixedUpdate();
				}
			}

			SceneUpdated?.Invoke(this, new SceneData() { gameObjects = this.gameObjects });

			Editor.I.Update();

			base.Update(gameTime);

			//pdateStopwatch.Stop ();
			//pdateTime = updateStopwatch.ElapsedMilliseconds;
			//pdateStopwatch.Reset ();
		}
		public void SpriteBatch_Begin(BlendState blend)
		{
			spriteBatch.Begin(transformMatrix: camera.TransformMatrix, blendState: blend, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default);
		}
		protected override void Draw(GameTime gameTime)
		{
			if (camera?.renderTarget == null)
			{
				return;
			}
			renderStopwatch.Start();
			DrawSceneToTarget();

			GraphicsDevice.Clear(new Color(33, 36, 38));

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: Camera.I.effect);

			spriteBatch.Draw(texture: camera.renderTarget, destinationRectangleFloat: new RectangleFloat(Editor.gameViewPosition.X, Editor.gameViewPosition.Y, camera.renderTarget.Width, camera.renderTarget.Height), color: Color.White);

			spriteBatch.End();

			Editor.I.Draw(gameTime);

			base.Draw(gameTime);

			renderStopwatch.Stop();
			renderTime = renderStopwatch.ElapsedMilliseconds;
			renderStopwatch.Reset();
		}
		List<Renderer> sortedRenderers = new List<Renderer>();
		void DrawSceneToTarget()
		{
			GraphicsDevice.SetRenderTarget(camera.renderTarget);
			GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

			GraphicsDevice.Clear(camera.color);

			SpriteBatchCache.UpdateAllTransformMatrices();
			SpriteBatchCache.BeginAll();
			spriteBatch.Begin(transformMatrix: camera.TransformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default);

			sortedRenderers.Clear();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].GetComponent<Renderer>())
				{
					sortedRenderers.AddRange(gameObjects[i].GetComponents<Renderer>());
				}
				//gameObjects[i].Draw(spriteBatch);
			}
			sortedRenderers.Sort();

			for (int i = 0; i < sortedRenderers.Count; i++)
			{
				sortedRenderers[i].Draw(spriteBatch);
			}
			if (transformHandle.GameObject != null)
			{
				transformHandle.GameObject.Draw(spriteBatch);
			}
			spriteBatch.End();
			SpriteBatchCache.EndAll();

			GraphicsDevice.SetRenderTarget(null);
		}
	}
}