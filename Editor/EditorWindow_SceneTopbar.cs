using ImGuiNET;

namespace Engine;

public class EditorWindow_SceneTopbar : IEditorWindow
{
	public static EditorWindow_SceneTopbar I { get; private set; }
	public void Init()
	{
		I = this;
	}
	public void Update()
	{
	}
	public void Draw()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

		ImGui.SetNextWindowSize(new Vector2(Camera.I.size.X, 50), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always, new Vector2(0, 0));
		ImGui.Begin("Scene", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

		ImGui.SetCursorPosX(Camera.I.size.X / 2 - 150);

		Vector4 activeColor = new Color(0.21f, 0.9f, 0.98f, 1f).ToVector4();
		Vector4 inactiveColor = new Color(1f, 1f, 1f, 1f).ToVector4();
		ImGui.PushStyleColor(ImGuiCol.Text, Physics.Running ? activeColor : inactiveColor);
		bool physicsButtonClicked = ImGui.Button("physics");
		if (physicsButtonClicked)
		{
			if (Physics.Running == false)
			{
				Physics.StartPhysics();
			}
			else if (Physics.Running == true)
			{
				Physics.StopPhysics();
			}
		}
		ImGui.SameLine();

		ImGui.PushStyleColor(ImGuiCol.Text, Global.GameRunning ? activeColor : inactiveColor);

		bool playButtonClicked = ImGui.Button("play");
		if (playButtonClicked)
		{
			Global.GameRunning = !Global.GameRunning;
		}

		ImGui.PopStyleVar();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();

		ImGui.SameLine();
		bool resetDataButtonClicked = ImGui.Button("delete data");
		if (resetDataButtonClicked)
		{
			PersistentData.DeleteAll();
		}

		ImGui.End();

	}
}
