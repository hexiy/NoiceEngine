using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImGuiNET;



namespace Engine
{
	public class EditorWindow_Browser : IEditorWindow
	{
		public static EditorWindow_Browser I { get; private set; }
		private int currentID = 0;
		string[] assets = new string[0];

		DirectoryInfo currentDirectory;

		private bool showCreateScenePopup = false;
		private string createScenePopupSceneName = "scene1";


		static float padding = 16.0f;
		static float thumbnailSize = 128.0f;


		public void Init()
		{
			I = this;

			currentDirectory = new DirectoryInfo("Assets");
		}
		public void Update()
		{
			if ((int)Time.elapsedSeconds % 2 == 0) { return; }

			RefreshAssets();
		}
		void RefreshAssets()
		{
			if (Directory.Exists(currentDirectory.FullName) == false) { return; }
			assets = Directory.GetDirectories(currentDirectory.FullName);
			assets = assets.Concat(Directory.GetFiles(currentDirectory.FullName, "", SearchOption.TopDirectoryOnly)).ToArray();
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
			ImGui.SetNextWindowSize(new Vector2(Window.I.ClientSize.X / 2 + 1, Window.I.ClientSize.Y - Camera.I.size.Y + 1), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(0, Window.I.ClientSize.Y), ImGuiCond.Always, new Vector2(0, 1));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Browser", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
			ResetID();

			if (ImGui.Button("<"))
			{
				currentDirectory = currentDirectory.Parent;
				RefreshAssets();
			}

			//ResetID();
			//if (Scene.I.GetSelectedGameObject() != null)
			//{
			//	PushNextID();
			//	bool saveBtnPressed = ImGui.Button("Save Prefab");
			//	if (saveBtnPressed)
			//	{
			//		if (Directory.Exists("Prefabs") == false)
			//		{
			//			Directory.CreateDirectory("Prefabs");
			//		}
			//		Serializer.I.SaveGameObject(Scene.I.GetSelectedGameObject(), "Prefabs/" + Scene.I.GetSelectedGameObject().name + ".prefab");
			//	}
			//}

			//for (int i = 0; i < assets.Length; i++)
			//{
			//	if (i > 0)
			//	{
			//		ImGui.SameLine();
			//	}
			//	ImGui.BeginGroup();
			//	string directoryName = new DirectoryInfo(directories[i]).Name;
			//	PushNextID();
			//
			//
			//	ImGui.PushStyleColor(ImGuiCol.Button, new Color(13, 27, 30).ToVector4());
			//	bool directoryClicked = ImGui.Button("FOLDER", new Vector2(100, 100));
			//	ImGui.PopStyleColor();
			//	if (directoryClicked)
			//	{
			//		currentDirectory = new DirectoryInfo(directories[i]);
			//		RefreshAssets();
			//		return;
			//	}
			//
			//	ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 25);
			//	ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
			//
			//	string a = directoryName.Substring(0, Math.Clamp(directoryName.Length, 1, 12));
			//	ImGui.Text(a);
			//
			//
			//	ImGui.EndGroup();
			//}
			for (int i = 0; i < assets.Length; i++)
			{
				if (i != 0)
				{
					ImGui.SameLine();
				}

				DirectoryInfo directoryInfo = new DirectoryInfo(assets[i]);
				bool isDirectory = directoryInfo.Exists;

				ImGui.BeginGroup();
				string assetName = Path.GetFileNameWithoutExtension(assets[i]);
				string assetExtension = Path.GetExtension(assets[i]).ToLower();
				PushNextID();

				if (isDirectory)
				{
					ImGui.PushStyleColor(ImGuiCol.Button, new Color(13, 27, 30).ToVector4());
					ImGui.Button("FOLDER", new Vector2(100, 100));
					ImGui.PopStyleColor();
				}
				else
				{
					ImGui.Button(assetExtension.Substring(1).ToUpper(), new Vector2(100, 100));
				}

				if (ImGui.BeginDragDropSource())
				{
					string itemPath = assets[i];
					IntPtr stringPointer = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(itemPath);

					ImGui.SetDragDropPayload("CONTENT_BROWSER_ITEM", stringPointer, (uint)(sizeof(char) * itemPath.Length));
					ImGui.Text(Path.GetFileNameWithoutExtension(itemPath));

					System.Runtime.InteropServices.Marshal.FreeHGlobal(stringPointer);

					ImGui.EndDragDropSource();

				}
				if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
				{
					if (isDirectory)
					{
						currentDirectory = directoryInfo;
						RefreshAssets();
						return;
					}
					if (assetExtension == ".prefab")
					{
						GameObject go = Serializer.I.LoadPrefab(assets[i]);
						EditorWindow_Hierarchy.I.SelectGameObject(go.id);
					}
					if (assetExtension == ".scene")
					{
						Scene.I.LoadScene(assets[i]);
					}
				}

				ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 25);
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);

				string a = assetName.Substring(0, Math.Clamp(assetName.Length, 1, 12));
				ImGui.Text(a);


				ImGui.EndGroup();
			}




			// show prefabs as btns from array that updates in Update()
			if (ImGui.BeginPopupContextWindow("BrowserPopup"))
			{
				if (ImGui.Button("New Scene"))
				{
					showCreateScenePopup = true;
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}

			if (showCreateScenePopup)
			{
				ImGui.OpenPopup("AddComponentPopup");

				if (ImGui.BeginPopupContextWindow("AddComponentPopup"))
				{
					ImGui.InputText("", ref createScenePopupSceneName, 100);
					if (ImGui.Button("Save"))
					{
						Scene.I.SaveScene(Path.Combine(currentDirectory.FullName, createScenePopupSceneName + ".scene"));

						showCreateScenePopup = false;
						ImGui.CloseCurrentPopup();
					}
					ImGui.SameLine();

					if (ImGui.Button("Cancel") || ImGui.IsKeyPressed((int)KeyboardInput.Keys.Escape))
					{
						showCreateScenePopup = false;
						ImGui.CloseCurrentPopup();
					}
					ImGui.EndPopup();
				}
				else
				{
					showCreateScenePopup = false;

				}
			}

			ImGui.End();
		}
	}
}