namespace Scripts;

[Serializable]
public class Material
{
	public bool additive = false;
	public Shader shader;
	
	public int vao;
	public int vbo;
	public string path;
	public void SetShader(Shader _shader)
	{
		shader = _shader;
		
		shader.Load();
		BufferCache.CreateBufferForShader(this);
	}

	
	
}