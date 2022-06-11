namespace Scripts;

public class Material
{
	public bool additive = false;
	public Shader shader;
	public int vao;

	public Material()
	{
	}

	public Material(Shader shader, int vao)
	{
		this.shader = shader;
		this.vao = vao;
	}
}