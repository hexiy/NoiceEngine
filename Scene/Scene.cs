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
	class Scene : GameWindow
	{
		ImGuiController _controller;

		public Scene() : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(1600, 900), APIVersion = new Version(4, 6) })
		{
			I = this;
			serializer = new Serializer();

			//MouseInput.Mouse3Down += OnMouse3Clicked;
			//MouseInput.Mouse3Up += OnMouse3Released;
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

			_controller = new ImGuiController(ClientSize.X, ClientSize.Y);


			Physics.Init();

			_ = new Editor();
			Editor.I.Init();

			CreateDefaultObjects();



			/*if (Serializer.lastScene != "" && File.Exists(Serializer.lastScene))
			{
				LoadScene(Serializer.lastScene);
			}*/
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			// Update the opengl viewport
			GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			// Tell ImGui of the new size
			_controller.WindowResized(ClientSize.X, ClientSize.Y);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			MouseInput.Update();

			GL.ClearColor(new Color4(0, 32, 48, 255));
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
			//GL.UseProgram(4);

			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Draw();
			}


			_controller.Update(this, (float)e.Time);

			Editor.I.Draw();
			//GL.UseProgram(0);
			_controller.Render();

			SwapBuffers();
		}

		protected override void OnTextInput(TextInputEventArgs e)
		{
			base.OnTextInput(e);


			_controller.PressChar((char)e.Unicode);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			_controller.MouseScroll(e.Offset);
		}

		public static Scene I { get; private set; }

		public string scenePath = "";
		public TransformHandle transformHandle;
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

		private void CreateDefaultObjects()
		{
			/*CreateTransformHandle();*/
			var camGO = GameObject.Create(name: "Camera");
			camGO.AddComponent<Camera>();
			camGO.Awake();

			GameObject go = GameObject.Create(name: "quad");
			go.AddComponent<QuadRenderer>();
			go.AddComponent<BoxShape>().size = new Vector2(100, 100);
			Rigidbody rb = go.AddComponent<Rigidbody>();
			go.AddComponent<Button>();
			rb.isButton = true;

			go.Awake();
			go.transform.position = new Vector2(20, 20);



			GameObject go2 = GameObject.Create(name: "quad 2");
			go2.AddComponent<QuadRenderer>();
			go2.GetComponent<QuadRenderer>().Color=Color.Magenta;
			go2.AddComponent<BoxShape>().size = new Vector2(150, 200);
			Rigidbody rb2 = go.AddComponent<Rigidbody>();
			go2.AddComponent<Button>();
			rb2.isButton = true;

			go2.Awake();
			go2.transform.position = new Vector2(150, 100);

			CreateTransformHandle();

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

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			Time.Update();
			//MouseInput.Update();

			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (Global.GameRunning || gameObjects[i].alwaysUpdate)
				{
					gameObjects[i].Update();
					gameObjects[i].FixedUpdate();
				}
			}

			SceneUpdated?.Invoke(this, new SceneData() { gameObjects = this.gameObjects });
			base.OnUpdateFrame(args);
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