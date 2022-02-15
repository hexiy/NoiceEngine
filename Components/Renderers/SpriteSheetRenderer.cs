using System.Xml.Serialization;

namespace Scripts;

public class SpriteSheetRenderer : SpriteRenderer
{
	private Vector2 spritesCount = new Vector2(1, 1);

	public Vector2 SpritesCount
	{
		get => spritesCount;
		set
		{
			spritesCount = value;
			/*if (texture != null)
			{
				SpriteSize = new Vector2(texture.Width / SpritesCount.X, texture.Height / SpritesCount.Y);
			}*/
		}
	}

	public int currentSpriteIndex;

	public Vector2 spriteSize;

	public override void Awake()
	{
		drawOffset = new Vector2(0, spriteSize.Y * spritesCount.Y - spriteSize.Y);

		base.Awake();
	}
	internal override void UpdateBoxShapeSize()
	{
	}
	public override void OnNewComponentAdded(Component comp)
	{
	}
	[Show] public Vector2 drawOffset = Vector2.Zero;
	[Show] public Vector2 u_scale = Vector2.Zero;
	public override void Render()
	{
		if (onScreen == false) return;
		if (boxShape == null) return;
		if (texture.loaded == false) return;

		ShaderCache.UseShader(ShaderCache.spriteSheetRendererShader);
		ShaderCache.spriteSheetRendererShader.SetVector2("u_resolution", texture.size);
		ShaderCache.spriteSheetRendererShader.SetMatrix4x4("u_mvp", LatestModelViewProjection);
		ShaderCache.spriteSheetRendererShader.SetColor("u_color", color.ToVector4());
		ShaderCache.spriteSheetRendererShader.SetVector2("u_scale", u_scale);


		float x = (currentSpriteIndex) % spritesCount.X;
		float y = (float)Math.Floor((float)currentSpriteIndex / spritesCount.X);

		drawOffset = new Vector2(x, y) * spriteSize*spritesCount;
		//drawOffset = new Vector2(100,0);

		ShaderCache.spriteSheetRendererShader.SetVector2("u_offset", drawOffset);

		BufferCache.BindVAO(BufferCache.spriteSheetRendererVAO);

		if (additive)
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusConstantColor);
		}
		else
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}
		TextureCache.BindTexture(texture.id);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		//BufferCache.BindVAO(0);
		//GL.Disable(EnableCap.Blend);

		Debug.CountStat("Draw Calls", 1);
	}
}
/*public override void OnTextureLoaded(Texture2D _texture, string _path)
{
	SpriteSize = new Vector2(_texture.Width / SpritesCount.X, _texture.Height / SpritesCount.Y);

	base.OnTextureLoaded(_texture, _path);
}*/
