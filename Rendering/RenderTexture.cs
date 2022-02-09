using System.Numerics;

namespace Engine;

public class RenderTexture
{
	public int id;
	public int colorAttachment;

	internal int vao;
	internal int vbo;
	public Shader shader;


	public RenderTexture()
	{
		GL.DeleteFramebuffers(1, ref id);
		Invalidate();
		InitRenderer();
	}
	public void Invalidate()
	{
		id = GL.GenFramebuffer();

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);

		GL.CreateTextures(TextureTarget.Texture2D, 1, out colorAttachment);

		GL.BindTexture(TextureTarget.Texture2D, colorAttachment);

		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, (int)Camera.I.size.X, (int)Camera.I.size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)null);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorAttachment, 0);

		if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
		{
			Debug.Log("RENDER TEXTURE ERROR");
		}

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void Bind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
	}
	public void Unbind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	private void InitRenderer()
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
layout(location = 0) out vec4 color;

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y));

vec2 relativePosition = texCoord.xy/vec2(1,1) - 0.5;
float len= length(relativePosition);
float vignette = smoothstep(.9,.2,len);
texColor.rgb=mix(texColor.rgb,texColor.rgb*vignette, .7);

texColor.a=1;



 color = texColor;
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
	public void RenderWithPostProcess(int sceneColorAttachment)
	{
		shader.Use();
		shader.SetMatrix4x4("u_mvp", GetModelViewProjection());

		GL.BindVertexArray(vao);
		GL.Enable(EnableCap.Blend);


		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		GL.BindTexture(TextureTarget.Texture2D, sceneColorAttachment);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

		GL.BindVertexArray(0);
		GL.Disable(EnableCap.Blend);
	}

	public Matrix4x4 GetModelViewProjection()
	{
		return Matrix4x4.CreateScale(2, 2, 1);
	}
}
