using System.Xml.Serialization;

namespace Engine;

public class BoxRenderer : Renderer
{
	[XmlIgnore] public static Shader shader;

	public override void Awake()
	{
		SetupRenderer();

		base.Awake();
	}
	private void SetupRenderer()
	{
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;
uniform mat4 u_mvp = mat4(1.0);
out vec4 frag_color;
uniform vec4 u_color;
void main(void)
{
gl_Position = u_mvp * position;
frag_color = u_color;
}";

		string fragmentShader = @"#version 460 core
in vec4 frag_color;
out vec4 color;

void main(void)
{
color = frag_color;
}";

		shader = new Shader(vertexShader, fragmentShader);
		shader.Load();
	}
	public override void Render()
	{
		if (boxShape == null) return;

		shader.Use();

		shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
		//shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
		shader.SetVector4("u_color", color.ToVector4());

		GL.BindVertexArray(RenderBuffers.boxRendererVAO);

		GL.Enable(EnableCap.Blend);

		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		GL.Disable(EnableCap.Blend);
		GL.BindVertexArray(0);
	}
}
