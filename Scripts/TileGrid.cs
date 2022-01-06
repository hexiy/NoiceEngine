using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scripts
{
	public class TileGrid : Component
	{
		[ShowInEditor] public Vector2 size { get; set; }

		[XmlIgnore] public List<GameObject> tiles = new List<GameObject>();
		[XmlIgnore] public List<float> tilesOGPositionsY = new List<float>();

		public override void Start()
		{
			playerSmoothPosition = Player.I.transform.position;
			if (tiles.Count == 0)
			{
				for (int x = 0; x < size.X; x++)
				{
					for (int y = 0; y < size.Y; y++)
					{
						GameObject tile = GameObject.Create(name: "Tile_" + y);
						tile.dynamicallyCreated = true;

						tile.AddComponent<SpriteSheetRenderer>();

						SpriteSheetRenderer spriteSheetRenderer = tile.GetComponent<SpriteSheetRenderer>();
						tile.Awake();
						spriteSheetRenderer.blendState = BlendState.Opaque;
						tile.SetParent(GameObject);
						tile.transform.localPosition = new Vector3(x * 3.2f, y * 3.2f, 0);
						tile.transform.scale = new Vector3(0.1f, 0.1f, 0.1f);

						spriteSheetRenderer.LoadTexture("2D/tiles.png");
						spriteSheetRenderer.SpritesCount = new Vector2(16, 16);
						if (y == 0)
						{
							if (x == 0)
							{
								spriteSheetRenderer.CurrentSpriteIndex = 0;
							}
							else if (x == size.X - 1)
							{
								spriteSheetRenderer.CurrentSpriteIndex = 2;
							}
							else
							{
								spriteSheetRenderer.CurrentSpriteIndex = 1;
							}
						}
						else
						{
							spriteSheetRenderer.CurrentSpriteIndex = 17;
						}
						tiles.Add(tile);
						tilesOGPositionsY.Add(tile.transform.position.Y);
					}
				}
			}
		}
		//public override void PreSceneSave()
		//{
		//	while (tiles.Count > 0)
		//	{
		//		tiles[0].Destroy();
		//		tiles.RemoveAt(0);
		//	}
		//	base.OnDestroyed();
		//}
		public override void Update()
		{
			if (KeyboardInput.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				for (int i = Scene.I.gameObjects.Count - 1; i > 0; i--)
				{
					if (Scene.I.gameObjects[i].Name.ToLower().Contains("tile") && Scene.I.gameObjects[i].GetComponent<TileGrid>() == null)
					{
						Scene.I.gameObjects[i].Destroy();
					}
				}
				tiles.Clear();
			}
			//UpdateFallingGroundPosition();
			UpdateLighting();

			base.Update();
		}
		Vector2 playerSmoothPosition;
		void UpdateLighting()
		{
			if (Player.I == null) return;

			playerSmoothPosition = Vector2.Lerp(playerSmoothPosition, Player.I.transform.position, Time.deltaTime * 7);
			for (int i = 0; i < tiles.Count; i++)
			{
				float distanceFromPlayer = Vector2.Distance(playerSmoothPosition.TranslateToGrid(2.4f), tiles[i].transform.position) + MathF.Sin(Time.elapsedTime * 1f) * 1;

				tiles[i].GetComponent<Renderer>().Color = new Color(Color.White * MathHelper.Clamp((1 / (distanceFromPlayer * 0.12f)), 0, 0.9f), 255);

			}
		}
		void UpdateFallingGroundPosition()
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				float distanceFromPlayerOnXAxis = MathF.Abs(tiles[i].transform.position.X - Player.I.transform.position.X);
				float goDown;
				if (distanceFromPlayerOnXAxis < 22)
				{
					goDown = 0;
				}
				else
				{
					goDown = distanceFromPlayerOnXAxis - 22;
				}

				tiles[i].transform.position.Y = MathHelper.Lerp(tiles[i].transform.position.Y, tilesOGPositionsY[i] + goDown, Time.deltaTime * (11 + MathF.Abs(Player.I.GetComponent<Rigidbody>().Velocity.X) * 2));

			}
		}
	}
}
