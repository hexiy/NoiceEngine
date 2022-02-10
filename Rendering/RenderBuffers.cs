
namespace Engine;

public static class RenderBuffers
{
	public static int spriteRendererVAO;
	public static int spriteRendererVBO;

	public static int boxRendererVAO;
	public static int boxRendererVBO;

	public static void CreateBuffers()
	{
		CreateSpriteRendererBuffers(ref spriteRendererVAO, ref spriteRendererVBO);
		CreateBoxRendererBuffers(ref boxRendererVAO, ref boxRendererVBO);
	}


	private static void CreateSpriteRendererBuffers(ref int vao, ref int vbo)
	{
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

	private static void CreateBoxRendererBuffers(ref int vao, ref int vbo)
	{
		vao = GL.GenVertexArray();
		vbo = GL.GenBuffer();

		GL.BindVertexArray(vao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

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
}
