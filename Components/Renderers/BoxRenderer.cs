namespace Engine;

public class BoxRenderer : Renderer
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Render()
	{
		if (boxShape == null) return;

		ShaderCache.UseShader(ShaderCache.boxRendererShader);
		ShaderCache.boxRendererShader.SetMatrix4x4("u_mvp", LatestModelViewProjection);
		ShaderCache.boxRendererShader.SetColor("u_color", color.ToVector4());

		BufferCache.BindVAO(BufferCache.boxRendererVAO);

		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		Debug.CountStat("Draw Calls", 1);
	}
}