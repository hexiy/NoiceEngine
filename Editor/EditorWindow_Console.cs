using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImGuiNET;



namespace Engine
{
	public class EditorWindow_Console : IEditorWindow
	{
		public static EditorWindow_Console I { get; private set; }
		private int currentID = 0;
		public void Init()
		{
			I = this;

			//Debug.UpdateLogs();
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

			if (ImGui.Button("Clear"))
			{
				Debug.Clear();
			}

			int logsCount = Debug.GetLogs().Count;
			for (int i = 0; i < logsCount; i++)
			{
				ImGui.Text(Debug.GetLogs()[logsCount - i - 1]);
			}
			//ResetID();

			ImGui.End();
		}

		public void Update()
		{

		}
	}
}