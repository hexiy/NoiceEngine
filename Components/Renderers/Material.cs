namespace Scripts;

public class Material
{
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