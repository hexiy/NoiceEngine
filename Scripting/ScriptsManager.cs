/*using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scripts;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Engine
{
	public static class ScriptsManager
	{
		private static List<string> unusedScripts = new List<string>();
		public static Assembly ScriptsAssembly;
		static ScriptsManager()
		{
		}
		public static void CompileScriptsAssembly()
		{
			if (Directory.Exists("./Scripts/") == false)
			{
				Directory.CreateDirectory("./Scripts/");
			}
			var files = Directory.GetFiles("./Scripts/");

			Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", "./roslyn", EnvironmentVariableTarget.Process);
			CSharpCodeProvider provider = new CSharpCodeProvider();
			Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", null, EnvironmentVariableTarget.Process);

			//CodeDomProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters(null, "Scripts.dll");
			parameters.ReferencedAssemblies.Add("Engine.dll");
			// parameters.ReferencedAssemblies.Add("Assemblies/Microsoft.Xna.Framework.dll");
			parameters.ReferencedAssemblies.Add("MonoGame.Extended.dll");
			parameters.ReferencedAssemblies.Add("MonoGame.Framework.dll");
			parameters.ReferencedAssemblies.Add("System.Drawing.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add("System.Xml.dll");

			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;

			System.ComponentModel.TypeDescriptor.AddAttributes(typeof(UInt16), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.Vector2Editor), typeof(System.Drawing.Design.UITypeEditor)) });
			//System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Microsoft.Xna.Framework.Vector2), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.BoolEditor), typeof(System.Drawing.Design.UITypeEditor)) });
			System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Action), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.MethodEditor), typeof(System.Drawing.Design.UITypeEditor)) });
			System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Color), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.ColorPickerEditor), typeof(System.Drawing.Design.UITypeEditor)) });
			System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Texture2D), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.TextureEditor), typeof(System.Drawing.Design.UITypeEditor)) });
			System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Effect), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.EffectEditor), typeof(System.Drawing.Design.UITypeEditor)) });
			//System.ComponentModel.TypeDescriptor.AddAttributes(typeof(Enum), new Attribute[] { new System.ComponentModel.EditorAttribute(typeof(Engine.UITypeEditors.EnumEditor), typeof(System.Drawing.Design.UITypeEditor)) });




			CompilerResults results = provider.CompileAssemblyFromFile(parameters, files);

			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder();

				foreach (CompilerError error in results.Errors)
				{
					sb.AppendLine(String.Format("Line [{0}] in {1}: \n Error ({2}): {3}", error.Line, error.FileName, error.ErrorNumber, error.ErrorText));

				}
				//EditorConsole.Log(sb.ToString());
			}

			Assembly assembly = results.CompiledAssembly;

			ScriptsAssembly = assembly;
		}
	}
}*/