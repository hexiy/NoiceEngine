using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Engine
{
	public class Shader
	{
		string vertexCode;
		string fragmentCode;

		public int ProgramID { get; set; }

		public Shader(string vertexCode, string fragmentCode)
		{
			this.vertexCode = vertexCode;
			this.fragmentCode = fragmentCode;
		}
		public void Load()
		{
			int vs, fs;

			vs = GL.CreateShader(ShaderType.VertexShaderArb);
			GL.ShaderSource(vs, vertexCode);
			GL.CompileShader(vs);

			string error = "";
			GL.GetShaderInfoLog(vs, out error);
			if (error.Length > 0)
			{
				Debug.WriteLine("ERROR COMPILING VERTEX SHADER " + error);
			}

			fs = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fs, fragmentCode);
			GL.CompileShader(fs);

			error = "";
			GL.GetShaderInfoLog(fs, out error);
			if (error.Length > 0)
			{
				Debug.WriteLine("ERROR COMPILING FRAGMENT SHADER " + error);
			}

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
		public void Use()
		{
			GL.UseProgram(ProgramID);
		}
		public void SetMatrix4x4(string uniformName, Matrix4x4 mat)
		{
			int location = GL.GetUniformLocation(ProgramID, uniformName);
			GL.UniformMatrix4(location, 1, false, GetMatrix4x4Values(mat));
		}
		public void SetFloat(string uniformName, float fl)
		{
			int location = GL.GetUniformLocation(ProgramID, uniformName);
			GL.Uniform1(location, fl);
		}
		public void SetVector2(string uniformName, Vector2 vec)
		{
			int location = GL.GetUniformLocation(ProgramID, uniformName);
			GL.Uniform2(location, vec.X, vec.Y);
		}
		public void SetVector3(string uniformName, Vector3 vec)
		{
			int location = GL.GetUniformLocation(ProgramID, uniformName);
			GL.Uniform3(location, vec.X, vec.Y, vec.Z);
		}
		public void SetVector4(string uniformName, Vector4 vec)
		{
			int location = GL.GetUniformLocation(ProgramID, uniformName);
			GL.Uniform4(location, vec.X, vec.Y, vec.Z, vec.W);
		}
		private float[] GetMatrix4x4Values(Matrix4x4 m)
		{
			return new float[]
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
}
