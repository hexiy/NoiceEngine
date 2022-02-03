using System;
using System.Collections.Generic;
using ImGuiNET;



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
			if (KeyboardInput.IsKeyDown(KeyboardInput.Keys.Delete) && canDelete == true)
			{
				canDelete = false;
				DestroySelectedGameObjects();
			}
			if (KeyboardInput.IsKeyUp(KeyboardInput.Keys.Delete))
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
				if (selectedGameObjectIndex < 0) return;
				GameObjectSelected.Invoke(Scene.I.gameObjects[selectedGameObjectIndex].id);
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
		enum MoveDirection { up, down };
		private void MoveSelectedGameObject(int addToIndex = 1)
		{
			int direction = addToIndex;
			if (Scene.I.GetSelectedGameObjects().Count == 0) { return; }

			GameObject go = Scene.I.GetSelectedGameObjects()[0];
			int oldIndex = Scene.I.GetGameObjectIndexInHierarchy(go.id);

			if (oldIndex + direction >= Scene.I.gameObjects.Count || oldIndex + direction < 0)
			{
				return;
			}
			while (Scene.I.gameObjects[oldIndex + direction].Parent != null)
			{
				direction += addToIndex;
			}

			Scene.I.gameObjects.RemoveAt(oldIndex);
			Scene.I.gameObjects.Insert(oldIndex + direction, go);

			selectedGameObjectIndex = oldIndex + direction;
			GameObjectSelected.Invoke(Scene.I.gameObjects[oldIndex + direction].id);
		}
		public void SelectGameObject(int id)
		{
			selectedGameObjectIndex = Scene.I.GetGameObjectIndexInHierarchy(id);
			GameObjectSelected.Invoke(id);
		}
		private List<GameObject> gameObjectsChildrened = new List<GameObject>();
		public void Draw()
		{
			gameObjectsChildrened = new List<GameObject>();
			ResetID();
			ImGui.SetNextWindowSize(new Vector2(300, Camera.I.size.Y), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X - 350 + 1, 0), ImGuiCond.Always, new Vector2(1, 0)); // +1 for double border uglyness
																														  //ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
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

			ImGui.SameLine();
			ImGui.Dummy(new Vector2(15, 0));
			ImGui.SameLine();
			if (ImGui.Button("^"))
			{
				MoveSelectedGameObject(-1);
			}

			ImGui.SameLine();
			if (ImGui.Button("V"))
			{
				MoveSelectedGameObject(1);
			}

			for (int goIndex = 0; goIndex < Scene.I.gameObjects.Count; goIndex++)
			{
				if (gameObjectsChildrened.Contains(Scene.I.gameObjects[goIndex]))
				{
					continue;
				}
				if (Scene.I.gameObjects[goIndex].Parent != null) { continue; }
				if (Scene.I.gameObjects[goIndex].silent) { continue; }


				bool hasAnyChildren = Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
				ImGuiTreeNodeFlags flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.OpenOnArrow;
				if (hasAnyChildren == false)
				{
					flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
				}

				ImGui.PushStyleColor(ImGuiCol.Text, Scene.I.gameObjects[goIndex].active ? Color.White.ToVector4() : new Color(1, 1, 1, 0.4f).ToVector4());
				bool opened = ImGui.TreeNodeEx(Scene.I.gameObjects[goIndex].name, flags);
				ImGui.PopStyleColor();

				if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
				{
					SelectGameObject(Scene.I.gameObjects[goIndex].id);
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
						int _i = Scene.I.GetGameObjectIndexInHierarchy(children[childrenIndex].id);

						bool hasAnyChildren2 = Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
						ImGuiTreeNodeFlags flags2 = ((selectedGameObjectIndex == _i) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
						if (hasAnyChildren2 == false)
						{
							flags2 = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Leaf : 0) | ImGuiTreeNodeFlags.Leaf;
						}

						bool opened2 = ImGui.TreeNodeEx(children[childrenIndex].name, flags2);
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