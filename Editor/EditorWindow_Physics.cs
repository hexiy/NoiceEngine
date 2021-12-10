using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Scripts;
using Color = Microsoft.Xna.Framework.Color;

namespace Engine
{
	public class EditorWindow_Physics : IEditorWindow
	{
		public static EditorWindow_Physics I { get; private set; }
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
		}
		public void Draw()
		{
			ImGui.SetNextWindowSize(new Vector2(300, Scene.I.Window.ClientBounds.Height), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Scene.I.Window.ClientBounds.Width - 600, 0), ImGuiCond.Always, new Vector2(1, 0));
			ImGui.Begin("Physics", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);


			bool fieldValue = Physics.Running;

			if (ImGui.Checkbox("Physics", ref fieldValue))
			{
				if (Physics.Running == false && fieldValue == true)
				{
					Physics.StartPhysics();
				}
				else if (Physics.Running == true && fieldValue == false)
				{
					Physics.StopPhysics();
				}
			}

			ImGui.End();
		}
	}
}