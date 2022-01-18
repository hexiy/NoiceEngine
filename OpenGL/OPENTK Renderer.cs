/*using Engine;
using OpenGLMystery.GameLoop;
using OpenGLMystery.Rendering.Display;
using OpenGLMystery.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static OpenGLMystery.OpenGL.GL;

namespace Engine
{
	class Renderer : Component
	{
		private Shader shader;

		uint vao;
		uint vbo;
		public static int COUNT = 100;

		public unsafe override void Awake()
		{

			string vertexShader = @"#version 330 core
                                    layout (location = 0) in vec4 aPosition;
                                    layout (location = 1) in vec3 aColor;
                                    out vec4 vertexColor;
									
									uniform mat4 u_MVP;

                                    void main() 
                                    {
										gl_Position = u_MVP * aPosition;

                                        vertexColor = vec4(aColor.rgb, 1.0);
                                    }";

			string fragmentShader = @"#version 330 core
                                    out vec4 FragColor;
                                    in vec4 vertexColor;

                                    void main() 
                                    {
                                       FragColor = vertexColor;
                                    }";

			shader = new Shader(vertexShader, fragmentShader);
			shader.Load();

			vao = glGenVertexArray();
			vbo = glGenBuffer();

			glBindVertexArray(vao);

			glBindBuffer(GL_ARRAY_BUFFER, vbo);

			float[] vertices =
			{
				-0.5f, 0.5f,         0f, 0f, 1f, // top left
                0.5f, 0.5f,          0f, 1f, 1f,// top right
                -0.5f, -0.5f,        0f, 0f, 1f, // bottom left

                0.5f, 0.5f,          0f, 1f, 1f, // top right
                0.5f, -0.5f,         0f, 1f, 1f, // bottom right
                -0.5f, -0.5f,        0f, 0f, 1f, // bottom left
            };

			fixed (float* v = &vertices[0])
			{
				glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
			}

			// positions
			glVertexAttribPointer(0, 2, GL_FLOAT, false, 5 * sizeof(float), (void*)0);
			glEnableVertexAttribArray(0);

			// colors
			glVertexAttribPointer(1, 3, GL_FLOAT, false, 5 * sizeof(float), (void*)(2 * (sizeof(float))));
			glEnableVertexAttribArray(1);

			glBindBuffer(GL_ARRAY_BUFFER, 0);
			glBindVertexArray(0);

			base.Awake();
		}
		public override void Update()
		{
			if (GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.A) == GLFW.InputState.Press)
			{
				transform.Position = new Vector3(transform.Position.X + -GameTime.DeltaTime * 2, transform.Position.Y, transform.Position.Z);
			}
			if (GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.D) == GLFW.InputState.Press)
			{
				transform.Position = new Vector3(transform.Position.X + GameTime.DeltaTime * 2, transform.Position.Y, transform.Position.Z);
			}
			if (GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.W) == GLFW.InputState.Press)
			{
				transform.Position = new Vector3(transform.Position.X, transform.Position.Y + GameTime.DeltaTime * 2, transform.Position.Z);
			}
			if (GLFW.Glfw.GetKey(DisplayManager.Window, GLFW.Keys.S) == GLFW.InputState.Press)
			{
				transform.Position = new Vector3(transform.Position.X, transform.Position.Y + -GameTime.DeltaTime * 2, transform.Position.Z);
			}
		}
		public void Render()
		{
			//rotation += new Vector3(GameTime.DeltaTime, -GameTime.DeltaTime, 0);

			Matrix4x4 _model = Matrix4x4.Identity;
			Matrix4x4 _translation = Matrix4x4.CreateTranslation(transform.Position);

			Matrix4x4 _projection = TestGame.I.cam.GetProjectionMatrix();//Matrix4x4.CreateOrthographicOffCenter(-2, 2, -1, 1, 0.001f, 1000f);

			Matrix4x4 _view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			Matrix4x4 _model_view_projection = _model * _translation * _view * _projection;



			shader.SetMatrix4x4("u_MVP", _model_view_projection);


			shader.Use();

			glBindVertexArray(vao);
			glDrawArraysInstanced(GL_TRIANGLES, 0, 6, COUNT);
			glBindVertexArray(0);
		}
	}
}
*/