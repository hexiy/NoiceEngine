using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
	public class EditorWindow_Hierarchy : IEditorWindow
	{
		public static EditorWindow_Hierarchy I { get; private set; }
		private int selectedGameObjectIndex = 0;
		private bool[] opened;
		public Action<int> GameObjectSelected;
		bool canDelete = true;
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
			opened = new bool[Scene.I.gameObjects.Count];
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
		private int currentID = 0;
		private void ResetID()
		{
			currentID = 0;
		}
		private void PushNextID()
		{
			ImGui.PushID(currentID++);
		}
		private List<GameObject> gameObjectsChildrened = new List<GameObject>();
		public void Draw()
		{
			gameObjectsChildrened = new List<GameObject>();
			ResetID();
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

			for (int goIndex = 0; goIndex < Scene.I.gameObjects.Count; goIndex++)
			{
				if (gameObjectsChildrened.Contains(Scene.I.gameObjects[goIndex]))
				{
					continue;
				}
				if (Scene.I.gameObjects[goIndex].Parent != null) { continue; }


				bool hasAnyChildren = Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
				ImGuiTreeNodeFlags flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.OpenOnArrow;
				if (hasAnyChildren == false)
				{
					flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
				}
				bool opened = ImGui.TreeNodeEx(Scene.I.gameObjects[goIndex].Name, flags);

				if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
				{
					selectedGameObjectIndex = goIndex;
					GameObjectSelected.Invoke(goIndex);
				}
				if (opened)
				{
					List<GameObject> children = Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]);

					for (int childrenIndex = 0; childrenIndex < children.Count; childrenIndex++)
					{
						if (gameObjectsChildrened.Contains(children[childrenIndex]))
						{
							children.RemoveAt(childrenIndex);
							childrenIndex--;
						}
						else
						{
							gameObjectsChildrened.Add(children[childrenIndex]);
						}
					}

					for (int childrenIndex = 0; childrenIndex < children.Count; childrenIndex++)
					{

						//ImGui.Dummy(new Vector2(15, 10));
						//ImGui.SameLine();
						int _i = Scene.I.GetGameObjectIndex(children[childrenIndex].ID);

						bool hasAnyChildren2 = Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
						ImGuiTreeNodeFlags flags2 = ((selectedGameObjectIndex == _i) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
						if (hasAnyChildren2 == false)
						{
							flags2 = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Leaf : 0) | ImGuiTreeNodeFlags.Leaf;
						}

						bool opened2 = ImGui.TreeNodeEx(children[childrenIndex].Name, flags2);
						if (ImGui.IsItemClicked())
						{
							selectedGameObjectIndex = _i;
							GameObjectSelected.Invoke(_i);
						}
						ImGui.TreePop();

					}
					ImGui.TreePop();
				}
				//ImGui.TreePop();
			}

			ImGui.End();
		}

		//public void DrawOld()
		//{
		//	ImGui.SetNextWindowSize(new Vector2(300, Scene.I.Window.ClientBounds.Height), ImGuiCond.Always);
		//	ImGui.SetNextWindowPos(new Vector2(Scene.I.Window.ClientBounds.Width, 0), ImGuiCond.Always, new Vector2(1, 0));
		//	//ImGui.SetNextWindowBgAlpha (0);
		//	ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
		//	if (ImGui.Button("+"))
		//	{
		//		GameObject go = GameObject.Create(name: "GameObject");
		//		go.Awake();
		//		go.transform.position = Camera.I.CenterOfScreenToWorld();
		//
		//	}
		//	ImGui.SameLine();
		//
		//	if (ImGui.Button("-"))
		//	{
		//		DestroySelectedGameObjects();
		//	}
		//	int oldSelectedGameObjectIndex = selectedGameObjectIndex;
		//	ImGui.SetNextItemWidth(300);
		//	ImGui.ListBox("", ref selectedGameObjectIndex, gameObjectNames, gameObjectNames.Length);
		//	if (oldSelectedGameObjectIndex != selectedGameObjectIndex)
		//	{
		//		GameObjectSelected.Invoke(selectedGameObjectIndex);
		//	}
		//
		//	ImGui.End();
		//}
	}
}