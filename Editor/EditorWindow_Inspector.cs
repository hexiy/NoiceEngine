using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace Engine;

public class EditorWindow_Inspector : EditorWindow
{
	private string addComponentPopupText = "";

	private GameObject selectedGameObject;
	public static EditorWindow_Inspector I { get; private set; }

	public override void Init()
	{
		I = this;
	}

	public override void Update()
	{
	}

	public void SelectGameObject(int id)
	{
		if (id == -1)
		{
			selectedGameObject = null;
		}
		else
		{
			selectedGameObject = Scene.I.GetGameObject(id);
		}
	}

	public override void Draw()
	{
		if (active == false)
		{
			return;
		}

		var windowWidth = 400;
		var contentMaxWidth = windowWidth - (int) ImGui.GetStyle().WindowPadding.X * 1;
		ImGui.SetNextWindowSize(new Vector2(windowWidth, Editor.sceneViewSize.Y), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X, 0), ImGuiCond.Always, new Vector2(1, 0));
		//ImGui.SetNextWindowBgAlpha (0);
		ImGui.Begin("Inspector", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
		ResetID();

		if (selectedGameObject != null)
		{
			if (selectedGameObject.isPrefab)
			{
				if (ImGui.Button("Update prefab"))
				{
					Serializer.I.SaveGameObject(selectedGameObject,  "Assets/Prefabs/" +selectedGameObject.name + ".prefab");
				}
			}

			PushNextID();
			var gameObjectName = selectedGameObject.name;
			ImGui.Checkbox("", ref selectedGameObject.activeSelf);
			ImGui.SameLine();
			PushNextID();
			ImGui.SetNextItemWidth(contentMaxWidth);
			if (ImGui.InputText("", ref gameObjectName, 100))
			{
				selectedGameObject.name = gameObjectName;
			}

			for (var i = 0; i < selectedGameObject.components.Count; i++)
			{
				PushNextID();

				//ImGui.SetNextItemWidth (300);
				ImGui.Checkbox("", ref selectedGameObject.components[i].enabled);
				ImGui.SameLine();

				if (ImGui.Button("-"))
				{
					selectedGameObject.RemoveComponent(selectedGameObject.components[i]);
					continue;
				}

				ImGui.SameLine();
				PushNextID();
				if (ImGui.CollapsingHeader(selectedGameObject.components[i].GetType().Name, ImGuiTreeNodeFlags.DefaultOpen))
				{
					FieldOrPropertyInfo[] infos;
					{
						var _fields = selectedGameObject.components[i].GetType().GetFields();
						var properties = selectedGameObject.components[i].GetType().GetProperties();
						infos = new FieldOrPropertyInfo[_fields.Length + properties.Length];
						var inspectorSupportedTypes = new List<Type>
						                              {
							                              typeof(Vector3),
							                              typeof(Vector2),
							                              typeof(Texture),
							                              typeof(Color),
							                              typeof(bool),
							                              typeof(float),
							                              typeof(int),
							                              typeof(string)
						                              };
						for (var fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
						{
							infos[fieldIndex] = new FieldOrPropertyInfo(_fields[fieldIndex]);
							if (_fields[fieldIndex].GetValue(selectedGameObject.components[i]) == null)
							{
								infos[fieldIndex].canShowInEditor = false;
							}
						}

						for (var propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
						{
							infos[_fields.Length + propertyIndex] = new FieldOrPropertyInfo(properties[propertyIndex]);
							if (properties[propertyIndex].GetValue(selectedGameObject.components[i]) == null)
							{
								infos[_fields.Length + propertyIndex].canShowInEditor = false;
							}
						}

						for (var infoIndex = 0; infoIndex < infos.Length; infoIndex++)
							if (inspectorSupportedTypes.Contains(infos[infoIndex].FieldOrPropertyType) == false)
							{
								infos[infoIndex].canShowInEditor = false;
							}
					}
					for (var infoIndex = 0; infoIndex < infos.Length; infoIndex++)
					{
						if (infos[infoIndex].canShowInEditor == false)
						{
							continue;
						}

						PushNextID();

						ImGui.Text(infos[infoIndex].Name);

						if (infos[infoIndex].FieldOrPropertyType == typeof(Vector3))
						{
							float itemWidth = 200;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							System.Numerics.Vector3 systemv3 = (Vector3) infos[infoIndex].GetValue(selectedGameObject.components[i]);
							if (ImGui.DragFloat3("", ref systemv3, 0.01f))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], (Vector3) systemv3);
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(Vector2))
						{
							float itemWidth = 200;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							System.Numerics.Vector2 systemv2 = (Vector2) infos[infoIndex].GetValue(selectedGameObject.components[i]);
							if (ImGui.DragFloat2("", ref systemv2, 0.01f))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], (Vector2) systemv2);
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(Texture) && selectedGameObject.components[i] is SpriteRenderer)
						{
							float itemWidth = 200;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							//Texture2D fieldValue = ((Texture2D)infos[infoIndex].GetValue(selectedGameObject.Components[i]));
							//ImGui.SameLine();
							//if (ImGui.Button("..."))
							//{
							//	OpenFileDialog ofd
							//}

							var textureName = Path.GetFileName((selectedGameObject.components[i] as SpriteRenderer).texture.path);

							var clicked = ImGui.Button(textureName, new Vector2(ImGui.GetContentRegionAvail().X, 20));
							//ImiGui.Text(textureName);
							if (clicked)
							{
								EditorWindow_Browser.I.GoToFile((selectedGameObject.components[i] as SpriteRenderer).texture.path);
							}

							if (ImGui.BeginDragDropTarget())
							{
								ImGui.AcceptDragDropPayload("CONTENT_BROWSER_TEXTURE", ImGuiDragDropFlags.None);
								var payload = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
								if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && payload.Length > 0)
								{
									payload = Path.GetRelativePath("Assets", payload);

									textureName = payload;

									(selectedGameObject.components[i] as SpriteRenderer).LoadTexture(textureName);
								}

								ImGui.EndDragDropTarget();
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(Color))
						{
							float itemWidth = 200;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth - 5);
							ImGui.SetNextItemWidth(itemWidth);

							System.Numerics.Vector4 fieldValue = ((Color) infos[infoIndex].GetValue(selectedGameObject.components[i])).ToVector4();

							if (ImGui.ColorEdit4("", ref fieldValue))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], fieldValue.ToColor());
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(bool))
						{
							ImGui.SameLine(ImGui.GetWindowWidth() - 25);

							var fieldValue = (bool) infos[infoIndex].GetValue(selectedGameObject.components[i]);

							if (ImGui.Checkbox("", ref fieldValue))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], fieldValue);
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(float))
						{
							float itemWidth = 100;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							var fieldValue = (float) infos[infoIndex].GetValue(selectedGameObject.components[i]);

							if (ImGui.DragFloat("", ref fieldValue, 0.01f, float.NegativeInfinity, float.PositiveInfinity, "%.05f"))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], fieldValue);
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(int))
						{
							float itemWidth = 100;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							var fieldValue = (int) infos[infoIndex].GetValue(selectedGameObject.components[i]);

							if (ImGui.DragInt("", ref fieldValue))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], fieldValue);
							}
						}
						else if (infos[infoIndex].FieldOrPropertyType == typeof(string))
						{
							float itemWidth = 150;
							ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
							ImGui.SetNextItemWidth(itemWidth);

							var fieldValue = infos[infoIndex].GetValue(selectedGameObject.components[i]).ToString();

							if (ImGui.InputText("", ref fieldValue, 100))
							{
								infos[infoIndex].SetValue(selectedGameObject.components[i], fieldValue);
							}
						}
						//ImGui.PopID();
					}

					//PropertyInfo[] properties = selectedGameObject.Components[i].GetType ().GetProperties ();
					//for (int j = 0; j < properties.Length; j++)
					//{
					//	PushNextID ();
					//	for (int k = 0; k < properties[j].CustomAttributes.Count (); k++)
					//	{
					//		if (properties[j].CustomAttributes.ElementAtOrDefault (k).AttributeType != typeof (ShowInEditor))
					//		{
					//			continue;
					//		}
					//		ImGui.Text (properties[j].Name);
					//		ImGui.SameLine ();
					//		//ImGui.Text (fieldInfo[j].GetValue (selectedGameObject.Components[i]).ToString ());

					//		//if (properties[j].PropertyType == typeof (float))
					//		//{
					//		//	float fl = (float) properties[j].GetValue (selectedGameObject.Components[i]);
					//		//	if (ImGui.DragFloat ("", ref fl, 0.01f, 0, 1))
					//		//	{
					//		//		properties[j].SetValue (selectedGameObject.Components[i], fl);
					//		//	}
					//		//}
					//		if (properties[j].PropertyType == typeof (Vector3))
					//		{
					//			Vector3 fl = (Vector3) properties[j].GetValue (selectedGameObject.Components[i]);
					//			if (ImGui.DragFloat3 ("", ref fl, 0.01f))
					//			{
					//				//properties[j].SetValue (selectedGameObject.Components[i], fl);
					//			}
					//		}
					//	}
					//	ImGui.PopID ();
					//}
				}
			}

			var justOpened = false;
			if (ImGui.Button("+"))
			{
				ImGui.OpenPopup("AddComponentPopup");
				justOpened = true;
			}

			if (ImGui.BeginPopupContextWindow("AddComponentPopup"))
			{
				if (justOpened)
				{
					ImGui.SetKeyboardFocusHere(0);
				}

				var enterPressed = ImGui.InputText("", ref addComponentPopupText, 100, ImGuiInputTextFlags.EnterReturnsTrue);


				if (addComponentPopupText.Length > 0)
				{
					var componentTypes = typeof(Component).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract).ToList();

					for (var i = 0; i < componentTypes.Count; i++)
						if (componentTypes[i].Name.ToLower().Contains(addComponentPopupText.ToLower()))
						{
							if (ImGui.Button(componentTypes[i].Name) || enterPressed)
							{
								selectedGameObject.AddComponent(componentTypes[i]);
								ImGui.CloseCurrentPopup();
								break;
							}
						}
				}
				else
				{
					var componentTypes = typeof(Component).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract).ToList();

					for (var i = 0; i < componentTypes.Count; i++)
						if (ImGui.Button(componentTypes[i].Name))
						{
							selectedGameObject.AddComponent(componentTypes[i]);
							ImGui.CloseCurrentPopup();
						}
				}

				ImGui.EndPopup();
			}
		}

		ImGui.End();
	}
}