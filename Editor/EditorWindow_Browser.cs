using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
			ImGui.SetNextWindowSize(new Vector2(Scene.I.Window.ClientBounds.Width, Scene.I.Window.ClientBounds.Height - Camera.I.Size.Y), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(0, Scene.I.Window.ClientBounds.Height), ImGuiCond.Always, new Vector2(0, 1));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Browser",/* ImGuiWindowFlags.NoTitleBar | */ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

			if (Scene.I.GetSelectedGameObject() != null)
			{
				bool saveBtnPressed = ImGui.Button("Save Prefab");
				if (saveBtnPressed)
				{
					Serializer.I.SaveGameObject(Scene.I.GetSelectedGameObject(), "Prefabs/" + Scene.I.GetSelectedGameObject().Name + ".prefab");
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
				if (ImGui.Button("", new Vector2(100, 100)))
				{
					GameObject go = Serializer.I.LoadGameObject(prefabs[i]);
					Scene.I.SelectGameObject(Scene.I.GetGameObjectIndex(go.ID));
					EditorWindow_Hierarchy.I.GameObjectSelected.Invoke(Scene.I.GetGameObjectIndex(go.ID));
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