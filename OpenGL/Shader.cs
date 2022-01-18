using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static OpenGLMystery.OpenGL.GL;

namespace Engine
{
	class Shader
	{
		string vertexCode;
		string fragmentCode;

		public uint ProgramID { get; set; }

		public Shader(string vertexCode, string fragmentCode)
		{
			this.vertexCode = vertexCode;
			this.fragmentCode = fragmentCode;
		}
		public void Load()
		{
			uint vs, fs;

			vs = glCreateShader(GL_VERTEX_SHADER);
			glShaderSource(vs, vertexCode);
			glCompileShader(vs);

			int[] status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);
			if (status[0] == 0)
			{
				string error = glGetShaderInfoLog(vs);
				Debug.WriteLine("ERROR COMPILING VERTEX SHADER " + error);
			}

			fs = glCreateShader(GL_FRAGMENT_SHADER);
			glShaderSource(fs, fragmentCode);
			glCompileShader(fs);


			status = glGetShaderiv(fs, GL_COMPILE_STATUS, 1);
			if (status[0] == 0)
			{
				string error = glGetShaderInfoLog(fs);
				Debug.WriteLine("ERROR COMPILING FRAGMENT SHADER " + error);
			}

			ProgramID = glCreateProgram();
			glAttachShader(ProgramID, vs);
			glAttachShader(ProgramID, fs);

			glLinkProgram(ProgramID);

			// Delete shaders
			glDetachShader(ProgramID, vs);
			glDetachShader(ProgramID, fs);
			glDeleteShader(vs);
			glDeleteShader(fs);


		}
		public void Use()
		{
			glUseProgram(ProgramID);
		}
		public void SetMatrix4x4(string uniformName, Matrix4x4 mat)
		{
			int location = glGetUniformLocation(ProgramID, uniformName);
			glUniformMatrix4fv(location, 1, false, GetMatrix4x4Values(mat));
		}
		public void SetFloat(string uniformName, float fl)
		{
			int location = glGetUniformLocation(ProgramID, uniformName);
			glUniform1f(location, fl);
		}
		public void SetVector2(string uniformName, Vector2 vec)
		{
			int location = glGetUniformLocation(ProgramID, uniformName);
			glUniform2f(location, vec.X, vec.Y);
		}
		public void SetVector3(string uniformName, Vector3 vec)
		{
			int location = glGetUniformLocation(ProgramID, uniformName);
			glUniform3f(location, vec.X, vec.Y,vec.Z);
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
	}
}
