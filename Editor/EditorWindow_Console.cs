using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;



namespace Engine
{
	public class EditorWindow_Console : IEditorWindow
	{
		public static EditorWindow_Console I { get; private set; }
		private int currentID = 0;
		string[] prefabs = new string[0];
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
			if (Directory.Exists("Prefabs") == false) { return; }
			prefabs = Directory.GetFiles("Prefabs");
		}

		private void ResetID()
		{
			currentID = 0;
		}
		private void PushNextID()
		{
			ImGui.PushID(currentID++);
		}
		public void Draw()
		{
			ImGui.SetNextWindowSize(new Vector2(Window.I.ClientSize.X / 2, Window.I.ClientSize.Y - Camera.I.size.Y + 1), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X, Window.I.ClientSize.Y), ImGuiCond.Always, new Vector2(1, 1));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Console", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

			ResetID();

			ImGui.End();
		}
	}
}