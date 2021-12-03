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
			get { return Camera.Instance; }
		}
		public List<GameObject> gameObjects = new List<GameObject> ();

		public GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteBatch uiBatch;
		public event EventHandler<GameObject> GameObjectCreated;
		public event EventHandler<GameObject> GameObjectDestroyed;
		public event EventHandler SceneLoad;

		public event EventHandler<SceneData> SceneUpdated;

		Stopwatch updateStopwatch = new Stopwatch ();
		Stopwatch renderStopwatch = new Stopwatch ();

		public float updateTime = 0;
		public float renderTime = 0;

		public Scene ()
		{
			I = this;

			IsMouseVisible = true;

			graphics = new GraphicsDeviceManager (this)
			{
				PreferredBackBufferWidth = 1600,
				PreferredBackBufferHeight = 720,
				//PreferMultiSampling = true,
				SynchronizeWithVerticalRetrace = false,

				//GraphicsProfile = GraphicsProfile.HiDef
			};
			graphics.ApplyChanges ();
			this.IsFixedTimeStep = false;
			//Window.IsBorderless = true;
			Window.Position = new Point (0, 100);

			MouseInput.Mouse1Down += OnMouse1Clicked;
			MouseInput.Mouse1Up += OnMouse1Released;

			MouseInput.Mouse3Down += OnMouse3Clicked;
			MouseInput.Mouse3Up += OnMouse3Released;

			Content.RootDirectory = "Content";
		}
		private void CreateDefaultObjects ()
		{
			//colliderEditor = new ColliderEditor();

			CreateTransformHandle ();
			var CameraGO = GameObject.Create (name: "Camera");
			CameraGO.AddComponent<Camera> ();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Awake ();
			}
			GameObject image = GameObject.Create (name: "Image");
			image.AddComponent<BoxShape> ();
			image.GetComponent<BoxShape> ().Size = new Vector2 (30, 30);
			var renderer = image.AddComponent<BoxRenderer> ();
			renderer.Fill = true;

			var text = image.AddComponent<Text> ();
			text.Value = "owo";

			image.AddComponent<TextRenderer> ();

			image.AddComponent<Button> ();
			image.Awake ();
			image.transform.anchor = new Vector2 (0.5f, 0.5f);

			image.transform.position = new Vector2 ((int) Camera.Instance.Size.X / 2, (int) Camera.Instance.Size.Y / 2);
		}
		void CreateTransformHandle ()
		{
			GameObject transformHandleGameObject = GameObject.Create (_silent: true);
			transformHandle = transformHandleGameObject.AddComponent<TransformHandle> ();
			transformHandleGameObject.Name = "Transform Handle";
			transformHandleGameObject.Active = false;
			transformHandleGameObject.Awake ();
		}
		public void SelectGameObject (GameObject go)
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

			transformHandle.SelectObject (go);
		}
		public List<GameObject> GetSelectedGameObjects ()
		{
			List<GameObject> selectedGameObjects = new List<GameObject> ();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i].selected) selectedGameObjects.Add (gameObjects[i]);
			}
			return selectedGameObjects;
		}
		public void SelectGameObject (int gameObjectIndex)
		{
			if (gameObjectIndex == -1)
			{
				SelectGameObject (null);
			}
			else
			{
				SelectGameObject (gameObjects[gameObjectIndex]);
			}
		}
		public SpriteBatch CreateSpriteBatch ()
		{
			return new SpriteBatch (GraphicsDevice);
		}
		protected override void Initialize ()
		{
			CreateDefaultObjects ();

			TargetElapsedTime = TimeSpan.FromMilliseconds (15);

			Window.AllowUserResizing = true;
			spriteBatch = new SpriteBatch (GraphicsDevice);
			uiBatch = new SpriteBatch (GraphicsDevice);

			Editor.I.Init ();

			if (Serializer.lastScene != "" && File.Exists (Serializer.lastScene))
			{
				LoadScene (Serializer.lastScene);
			}
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteFont = Content.Load<SpriteFont> ("font_Borda");
		}
		public static Texture2D CreateTexture (GraphicsDevice device, int width, int height, Func<int, Color> paint)
		{
			//initialize a texture
			var texture = new Texture2D (device, width, height);

			//the array holds the color for each pixel in the texture
			Color[] data = new Color[width * height];
			for (var pixel = 0; pixel < data.Length; pixel++)
			{
				//the function applies the color according to the specified pixel
				data[pixel] = paint (pixel);
			}

			//set the color
			texture.SetData (data);

			return texture;
		}
		protected override void UnloadContent ()
		{
		}
		public SceneFile GetSceneFile ()
		{
			SceneFile sf = new SceneFile ();
			sf.Components = new List<Component> ();
			sf.GameObjects = new List<GameObject> ();
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i] == transformHandle.GameObject) continue;
				sf.Components.AddRange (gameObjects[i].Components);
				sf.GameObjects.Add (gameObjects[i]);
			}
			sf.gameObjectNextID = IDsManager.gameObjectNextID;
			return sf;
		}
		public GameObject FindGameObject (Type type)
		{
			foreach (var gameObject in gameObjects)
			{
				var bl = gameObject.GetComponent (type);
				if (bl != null)
				{
					return gameObject;
				}
			}
			return null;
		}
		public int GetGameObjectIndex (int ID)
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
		public void OnGameObjectCreated (GameObject gameObject)
		{
			gameObjects.Add (gameObject);

			GameObjectCreated?.Invoke (this, gameObject);
		}
		public bool LoadScene (string path = null)
		{
			//Add method to clean scene
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Destroy ();
			}
			gameObjects.Clear ();

			//Physics.rigidbodies.Clear();

			gameObjects = new List<GameObject> ();
			SceneFile sceneFile = Serializer.GetInstance ().LoadGameObjects (path);

			Serializer.GetInstance ().ConnectGameObjectsWithComponents (sceneFile);
			IDsManager.gameObjectNextID = sceneFile.gameObjectNextID;

			for (int i = 0; i < sceneFile.GameObjects.Count; i++)
			{
				Scene.I.OnGameObjectCreated (sceneFile.GameObjects[i]);

				sceneFile.GameObjects[i].Awake ();
			}

			CreateTransformHandle ();

			SceneLoad?.Invoke (this, null);

			scenePath = path;

			return true;
		}
		public void OnGameObjectDestroyed (GameObject gameObject)
		{
			if (gameObjects.Contains (gameObject))
			{
				gameObjects.Remove (gameObject);
			}
			GameObjectDestroyed?.Invoke (this, gameObject);
		}

		private void OnMouse3Clicked ()
		{
		}
		private void OnMouse3Released ()
		{
		}

		private void OnMouse1Released ()
		{
			if (ColliderEditor.editing == true)
			{
				return;
			}
			if (true) //Tools.CurrentTool == Tools.ToolTypes.Select)
			{
				transformHandle.CurrentAxisSelected = null;
			}
		}
		private void OnMouse1Clicked ()
		{
			if (ColliderEditor.editing == true)
			{
				return;
			}
			float minDistance = float.PositiveInfinity;
			GameObject closestGameObject = null;
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (gameObjects[i] == transformHandle.GameObject || gameObjects[i].Active == false)
				{
					continue;
				}
				(bool intersects, float distance) detection = MouseInput.Position.In (gameObjects[i].GetComponent<Shape> ());
				if (detection.distance < minDistance && detection.intersects)
				{
					closestGameObject = gameObjects[i];
					minDistance = detection.distance;
				}
				gameObjects[i].selected = false;
			}
			transformHandle.clicked = false;
			if (MouseInput.Position.In (transformHandle.boxColliderX).intersects)
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.X;
				transformHandle.clicked = true;
			}
			if (MouseInput.Position.In (transformHandle.boxColliderY).intersects)
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.Y;
				transformHandle.clicked = true;
			}
			if (MouseInput.Position.In (transformHandle.boxColliderXY).intersects)
			{
				transformHandle.CurrentAxisSelected = TransformHandle.Axis.XY;
				transformHandle.clicked = true;
			}

			if (closestGameObject != null && minDistance < 100)
			{
				SelectGameObject (closestGameObject);
			}
			else if (transformHandle.clicked == false)
			{
				// todo uncomment
				//transformHandle.SelectObject(null);
			}
		}
		protected override void Update (GameTime gameTime)
		{
			//updateStopwatch.Start ();

			if (Global.GameRunning == false)
			{
				return;
			}
			Time.Update(gameTime);

			MouseInput.Update (Mouse.GetState ());

			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Update ();
			}

			SceneUpdated?.Invoke (this, new SceneData () {gameObjects = this.gameObjects});

			Editor.I.Update ();

			base.Update (gameTime);

			//pdateStopwatch.Stop ();
			//pdateTime = updateStopwatch.ElapsedMilliseconds;
			//pdateStopwatch.Reset ();
		}
		protected override void Draw (GameTime gameTime)
		{
			if (camera?.renderTarget == null)
			{
				return;
			}
			renderStopwatch.Start ();
			DrawSceneToTarget ();

			GraphicsDevice.Clear (new Color (33, 36, 38));

			spriteBatch.Begin (SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, effect: Camera.Instance.effect);

			spriteBatch.Draw (texture: camera.renderTarget, destinationRectangle: new Rectangle ((int) Editor.gameViewPosition.X, (int) Editor.gameViewPosition.Y, camera.renderTarget.Width, camera.renderTarget.Height), color: Color.White);

			spriteBatch.End ();

			Editor.I.Draw (gameTime);

			base.Draw (gameTime);

			renderStopwatch.Stop ();
			renderTime = renderStopwatch.ElapsedMilliseconds;
			renderStopwatch.Reset ();
		}
		void DrawSceneToTarget ()
		{
			GraphicsDevice.SetRenderTarget (camera.renderTarget);
			GraphicsDevice.DepthStencilState = new DepthStencilState () {DepthBufferEnable = true};

			GraphicsDevice.Clear (camera.color);

			spriteBatch.Begin (transformMatrix: camera.TranslationMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default);
			for (int i = 0; i < gameObjects.Count; i++)
			{
				gameObjects[i].Draw (spriteBatch);
			}
			if (transformHandle.GameObject != null)
			{
				transformHandle.GameObject.Draw (spriteBatch);
			}
			spriteBatch.End ();

			GraphicsDevice.SetRenderTarget (null);
		}
	}
}