using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;



namespace Engine
{
	public class EditorWindow_Browser : IEditorWindow
	{
		public static EditorWindow_Browser I { get; private set; }
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
			ImGui.SetNextWindowSize(new Vector2(Window.I.ClientSize.X / 2 + 1, Window.I.ClientSize.Y - Camera.I.size.Y + 1), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(0, Window.I.ClientSize.Y), ImGuiCond.Always, new Vector2(0, 1));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Browser", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

			ResetID();
			if (Scene.I.GetSelectedGameObject() != null)
			{
				PushNextID();
				bool saveBtnPressed = ImGui.Button("Save Prefab");
				if (saveBtnPressed)
				{
					if (Directory.Exists("Prefabs") == false)
					{
						Directory.CreateDirectory("Prefabs");
					}
					Serializer.I.SaveGameObject(Scene.I.GetSelectedGameObject(), "Prefabs/" + Scene.I.GetSelectedGameObject().name + ".prefab");
				}
				PushNextID();
				bool clearBtnPressed = ImGui.Button("Clear");
				if (clearBtnPressed)
				{
					if (Directory.Exists("Prefabs"))
					{
						string[] prefabFiles = Directory.GetFiles("Prefabs");
						for (int i = 0; i < prefabFiles.Length; i++)
						{
							File.Delete(prefabFiles[i]);
						}
					}
				}
			}
			else
			{
				ImGui.Dummy(Vector2.One);
			}

			for (int i = 0; i < prefabs.Length; i++)
			{
				ImGui.SameLine();
				ImGui.BeginGroup();
				string prefabName = Path.GetFileNameWithoutExtension(prefabs[i]);
				PushNextID();
				if (ImGui.Button("", new Vector2(100, 100)))
				{
					GameObject go = Serializer.I.LoadPrefab(prefabs[i]);
					EditorWindow_Hierarchy.I.SelectGameObject(go.id);
				}
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 25);
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);

				string a = prefabName.Substring(0, Math.Clamp(prefabName.Length, 1, 12));
				ImGui.Text(a);


				ImGui.EndGroup();
			}
			// show prefabs as btns from array that updates in Update()

			ImGui.End();
		}
	}
}