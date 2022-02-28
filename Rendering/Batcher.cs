using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Engine;

public class Batcher
{
	private int vao = -1;
	private int vbo = -1;
	private int vbo_positions = -1;
	public Material material;
	public Texture texture;
	List<float> attribs = new List<float>();
	private Dictionary<int, int> rendererLocationsInAttribs = new Dictionary<int, int>(); // key:renderer ID, value:index in attribs list

	private int size;

	public Batcher(int size, Material material, Texture texture)
	{
		this.size = size;
		this.material = material;
		this.texture = texture;
		// 24 floats per 1 object/quad/sprite
		//attribs = new float[size];
	}

	private void CreateBuffers()
	{
		vao = GL.GenVertexArray();
		vbo = GL.GenBuffer();

		BufferCache.BindVAO(vao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

		List<float> verticesList = new List<float>();

		// 100 limit for now, but it can be dynamic too
		for (int i = 0; i < size; i++)
		{
			verticesList.AddRange(new float[]
			                      {
				                      -0.5f, -0.5f, 0, 0,
				                      0.5f, -0.5f, 1, 0,
				                      -0.5f, 0.5f, 0, 1,

				                      -0.5f, 0.5f, 0, 1,
				                      0.5f, -0.5f, 1, 0,
				                      0.5f, 0.5f, 1, 1,
			                      });
		}

		float[] vertices = verticesList.ToArray();

		GL.NamedBufferStorage(
		                      vbo,
		                      sizeof(float) * vertices.Length,
		                      vertices,
		                      BufferStorageFlags.MapWriteBit);

		// ATTRIB: vertex position -   2 floats
		GL.VertexArrayAttribBinding(vao, 0, 0);
		GL.EnableVertexArrayAttrib(vao, 0);
		GL.VertexArrayAttribFormat(
		                           vao,
		                           0, // attribute index, from the shader location = 0
		                           2, // size of attribute, vec2
		                           VertexAttribType.Float, // contains floats
		                           false,
		                           0); // relative offset, first item

		// ATTRIB: texture coord -  2 floats
		GL.VertexArrayAttribBinding(vao, 1, 0);
		GL.EnableVertexArrayAttrib(vao, 1);
		GL.VertexArrayAttribFormat(
		                           vao,
		                           1, // attribute index, from the shader location = 0
		                           2, // size of attribute, vec2
		                           VertexAttribType.Float, // contains floats
		                           true,
		                           8); // relative offset, first item

		GL.VertexArrayVertexBuffer(vao, 0, vbo, new IntPtr(0), sizeof(float) * 4);
	}

	private int currentBufferUploadedSize = 0;

	public void Render()
	{
		if (material == null) return;
		if (texture == null) return;
		if (vao == -1)
		{
			CreateBuffers();
		}

		bool createdBufferThisFrame = false;
		if (vbo_positions == -1)
		{
			vbo_positions = GL.GenBuffer();
			createdBufferThisFrame = true;
		}

		BufferCache.BindVAO(vao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_positions);


		float[] attribsArray = attribs.ToArray();


		GL.NamedBufferData(
		                   vbo_positions,
		                   sizeof(float) * attribsArray.Length,
		                   attribsArray,
		                   BufferUsageHint.StreamCopy);
		currentBufferUploadedSize = attribsArray.Length;


		// ATTRIB: vertex position -   2 floats
		GL.VertexArrayAttribBinding(vao, 2, 1);
		GL.EnableVertexArrayAttrib(vao, 2);
		GL.VertexArrayAttribFormat(
		                           vao,
		                           2, // attribute index, from the shader location = 0
		                           2, // size of attribute, vec2
		                           VertexAttribType.Float, // contains floats
		                           false,
		                           0); // relative offset, first item

		// ATTRIB: size -   2 floats
		GL.VertexArrayAttribBinding(vao, 3, 1);
		GL.EnableVertexArrayAttrib(vao, 3);
		GL.VertexArrayAttribFormat(
		                           vao,
		                           3, // attribute index, from the shader location = 0
		                           2, // size of attribute, vec2
		                           VertexAttribType.Float, // contains floats
		                           false,
		                           8); // relative offset, first item


		// ATTRIB: color -   4 floats
		GL.VertexArrayAttribBinding(vao, 4, 1);
		GL.EnableVertexArrayAttrib(vao, 4);
		GL.VertexArrayAttribFormat(
		                           vao,
		                           4, // attribute index, from the shader location = 4
		                           4, // size of attribute
		                           VertexAttribType.Float, // uint representation of a color
		                           true,
		                           16); // relative offset, first item


		GL.VertexArrayVertexBuffer(vao, 1, vbo_positions, IntPtr.Zero, sizeof(float) * 8);


		ShaderCache.UseShader(material.shader);
		material.shader.SetVector2("u_resolution", texture.size);
		material.shader.SetMatrix4x4("u_mvp", Matrix4x4.Identity * Camera.I.viewMatrix * Camera.I.projectionMatrix);
		material.shader.SetColor("u_color", Color.White.ToVector4());

		BufferCache.BindVAO(vao);

		if (material.additive)
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusConstantColor);
		}
		else
		{
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		TextureCache.BindTexture(texture.id);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 6 * size);


		Debug.CountStat("Draw Calls", 1);
	}

	public void AddGameObject(int gameObjectID, int instanceIndex = 0)
	{
		int index = gameObjectID;
		if (instanceIndex != 0)
		{
			index = -gameObjectID - instanceIndex * 8;
		}

		if (rendererLocationsInAttribs.ContainsKey(index))
		{
			return;
		}

		rendererLocationsInAttribs.Add(index, attribs.Count);
		float[] _att = new float[] {0, 0, 100, 100, 1,1,1,1};

		for (int i = 0; i < 6; i++)
		{
			attribs.AddRange(_att);
		}
	}

	public void SetAttribs(int gameObjectID, float[] _attribs, int instanceIndex = 0)
	{
		int index = gameObjectID;
		if (instanceIndex != 0)
		{
			index = -gameObjectID - instanceIndex * 8;
		}

		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				this.attribs[rendererLocationsInAttribs[index] + i * 8 + j] = _attribs[j];
			}
		}
	}
}