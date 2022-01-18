using Engine;
using static OpenGLMystery.OpenGL.GL;

namespace Scripts
{
	public class BoxRenderer : Renderer
	{
		public bool fill = false;
		[System.Xml.Serialization.XmlIgnore]
		[LinkableComponent]
		public BoxShape boxShape;

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

		}
		public override void Draw()
		{
			if (GameObject == null || boxShape == null)
			{
				return;
			}

			System.Numerics.Matrix4x4 _model = System.Numerics.Matrix4x4.Identity;
			System.Numerics.Matrix4x4 _translation = System.Numerics.Matrix4x4.CreateTranslation(transform.position);
			System.Numerics.Matrix4x4 _rotation = System.Numerics.Matrix4x4.CreateRotationZ(transform.rotation.Z/180 * MathHelper.Pi);
			System.Numerics.Matrix4x4 _scale = System.Numerics.Matrix4x4.CreateScale(transform.scale);

			System.Numerics.Matrix4x4 _projection = Scene.I.cam.GetProjectionMatrix();//Matrix4x4.CreateOrthographicOffCenter(-2, 2, -1, 1, 0.001f, 1000f);

			System.Numerics.Matrix4x4 _view = System.Numerics.Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			System.Numerics.Matrix4x4 _model_view_projection = _model* _translation * _scale * _view * _rotation * _projection;



			shader.SetMatrix4x4("u_MVP", _model_view_projection);


			shader.Use();

			glBindVertexArray(vao);
			glDrawArraysInstanced(GL_TRIANGLES, 0, 6, COUNT);
			glBindVertexArray(0);
			/*MonoGame.Extended.RectangleF drawRect = new MonoGame.Extended.RectangleF(Vector2.Zero, boxShape.rect.Size * transform.scale.ToVector2());

			drawRect.Offset(-boxShape.rect.Size.X * transform.anchor.X + ((boxShape.rect.Size.X * transform.anchor.X) * (1 - transform.scale.X)) + transform.position.X,
				-boxShape.rect.Size.Y * transform.anchor.Y + ((boxShape.rect.Size.Y * transform.anchor.Y) * (1 - transform.scale.Y)) + transform.position.Y);
*/
		}
	}
}