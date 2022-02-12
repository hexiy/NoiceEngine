using System.Collections.Generic;
using ImGuiNET;

namespace Engine;

public class EditorWindow_Hierarchy : EditorWindow
{
	public static EditorWindow_Hierarchy I { get; private set; }

	private int selectedGameObjectIndex = 0;
	public Action<int> GameObjectSelected;
	bool canDelete = true;

	enum MoveDirection { up, down };
	private List<GameObject> gameObjectsChildrened = new List<GameObject>();

	public override void Init()
	{
		I = this;

	}
	public override void Update()
	{
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
		foreach (var selectedGameObject in Editor.I.GetSelectedGameObjects())
		{
			selectedGameObject.Destroy();
			selectedGameObjectIndex--;
			if (selectedGameObjectIndex < 0) return;
			GameObjectSelected.Invoke(Scene.I.gameObjects[selectedGameObjectIndex].id);
		}
	}
	private void MoveSelectedGameObject(int addToIndex = 1)
	{
		int direction = addToIndex;
		if (Editor.I.GetSelectedGameObjects().Count == 0) { return; }

		GameObject go = Editor.I.GetSelectedGameObjects()[0];
		int oldIndex = go.indexInHierarchy;

		if (oldIndex + direction >= Scene.I.gameObjects.Count || oldIndex + direction < 0)
		{
			return;
		}
		while (Scene.I.gameObjects[oldIndex + direction].transform.parent != null)
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
		selectedGameObjectIndex = Editor.I.GetGameObjectIndexInHierarchy(id);
		GameObjectSelected.Invoke(id);
	}
	public override void Draw()
	{
		if (active == false) return;

		ResetID();
		ImGui.SetNextWindowSize(new Vector2(300, Editor.sceneViewSize.Y), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X - 350, 0), ImGuiCond.Always, new Vector2(1, 0)); // +1 for double border uglyness
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
		ImGui.SameLine();
		if (ImGui.Button("Add children"))
		{
			GameObject go = GameObject.Create(name: "Children");
			go.Awake();
			go.transform.SetParent(Scene.I.gameObjects[selectedGameObjectIndex].transform);
		}
		for (int goIndex = 0; goIndex < Scene.I.gameObjects.Count; goIndex++)
		{
			if (Scene.I.gameObjects[goIndex].transform.parent != null) { continue; }
			if (Scene.I.gameObjects[goIndex].silent) { continue; }
			bool hasAnyChildren = false;//Scene.I.GetChildrenOfGameObject(Scene.I.gameObjects[goIndex]).Count != 0;
			ImGuiTreeNodeFlags flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.OpenOnArrow;
			if (hasAnyChildren == false)
			{
				flags = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
			}

			ImGui.PushStyleColor(ImGuiCol.Text, Scene.I.gameObjects[goIndex].activeInHierarchy ? Color.White.ToVector4() : new Color(1, 1, 1, 0.4f).ToVector4());
			bool opened = ImGui.TreeNodeEx($"[{Scene.I.gameObjects[goIndex].id}]" + Scene.I.gameObjects[goIndex].name, flags);
			ImGui.PopStyleColor();

			if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
			{
				SelectGameObject(Scene.I.gameObjects[goIndex].id);
			}
			if (opened)
			{
				List<Transform> children = Scene.I.gameObjects[goIndex].transform.children;

				for (int childrenIndex = 0; childrenIndex < children.Count; childrenIndex++)
				{

					//ImGui.Dummy(new Vector2(15, 10));
					//ImGui.SameLine();
					int indexInHierarchy = children[childrenIndex].gameObject.indexInHierarchy;

					bool hasAnyChildren2 = Scene.I.gameObjects[goIndex].transform.children.Count != 0;
					ImGuiTreeNodeFlags flags2 = ((selectedGameObjectIndex == indexInHierarchy) ? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.Leaf;
					if (hasAnyChildren2 == false)
					{
						flags2 = ((selectedGameObjectIndex == goIndex) ? ImGuiTreeNodeFlags.Leaf : 0) | ImGuiTreeNodeFlags.Leaf;
					}
					ImGui.PushStyleColor(ImGuiCol.Text, children[childrenIndex].gameObject.activeInHierarchy ? Color.White.ToVector4() : new Color(1, 1, 1, 0.4f).ToVector4());
					bool opened2 = ImGui.TreeNodeEx($"[{children[childrenIndex].gameObject.id}]" + children[childrenIndex].gameObject.name, flags2);
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
}
