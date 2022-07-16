namespace Engine;

public static class ShaderCache
{
	public static int shaderInUse = -1;

	public static void UseShader(Shader shader)
	{
		if (shader.ProgramID == shaderInUse)
		{
			return;
		}

		shaderInUse = shader.ProgramID;
		GL.UseProgram(shader.ProgramID);
	}
}