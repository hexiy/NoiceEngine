using System.IO;
using ImGuiNET;

namespace Engine;

public class BrowserContextItem
{
	private string itemName;
	private string defaultFileName;
	private string fileExtension;

	private Action<string> confirmAction;
	public bool showPopup = false;

	public BrowserContextItem(string itemName, string defaultFileName, string fileExtension, Action<string> confirmAction)
	{
		this.itemName = itemName;
		this.defaultFileName = defaultFileName;
		this.fileExtension = fileExtension;
		this.confirmAction = confirmAction;
	}

	public void ShowContextItem()
	{
		if (ImGui.Button(itemName))
		{
			showPopup = true;
			ImGui.CloseCurrentPopup();
		}
	}

	public void ShowPopupIfOpen()
	{
		if (showPopup)
		{
			ImGui.OpenPopup(itemName);

			if (ImGui.BeginPopupContextWindow(itemName))
			{
				ImGui.InputText("", ref defaultFileName, 100);
				if (ImGui.Button("Save"))
				{
					string filePath = Path.Combine(EditorWindow_Browser.I.currentDirectory.FullName, defaultFileName + fileExtension);
					confirmAction.Invoke(filePath);

					showPopup = false;
					ImGui.CloseCurrentPopup();
				}

				ImGui.SameLine();

				if (ImGui.Button("Cancel") || ImGui.IsKeyPressed((int) Keys.Escape))
				{
					showPopup = false;
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
			else
			{
				showPopup = false;
			}
		}
	}
}