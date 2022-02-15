using ImGuiNET;

namespace Engine;

public class EditorWindow
{

	private int currentID = 0;
	internal void ResetID()
	{
		currentID = 0;
	}
	internal void PushNextID()
	{
		ImGui.PushID(currentID++);
	}

	internal bool active = true;
	public virtual void Init() { }
	public virtual void Update() { }
	public virtual void Draw() { }
}
