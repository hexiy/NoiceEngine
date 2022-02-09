using System.IO;

namespace Engine;

public class SpriteRenderer : Renderer
{
	public Texture texture;
	public bool additive;

	public override void Awake()
	{
		SetupRenderer();

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
	private void SetupRenderer()
	{
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
		vao = GL.GenVertexArray();
		vbo = GL.GenBuffer();

		GL.BindVertexArray(vao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

		float[] vertices =
{
				-0.5f, -0.5f,  0,0,
				0.5f,-0.5f,    1,0,
				-0.5f,0.5f,    0,1,

				-0.5f,0.5f,    0,1,
				0.5f,-0.5f,    1,0,
				0.5f, 0.5f,    1,1

			};

		GL.NamedBufferStorage(
			vbo,
			sizeof(float) * vertices.Length,
			vertices,
			 BufferStorageFlags.MapWriteBit);

		GL.VertexArrayAttribBinding(vao, 0, 0);
		GL.EnableVertexArrayAttrib(vao, 0);
		GL.VertexArrayAttribFormat(
			vao,
			0,                      // attribute index, from the shader location = 0
			2,                      // size of attribute, vec2
			VertexAttribType.Float, // contains floats
			false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
			0);                     // relative offset, first item
		GL.VertexArrayAttribBinding(vao, 1, 0);
		GL.EnableVertexArrayAttrib(vao, 1);
		GL.VertexArrayAttribFormat(
			vao,
			1,                      // attribute index, from the shader location = 0
			2,                      // size of attribute, vec2
			VertexAttribType.Float, // contains floats
			true,                  // does not need to be normalized as it is already, floats ignore this flag anyway
			8);                     // relative offset, first item

		GL.VertexArrayVertexBuffer(vao, 0, vbo, IntPtr.Zero, sizeof(float) * 4);
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
	private float thickness = 30;
	public override void Update()
	{
		thickness = (float)Math.Sin(Time.deltaTime * 3) * 30 + 50;
		base.Update();
	}
	public override void Render()
	{
		if (boxShape == null) return;
		if (texture.loaded == false) return;

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

		GL.BindVertexArray(0);
		GL.Disable(EnableCap.Blend);
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