using System.IO;

namespace Engine;

public static class ShaderCache
{
	public static Shader spriteRendererShader;
	public static Shader spriteSheetRendererShader;
	public static Shader boxRendererShader;
	public static Shader renderTextureShader;
	public static Shader renderTexturePostProcessShader;
	public static Shader snowShader;
	public static Shader renderTextureBloomShader;

	public static int shaderInUse = -1;

	public static void CreateShaders()
	{
		spriteRendererShader = CreateShader(Path.Combine("Shaders", "SpriteRenderer.glsl"));
		spriteSheetRendererShader = CreateShader(Path.Combine("Shaders", "SpriteSheetRenderer.glsl"));
		boxRendererShader = CreateShader(Path.Combine("Shaders", "BoxRenderer.glsl"));
		renderTextureShader = CreateShader(Path.Combine("Shaders", "RenderTexture.glsl"));
		renderTexturePostProcessShader = CreateShader(Path.Combine("Shaders", "RenderTexturePostProcess.glsl"));
		snowShader = CreateShader(Path.Combine("Shaders", "Snow.glsl"));
		renderTextureBloomShader = CreateShader(Path.Combine("Shaders", "RenderTextureBloom.glsl"));
	}

	private static Shader CreateShader(string path)
	{
		var shaderFile = File.ReadAllText(path);
		var vertexShader = GetVertexShaderFromFileString(shaderFile);
		var fragmentShader = GetFragmentShaderFromFileString(shaderFile);

		using (var shader = new Shader(vertexShader, fragmentShader))
		{
			shader.Load();

			return shader;
		}
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