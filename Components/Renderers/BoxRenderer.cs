using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;


namespace Engine
{
	public class BoxRenderer : Renderer
	{
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

			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);


			//float[] vertices = new float[]
			//			{
			//			-0.5f, 0.5f,
			//			0.5f, 0.5f,
			//			-0.5f, -0.5f,
			//
			//			0.5f, 0.5f,
			//			0.5f, -0.5f,
			//			-0.5f, -0.5f
			//		};

			float[] vertices =
	{
				-0.5f, -0.5f,
				0.5f,-0.5f,
				-0.5f,0.5f,

				-0.5f,0.5f,
				0.5f,-0.5f,
				0.5f, 0.5f
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
				2,                      // size of attribute, vec4
				VertexAttribType.Float, // contains floats
				false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
				0);                     // relative offset, first item

			GL.VertexArrayVertexBuffer(vao, 0, vbo, IntPtr.Zero, sizeof(float) * 2);
		}
		public override void Render()
		{
			if (boxShape == null) return;

			shader.Use();

			shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
			//shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
			shader.SetVector4("u_color", color.ToVector4());

			GL.BindVertexArray(vao);

			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

			GL.BindVertexArray(0);
		}
	}
}
