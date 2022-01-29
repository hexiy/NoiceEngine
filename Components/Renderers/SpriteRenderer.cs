using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;


namespace Engine
{
	public class SpriteRenderer : Renderer
	{
		int vao;
		int vbo;
		public Shader shader;
		[LinkableComponent]
		public BoxShape boxShape;

		public Texture texture;

		public unsafe override void Awake()
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

 color = texture(textureObject, vec2(texCoord.x,texCoord.y))*u_color;
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
			texture = new Texture("2D/bg.png");
		}
		public override void Render()
		{
			shader.Use();

			//System.Numerics.Matrix4x4 _translation = System.Numerics.Matrix4x4.CreateTranslation(new Vector3(-1600 / 2, -900 / 2, 0)) * System.Numerics.Matrix4x4.CreateTranslation(transform.position);
			System.Numerics.Matrix4x4 _translation = System.Numerics.Matrix4x4.CreateTranslation(transform.position + boxShape.offset);

			System.Numerics.Matrix4x4 _rotation = System.Numerics.Matrix4x4.CreateRotationZ(transform.rotation.Z / 180 * MathHelper.Pi * 4);
			System.Numerics.Matrix4x4 _scale = System.Numerics.Matrix4x4.CreateScale(boxShape.size.X * boxShape.transform.scale.X, boxShape.size.Y * boxShape.transform.scale.Y, 1);

			System.Numerics.Matrix4x4 _model = System.Numerics.Matrix4x4.Identity;

			System.Numerics.Matrix4x4 _view = System.Numerics.Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			System.Numerics.Matrix4x4 _projection = Camera.I.GetProjectionMatrix();//Matrix4x4.CreateOrthographicOffCenter(-2, 2, -1, 1, 0.001f, 1000f);
																				   //_projection = System.Numerics.Matrix4x4.CreateOrthographicOffCenter(-1280, 1280, -600, 600, 0.00001f, 10000000f);

			System.Numerics.Matrix4x4 _model_view_projection = _scale * _model * _translation * _view * _projection;
			_model_view_projection = _scale * _model * _rotation * _translation * _view * _projection; // rotation experimenting
																									   //_model_view_projection = (_model * _scale) * _projection;


			shader.SetMatrix4x4("u_mvp", _model_view_projection);
			shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
			//shader.SetVector4("u_color", Color.ToVector4());

			GL.BindVertexArray(vao);
			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			GL.BindVertexArray(0);
		}
	}
}
