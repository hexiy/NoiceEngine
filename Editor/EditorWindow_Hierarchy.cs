﻿using System.Collections.Generic;
using ImGuiNET;

namespace Engine;

public class EditorWindow_Hierarchy : EditorWindow
{
	private bool canDelete = true;
	public Action<int> GameObjectSelected;
	private List<GameObject> gameObjectsChildrened = new();

	private int selectedGameObjectIndex;
	private bool showUpdatePrefabPopup;
	public static EditorWindow_Hierarchy I { get; private set; }

	public override void Init()
	{
		I = this;
	}

	private GameObject clipboardGameObject;

	public override void Update()
	{
		if (KeyboardInput.IsKeyDown(Keys.Delete) && canDelete)
		{
			canDelete = false;
			DestroySelectedGameObjects();
		}

		if (KeyboardInput.IsKeyUp(Keys.Delete))
		{
			canDelete = true;
		}

		if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyUp(Keys.C))
		{
			if (Editor.I.GetSelectedGameObject() != null)
			{
				clipboardGameObject = Editor.I.GetSelectedGameObject();
				Serializer.I.SaveClipboardGameObject(clipboardGameObject);
			}
		}

		if (KeyboardInput.IsKeyDown(Keys.LeftControl) && KeyboardInput.IsKeyUp(Keys.V))
		{
			if (clipboardGameObject != null)
			{
				GameObject loadedGO = Serializer.I.LoadClipboardGameObject();
				EditorWindow_Hierarchy.I.SelectGameObject(loadedGO.id);
			}
		}
	}

	private void DestroySelectedGameObjects()
	{
		foreach (var selectedGameObject in Editor.I.GetSelectedGameObjects())
		{
			selectedGameObject.Destroy();
			selectedGameObjectIndex--;
			if (selectedGameObjectIndex < 0)
			{
				return;
			}

			GameObjectSelected.Invoke(Scene.I.gameObjects[selectedGameObjectIndex].id);
		}
	}

	private void MoveSelectedGameObject(int addToIndex = 1)
	{
		var direction = addToIndex;
		if (Editor.I.GetSelectedGameObjects().Count == 0)
		{
			return;
		}

		var go = Editor.I.GetSelectedGameObjects()[0];
		var oldIndex = go.indexInHierarchy;

		if (oldIndex + direction >= Scene.I.gameObjects.Count || oldIndex + direction < 0)
		{
			return;
		}

		while (Scene.I.gameObjects[oldIndex + direction].transform.parent != null) direction += addToIndex;

		Scene.I.gameObjects.RemoveAt(oldIndex);
		Scene.I.gameObjects.Insert(oldIndex + direction, go);

		selectedGameObjectIndex = oldIndex + direction;
		GameObjectSelected.Invoke(Scene.I.gameObjects[oldIndex + direction].id);
	}

	public void SelectGameObject(int id)
	{
		selectedGameObjectIndex = Editor.I.GetGameObjectIndexInHierarchy(id);
		GameObjectSelected.Invoke(id);
	}

	public override void Draw()
	{
		if (active == false)
		{
			return;
		}

		ResetID();
		ImGui.SetNextWindowSize(new Vector2(300, Editor.sceneViewSize.Y), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X - 400, 0), ImGuiCond.Always, new Vector2(1, 0)); // +1 for double border uglyness
		//ImGui.SetNextWindowBgAlpha (0);
		ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
		if (ImGui.Button("+"))
		{
			var go = GameObject.Create(name: "GameObject");
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
			MoveSelectedGameObject();
		}

		ImGui.SameLine();
		if (ImGui.Button("Add children"))
		{
			var go = GameObject.Create(name: "Children");
			go.Awake();
			go.transform.SetParent(Scene.I.gameObjects[selectedGameObjectIndex].transform);
		}

		for (var goIndex = 0; goIndex < Scene.I.gameObjects.Count; goIndex++)
		{
			if (Scene.I.gameObjects[goIndex].transform.parent != null)
			{
				continue;
			}

			if (Scene.I.gameObjects[goIndex].silent)
			{
				continue;
			}

			var hasAnyChildren = false; //Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
			var flags = (selectedGameObjectIndex == goIndex ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.OpenOnArrow;
			if (hasAnyChildren == false)
			{
				flags = (selectedGameObjectIndex == goIndex ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
			}

			Vector4 nameColor = Scene.I.gameObjects[goIndex].activeInHierarchy ? Color.White.ToVector4() : new Color(1, 1, 1, 0.4f).ToVector4();

			if (Scene.I.gameObjects[goIndex].isPrefab)
			{
				nameColor = Scene.I.gameObjects[goIndex].activeInHierarchy ? Color.SkyBlue.ToVector4() : new Color(135, 206, 235, 130).ToVector4();
			}

			ImGui.PushStyleColor(ImGuiCol.Text, nameColor);
			var opened = ImGui.TreeNodeEx($"[{Scene.I.gameObjects[goIndex].id}]" + Scene.I.gameObjects[goIndex].name, flags);
			ImGui.PopStyleColor();

			if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
			{
				SelectGameObject(Scene.I.gameObjects[goIndex].id);
			}

			if (opened)
			{
				var children = Scene.I.gameObjects[goIndex].transform.children;

				for (var childrenIndex = 0; childrenIndex < children.Count; childrenIndex++)
				{
					//ImGui.Dummy(new Vector2(15, 10));
					//ImGui.SameLine();
					var indexInHierarchy = children[childrenIndex].gameObject.indexInHierarchy;

					var hasAnyChildren2 = Scene.I.gameObjects[goIndex].transform.children.Count != 0;
					var flags2 = (selectedGameObjectIndex == indexInHierarchy ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
					if (hasAnyChildren2 == false)
					{
						flags2 = (selectedGameObjectIndex == goIndex ? ImGuiTreeNodeFlags.Leaf : 0) | ImGuiTreeNodeFlags.Leaf;
					}

					ImGui.PushStyleColor(ImGuiCol.Text, children[childrenIndex].gameObject.activeInHierarchy ? Color.White.ToVector4() : new Color(1, 1, 1, 0.4f).ToVector4());
					var opened2 = ImGui.TreeNodeEx($"[{children[childrenIndex].gameObject.id}]" + children[childrenIndex].gameObject.name, flags2);
					ImGui.PopStyleColor();

					if (ImGui.IsItemClicked())
					{
						SelectGameObject(Scene.I.gameObjects[indexInHierarchy].id);
					}

					ImGui.TreePop();
				}

				ImGui.TreePop();
			}
		}

		ImGui.End();
	}

	private enum MoveDirection
	{
		up,
		down
	}
}