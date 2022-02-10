using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Engine;

public class SpriteRenderer : Renderer
{
	[XmlIgnore] public static Shader shader;

	public Texture texture;
	public bool additive;

	private bool onScreen = true;
	public static bool setup = false;
	public override void Awake()
	{
		if (setup == false)
		{
			SetupRenderer();
		}

		if (texture == null)
		{
			texture = new Texture();
		}
		else
		{
			LoadTexture(texture.path);
		}
		base.Awake();
	}
	private static void SetupRenderer()
	{
		setup = true;
		string vertexShader = @"#version 460 core

layout(location = 0) in vec4 position;

layout(location = 1) in vec4 aTexCoord;

out vec4 texCoord;
out vec4 frag_color;
uniform mat4 u_mvp = mat4(1.0);

void main(void)
{

    texCoord = aTexCoord;

    gl_Position = u_mvp * position;
}";

		string fragmentShader = @"#version 450 core
in vec4 texCoord;
uniform sampler2D textureObject;
uniform vec4 u_color;
layout(location = 0) out vec4 color;

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y)) * u_color;
if(texColor.a < 0.1){
        discard;
}
else{
 color = texColor;
}
}";

		shader = new Shader(vertexShader, fragmentShader);

		shader.Load();

	}
	public void LoadTexture(string _texturePath)
	{
		if (_texturePath.Contains("Assets") == false)
		{
			_texturePath = Path.Combine("Assets", _texturePath);
		}
		if (File.Exists(_texturePath) == false) { return; }

		texture.Load(_texturePath);

		UpdateBoxShapeSize();
	}
	private void UpdateBoxShapeSize()
	{
		if (boxShape != null)
		{
			boxShape.size = texture.size;
		}
	}
	public override void OnNewComponentAdded(Component comp)
	{
		if (comp is BoxShape && texture != null)
		{
			UpdateBoxShapeSize();
		}
		base.OnNewComponentAdded(comp);
	}
	public override void Update()
	{
		if (Time.elapsedTicks % 10 == 0) onScreen = Camera.I.RectangleVisible(boxShape);

		//if (RectA.Left < RectB.Right && RectA.Right > RectB.Left &&
		//RectA.Top > RectB.Bottom && RectA.Bottom < RectB.Top ) 
		//
		base.Update();
	}
	public override void Render()
	{
		if (onScreen == false) return;
		if (boxShape == null) return;
		if (texture.loaded == false) return;

		shader.Use();
		shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
		shader.SetVector4("u_color", color.ToVector4());

		GL.BindVertexArray(RenderBuffers.spriteRendererVAO);
		GL.Enable(EnableCap.Blend);

		if (additive)
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusConstantColor);
		}
		else
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}
		texture.Use();

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		GL.BindVertexArray(0);
		GL.Disable(EnableCap.Blend);

		Debug.CountStat("Draw Calls", 1);
	}
}
// STENCIL working

/*
public override void Render()
		{
			if (boxShape == null) return;
			if (texture.loaded == false) return;
			// stencil experiment
			GL.Enable(EnableCap.StencilTest);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
			GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
			GL.StencilMask(0xFF);
			shader.Use();
			shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
			shader.SetVector4("u_color", color.ToVector4());

			GL.BindVertexArray(vao);
			GL.Enable(EnableCap.Blend);

			if (additive)
			{
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusConstantColor);
			}
			else
			{
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			}
			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			// stencil after
			GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
			GL.StencilMask(0x00);
			GL.Disable(EnableCap.DepthTest);

			shader.Use();
			shader.SetMatrix4x4("u_mvp", GetModelViewProjectionForOutline(thickness));
			//shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
			shader.SetVector4("u_color", Color.Black.ToVector4());

			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

			GL.StencilMask(0xFF);
			GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
			GL.Enable(EnableCap.DepthTest);

			GL.BindVertexArray(0);
			GL.Disable(EnableCap.Blend);
		}
*/