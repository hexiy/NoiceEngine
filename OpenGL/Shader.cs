using System.Numerics;

namespace Engine;

public class Shader : IDisposable
{
	private string fragmentCode;
	private int uLocation_u_color = -1;

	private int uLocation_u_mvp = -1;
	private string vertexCode;

	public Shader()
	{
	}

	public Shader(string vertexCode, string fragmentCode)
	{
		this.vertexCode = vertexCode;
		this.fragmentCode = fragmentCode;
	}

	public int ProgramID { get; set; }

	public void Dispose()
	{
	}

	public void Load()
	{
		int vs, fs;

		vs = GL.CreateShader(ShaderType.VertexShaderArb);
		GL.ShaderSource(vs, vertexCode);
		GL.CompileShader(vs);

		var error = "";
		GL.GetShaderInfoLog(vs, out error);
		if (error.Length > 0) System.Diagnostics.Debug.WriteLine("ERROR COMPILING VERTEX SHADER " + error);

		fs = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(fs, fragmentCode);
		GL.CompileShader(fs);

		error = "";
		GL.GetShaderInfoLog(fs, out error);
		if (error.Length > 0) System.Diagnostics.Debug.WriteLine("ERROR COMPILING FRAGMENT SHADER " + error);

		ProgramID = GL.CreateProgram();
		GL.AttachShader(ProgramID, vs);
		GL.AttachShader(ProgramID, fs);

		GL.LinkProgram(ProgramID);

		// Delete shaders
		GL.DetachShader(ProgramID, vs);
		GL.DetachShader(ProgramID, fs);
		GL.DeleteShader(vs);
		GL.DeleteShader(fs);
	}

	public void SetMatrix4x4(string uniformName, Matrix4x4 mat)
	{
		if (uLocation_u_mvp == -1)
		{
			var location = GL.GetUniformLocation(ProgramID, uniformName);
			uLocation_u_mvp = location;
		}

		GL.UniformMatrix4(uLocation_u_mvp, 1, false, GetMatrix4x4Values(mat));
	}

	public void SetFloat(string uniformName, float fl)
	{
		var location = GL.GetUniformLocation(ProgramID, uniformName);
		GL.Uniform1(location, fl);
	}

	public void SetVector2(string uniformName, Vector2 vec)
	{
		var location = GL.GetUniformLocation(ProgramID, uniformName);
		GL.Uniform2(location, vec.X, vec.Y);
	}

	public void SetVector3(string uniformName, Vector3 vec)
	{
		var location = GL.GetUniformLocation(ProgramID, uniformName);
		GL.Uniform3(location, vec.X, vec.Y, vec.Z);
	}

	public void SetVector4(string uniformName, Vector4 vec)
	{
		var location = GL.GetUniformLocation(ProgramID, uniformName);
		GL.Uniform4(location, vec.X, vec.Y, vec.Z, vec.W);
	}

	public void SetColor(string uniformName, Vector4 vec)
	{
		if (uLocation_u_color == -1)
		{
			var location = GL.GetUniformLocation(ProgramID, uniformName);
			uLocation_u_color = location;
		}

		GL.Uniform4(uLocation_u_color, vec.X, vec.Y, vec.Z, vec.W);
	}

	private float[] GetMatrix4x4Values(Matrix4x4 m)
	{
		return new[]
		       {
			       m.M11, m.M12, m.M13, m.M14,
			       m.M21, m.M22, m.M23, m.M24,
			       m.M31, m.M32, m.M33, m.M34,
			       m.M41, m.M42, m.M43, m.M44
		       };
	}

	public int GetAttribLocation(string attribName)
	{
		return GL.GetAttribLocation(ProgramID, attribName);
	}
}