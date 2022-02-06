using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImGuiNET;



namespace Engine
{
	public class EditorWindow_Floating : IEditorWindow
	{
		public static EditorWindow_Floating I { get; private set; }
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
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Floating", ImGuiWindowFlags.NoCollapse);

			ImGui.Image((IntPtr)Window.I.sceneRenderTexture.colorAttachment, new Vector2(300, 300));

			ImGui.End();
		}

		public void Update()
		{

		}
	}
}