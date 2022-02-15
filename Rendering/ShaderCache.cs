using System.IO;

namespace Engine;

public static class ShaderCache
{
	public static Shader spriteRendererShader;
	public static Shader spriteSheetRendererShader;
	public static Shader boxRendererShader;
	public static Shader renderTextureShader;
	public static Shader renderTexturePostProcessShader;
	public static Shader renderTextureBloomShader;

	public static int shaderInUse = -1;


	public static void CreateShaders()
	{
		CreateSpriteRendererShader();
		CreateSpriteSheetRendererShader();
		CreateBoxRendererShader();
		CreateRenderTextureShader();
		CreateRenderTexturePostProcessShader();
		CreateRenderTextureBloomShader();
	}

	private static void CreateRenderTextureShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "RenderTexture.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);


		renderTextureShader = new Shader(vertexShader, fragmentShader);

		renderTextureShader.Load();
	}

	private static void CreateRenderTextureBloomShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "RenderTextureBloom.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);

		renderTextureBloomShader = new Shader(vertexShader, fragmentShader);

		renderTextureBloomShader.Load();
	}

	private static void CreateRenderTexturePostProcessShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "RenderTexturePostProcess.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);


		renderTexturePostProcessShader = new Shader(vertexShader, fragmentShader);

		renderTexturePostProcessShader.Load();
	}

	private static void CreateSpriteRendererShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "SpriteRenderer.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);

		spriteRendererShader = new Shader(vertexShader, fragmentShader);

		spriteRendererShader.Load();
	}

	private static void CreateSpriteSheetRendererShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "SpriteSheetRenderer.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);

		spriteSheetRendererShader = new Shader(vertexShader, fragmentShader);

		spriteSheetRendererShader.Load();
	}

	private static void CreateBoxRendererShader()
	{
		string shaderFile = File.ReadAllText(Path.Combine("Shaders", "BoxRenderer.glsl"));
		string vertexShader = GetVertexShaderFromFileString(shaderFile);
		string fragmentShader = GetFragmentShaderFromFileString(shaderFile);

		boxRendererShader = new Shader(vertexShader, fragmentShader);
		boxRendererShader.Load();
	}

	public static void UseShader(Shader shader)
	{
		if (shader.ProgramID == shaderInUse)
		{
			return;
		}

		shaderInUse = shader.ProgramID;
		GL.UseProgram(shader.ProgramID);
	}

	private static string GetVertexShaderFromFileString(string shaderFile)
	{
		return shaderFile.Substring(shaderFile.IndexOf("[VERTEX]") + 8,
			shaderFile.IndexOf("[FRAGMENT]") - shaderFile.IndexOf("[VERTEX]") - 8); //File.ReadA;
	}

	private static string GetFragmentShaderFromFileString(string shaderFile)
	{
		return shaderFile.Substring(shaderFile.IndexOf("[FRAGMENT]") + 10); //File.ReadA;
	}
}