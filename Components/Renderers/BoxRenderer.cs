using System.IO;

namespace Engine;

public class BoxRenderer : Renderer
{
	public override void Awake()
	{
		base.Awake();
	}
	public override void CreateMaterial()
	{
		material = MaterialAssetManager.LoadMaterial("BoxMaterial.mat");
	}

	public override void Render()
	{
		if (boxShape == null || material == null)
		{
			return;
		}

		ShaderCache.UseShader(material.shader);
		material.shader.SetMatrix4x4("u_mvp", LatestModelViewProjection);
		material.shader.SetColor("u_color", color.ToVector4());

		BufferCache.BindVAO(material.vao);

		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		Debug.CountStat("Draw Calls", 1);
	}
}