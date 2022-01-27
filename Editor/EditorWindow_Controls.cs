using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Scripts;

namespace Engine
{
	public class EditorWindow_Controls : IEditorWindow
	{
		public static EditorWindow_Controls I { get; private set; }
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
		}
		public void Draw()
		{
			ImGui.SetNextWindowSize(new Vector2(300, Scene.I.ClientSize.Y), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Scene.I.ClientSize.X - 600, 0), ImGuiCond.Always, new Vector2(1, 0));
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
			bool playButtonClicked = ImGui.Button(Global.GameRunning ? "playing" : "stopped");
			if (playButtonClicked)
			{
				Global.GameRunning = !Global.GameRunning;
			}

			ImGui.End();
		}
	}
}