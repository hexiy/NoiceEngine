using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
	public class EditorWindow_Hierarchy : IEditorWindow
	{
		public static EditorWindow_Hierarchy I { get; private set; }
		private string[] gameObjectNames;
		private int selectedGameObjectIndex = 0;

		public Action<int> GameObjectSelected;
		bool canDelete = true;
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
			gameObjectNames = new string[Scene.I.gameObjects.Count];
			for (int i = 0; i < gameObjectNames.Length; i++)
			{
				gameObjectNames[i] = Scene.I.gameObjects[i].Name;
			}
			if (KeyboardInput.IsKeyDown(Keys.Delete) && canDelete == true)
			{
				canDelete = false;
				DestroySelectedGameObjects();
			}
			if (KeyboardInput.IsKeyUp(Keys.Delete))
			{
				canDelete = true;
			}
		}
		private void DestroySelectedGameObjects()
		{
			foreach (var selectedGameObject in Scene.I.GetSelectedGameObjects())
			{
				selectedGameObject.Destroy();
				selectedGameObjectIndex--;
				GameObjectSelected.Invoke(selectedGameObjectIndex);
			}
		}
		public void Draw()
		{
			ImGui.SetNextWindowSize(new Vector2(300, Scene.I.Window.ClientBounds.Height), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Scene.I.Window.ClientBounds.Width, 0), ImGuiCond.Always, new Vector2(1, 0));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
			if (ImGui.Button("+"))
			{
				GameObject go = GameObject.Create(name: "GameObject");
				go.Awake();
				go.transform.position = Camera.I.CenterOfScreenToWorld();

			}
			ImGui.SameLine();

			if (ImGui.Button("-"))
			{
				DestroySelectedGameObjects();
			}
			int oldSelectedGameObjectIndex = selectedGameObjectIndex;
			ImGui.SetNextItemWidth(300);
			ImGui.ListBox("", ref selectedGameObjectIndex, gameObjectNames, gameObjectNames.Length);
			if (oldSelectedGameObjectIndex != selectedGameObjectIndex)
			{
				GameObjectSelected.Invoke(selectedGameObjectIndex);
			}

			ImGui.End();
		}
	}
}