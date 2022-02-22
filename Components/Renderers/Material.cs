namespace Scripts;

public class Material
{
	public Shader shader;
	public int vao;
	public bool additive = false;
	public Material()
	{
		
	}
		
	public Material(Shader shader, int vao)
	{
		this.shader = shader;
		this.vao = vao;
	}
}