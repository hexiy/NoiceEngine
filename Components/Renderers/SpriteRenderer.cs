using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace Engine
{
	public class SpriteRenderer : Renderer
	{
		[System.Xml.Serialization.XmlIgnore] public Texture texture;
		public override void Awake()
		{
			SetupRenderer();

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
out vec4 color;

void main(void)
{
vec4 texColor =texture(textureObject, vec2(texCoord.x, texCoord.y)) * u_color;
if(texColor.a < 0.1)
        discard;
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

			LoadTexture("2D/bg.png");
		}
		public void LoadTexture(string _texturePath)
		{
			if (File.Exists(_texturePath) == false) { return; }
			texture = new Texture(_texturePath);
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
		public override void Render()
		{
			if (boxShape == null) return;
			shader.Use();

			shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
			//shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
			shader.SetVector4("u_color", color.ToVector4());

			GL.BindVertexArray(vao);
			//GL.BlendEquation(BlendEquationMode.);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			//GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			GL.BindVertexArray(0);
			GL.Disable(EnableCap.Blend);

		}
	}
}
