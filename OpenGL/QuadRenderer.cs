using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;


namespace Engine
{
	public class QuadRenderer : Renderer
	{
		int vao;
		int vbo;
		public Shader shader;
		[LinkableComponent]
		public BoxShape boxShape;

		public unsafe override void Awake()
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

			float[] vertices =
	{
				-0.5f, 0.5f,			// top left
                0.5f, 0.5f,				// top right
                -0.5f, -0.5f,			// bottom left

                0.5f, 0.5f,				// top right
                0.5f, -0.5f,			// bottom right
                -0.5f, -0.5f,			// bottom left
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
		public unsafe void AwakeOld()
		{
			string vertexShader = @"#version 450 core
                                    layout (location = 0) in vec2 aPosition;

                                    out vec3 color;

									//uniform mat4 u_MVP;
									//uniform vec3 u_color;

                                    void main(void) 
                                    {
										//color = u_color;
										//color = vec3(1,1,1);
										gl_Position = u_MVP * aPosition;
                                    }";

			string fragmentShader = @"#version 450 core
                                    out vec3 FragColor;
                                    in vec3 color;

                                    void main(void) 
                                    {
                                       FragColor = vec3(1,1,1);
                                    }";

			shader = new Shader(vertexShader, fragmentShader);
			shader.Load();

			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();

			GL.BindVertexArray(vao);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			float[] vertices =
				{
				-0.5f, 0.5f,			// top left
                0.5f, 0.5f,				// top right
                -0.5f, -0.5f,			// bottom left

                0.5f, 0.5f,				// top right
                0.5f, -0.5f,			// bottom right
                -0.5f, -0.5f,			// bottom left
            };

			fixed (float* v = &vertices[0])
			{
				GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, (IntPtr)v, BufferUsageHint.StaticDraw);
			}

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (IntPtr)0);
			GL.EnableVertexArrayAttrib(0, 0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			GL.BindVertexArray(0);

		}
		public override void Update()
		{

			transform.position = MouseInput.Position;
			base.Update();
		}
		public override void Render()
		{
			shader.Use();

			//System.Numerics.Matrix4x4 _translation = System.Numerics.Matrix4x4.CreateTranslation(new Vector3(-1600 / 2, -900 / 2, 0)) * System.Numerics.Matrix4x4.CreateTranslation(transform.position);
			System.Numerics.Matrix4x4 _translation = System.Numerics.Matrix4x4.CreateTranslation(new Vector3(0, 0, 0)) * System.Numerics.Matrix4x4.CreateTranslation(transform.position);

			System.Numerics.Matrix4x4 _rotation = System.Numerics.Matrix4x4.CreateRotationZ(transform.rotation.Z / 180 * MathHelper.Pi * 4);
			System.Numerics.Matrix4x4 _scale = System.Numerics.Matrix4x4.CreateScale(boxShape.size.X * boxShape.transform.scale.X, boxShape.size.Y * boxShape.transform.scale.Y, 1);

			System.Numerics.Matrix4x4 _model = System.Numerics.Matrix4x4.Identity;

			System.Numerics.Matrix4x4 _view = System.Numerics.Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			System.Numerics.Matrix4x4 _projection = Camera.I.GetProjectionMatrix();//Matrix4x4.CreateOrthographicOffCenter(-2, 2, -1, 1, 0.001f, 1000f);
																				   //_projection = System.Numerics.Matrix4x4.CreateOrthographicOffCenter(-1280, 1280, -600, 600, 0.00001f, 10000000f);

			System.Numerics.Matrix4x4 _model_view_projection = _scale * _model * _translation * _view * _projection;
			//_model_view_projection = (_model * _scale) * _projection;



			shader.SetMatrix4x4("u_mvp", _model_view_projection);
			shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));


			GL.BindVertexArray(vao);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			GL.BindVertexArray(0);
		}
	}
}
