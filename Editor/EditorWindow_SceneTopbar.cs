using ImGuiNET;

namespace Engine;

public class EditorWindow_SceneTopbar : EditorWindow
{
	public static EditorWindow_SceneTopbar I { get; private set; }

	public override void Init()
	{
		I = this;
	}

	public override void Update()
	{
	}

	public override void Draw()
	{
		if (active == false)
		{
			return;
		}

		ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

		ImGui.SetNextWindowSize(new Vector2(Camera.I.size.X, 50), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always, new Vector2(0, 0));
		ImGui.Begin("Scene", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

		ImGui.SetCursorPosX(Camera.I.size.X / 2 - 150);

		var activeColor = new Color(0.21f, 0.9f, 0.98f, 1f).ToVector4();
		var inactiveColor = new Color(1f, 1f, 1f, 1f).ToVector4();
		ImGui.PushStyleColor(ImGuiCol.Text, Physics.Running ? activeColor : inactiveColor);
		var physicsButtonClicked = ImGui.Button("physics");
		if (physicsButtonClicked)
		{
			if (Physics.Running == false)
			{
				Physics.StartPhysics();
			}
			else if (Physics.Running)
			{
				Physics.StopPhysics();
			}
		}

		ImGui.SameLine();

		ImGui.PushStyleColor(ImGuiCol.Text, Global.GameRunning ? activeColor : inactiveColor);

		var playButtonClicked = ImGui.Button("play");
		if (playButtonClicked)
		{
			Global.GameRunning = !Global.GameRunning;
		}

		ImGui.PopStyleVar();
		ImGui.PopStyleColor();
		ImGui.PopStyleColor();

		ImGui.SameLine();
		var resetDataButtonClicked = ImGui.Button("delete data");
		if (resetDataButtonClicked)
		{
			PersistentData.DeleteAll();
		}

		ImGui.End();
	}
}