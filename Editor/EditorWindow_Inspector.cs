using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Engine.Components.Renderers;
using ImGuiNET;

namespace Engine;

public class EditorWindow_Inspector : EditorWindow
{
	private string addComponentPopupText = "";

	private GameObject selectedGameObject;
	private Material selectedMaterial;
	private int windowWidth;
	private int contentMaxWidth;
	public static EditorWindow_Inspector I { get; private set; }

	public override void Init()
	{
		I = this;
	}

	public override void Update()
	{
	}

	public void OnGameObjectSelected(int id)
	{
		if (id == -1)
		{
			selectedGameObject = null;
		}
		else
		{
			selectedGameObject = Scene.I.GetGameObject(id);

			selectedMaterial = null;
		}
	}

	public void OnMaterialSelected(string materialPath)
	{
		selectedMaterial = MaterialCache.GetMaterial(Path.GetFileName(materialPath)); //MaterialAssetManager.LoadMaterial(materialPath);

		selectedGameObject = null;
	}

	public override void Draw()
	{
		if (active == false)
		{
			return;
		}

		windowWidth = 400;
		contentMaxWidth = windowWidth - (int) ImGui.GetStyle().WindowPadding.X * 1;
		ImGui.SetNextWindowSize(new Vector2(windowWidth, Editor.sceneViewSize.Y), ImGuiCond.Always);
		ImGui.SetNextWindowPos(new Vector2(Window.I.ClientSize.X, 0), ImGuiCond.Always, new Vector2(1, 0));
		//ImGui.SetNextWindowBgAlpha (0);
		ImGui.Begin("Inspector", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
		ResetID();

		if (selectedGameObject != null)
		{
			DrawGameObjectInspector();
		}

		if (selectedMaterial != null)
		{
			DrawMaterialInspector();
		}

		ImGui.End();
	}

	private void DrawMaterialInspector()
	{
		PushNextID();
		string materialName = Path.GetFileNameWithoutExtension(selectedMaterial.path);
		ImGui.Text(materialName);

		ImGui.Text("Shader");
		float itemWidth = 200;
		ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
		ImGui.SetNextItemWidth(itemWidth);

		string shaderPath = selectedMaterial.shader?.path ?? "";
		string shaderName = Path.GetFileName(shaderPath);
		bool clicked = ImGui.Button(shaderName, new Vector2(ImGui.GetContentRegionAvail().X, 20));
		if (clicked)
		{
			EditorWindow_Browser.I.GoToFile(shaderPath);
		}

		if (ImGui.BeginDragDropTarget())
		{
			ImGui.AcceptDragDropPayload("SHADER", ImGuiDragDropFlags.None);

			shaderPath = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
			if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && shaderPath.Length > 0)
			{
				//shaderPath = Path.GetRelativePath("Assets", shaderPath);
				Shader shader = new Shader(shaderPath);

				selectedMaterial.shader = shader;
				MaterialAssetManager.SaveMaterial(selectedMaterial);
			}

			ImGui.EndDragDropTarget();
		}

		if (selectedMaterial.shader != null)
		{
			ShaderUniform[] shaderUniforms = selectedMaterial.shader.GetAllUniforms();
			for (int i = 0; i < shaderUniforms.Length; i++)
			{
				PushNextID();

				ImGui.Text(shaderUniforms[i].name);

				if (shaderUniforms[i].type == typeof(Vector4))
				{
					ImGui.SameLine(ImGui.GetWindowWidth() - 200 - 5);
					ImGui.SetNextItemWidth(itemWidth);

					if (selectedMaterial.shader.uniforms.ContainsKey(shaderUniforms[i].name) == false)
					{
						continue;
					}

					object uniformValue = selectedMaterial.shader.uniforms[shaderUniforms[i].name];
					System.Numerics.Vector4 col = ((Vector4) uniformValue).ToNumerics();

					if (ImGui.ColorEdit4("", ref col))
					{
						//selectedMaterial
						int lastShader = ShaderCache.shaderInUse;
						ShaderCache.UseShader(selectedMaterial.shader);

						selectedMaterial.shader.SetColor(shaderUniforms[i].name, col);
						ShaderCache.UseShader(lastShader);
					}
				}

				if (shaderUniforms[i].type == typeof(float))
				{
					ImGui.SameLine(ImGui.GetWindowWidth() - 200 - 5);
					ImGui.SetNextItemWidth(itemWidth);

					if (selectedMaterial.shader.uniforms.ContainsKey(shaderUniforms[i].name) == false)
					{
						selectedMaterial.shader.uniforms[shaderUniforms[i].name] = Activator.CreateInstance(shaderUniforms[i].type);
					}

					object uniformValue = selectedMaterial.shader.uniforms[shaderUniforms[i].name];
					float fl = ((float) uniformValue);

					if (ImGui.InputFloat("xxx", ref fl))
					{
						//selectedMaterial
						int lastShader = ShaderCache.shaderInUse;
						ShaderCache.UseShader(selectedMaterial.shader);

						selectedMaterial.shader.SetFloat(shaderUniforms[i].name, fl);
						ShaderCache.UseShader(lastShader);
					}
				}
			}
		}
	}

	private void DrawGameObjectInspector()
	{
		if (selectedGameObject.isPrefab)
		{
			if (ImGui.Button("Update prefab"))
			{
				Serializer.I.SaveGameObject(selectedGameObject, "Assets/Prefabs/" + selectedGameObject.name + ".prefab");
			}
		}

		PushNextID();
		string gameObjectName = selectedGameObject.name;
		ImGui.Checkbox("", ref selectedGameObject.activeSelf);
		ImGui.SameLine();
		PushNextID();
		ImGui.SetNextItemWidth(contentMaxWidth);
		if (ImGui.InputText("", ref gameObjectName, 100))
		{
			selectedGameObject.name = gameObjectName;
		}

		for (int i = 0; i < selectedGameObject.components.Count; i++)
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
					FieldInfo[] _fields = selectedGameObject.components[i].GetType().GetFields();
					PropertyInfo[] properties = selectedGameObject.components[i].GetType().GetProperties();
					infos = new FieldOrPropertyInfo[_fields.Length + properties.Length];
					List<Type> inspectorSupportedTypes = new List<Type>
					                                     {
						                                     typeof(GameObject),
						                                     typeof(Material),
						                                     typeof(Vector3),
						                                     typeof(Vector2),
						                                     typeof(Texture),
						                                     typeof(Color),
						                                     typeof(bool),
						                                     typeof(float),
						                                     typeof(int),
						                                     typeof(string),
						                                     typeof(List<GameObject>),
						                                     typeof(Action),
					                                     };
					for (int fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
					{
						infos[fieldIndex] = new FieldOrPropertyInfo(_fields[fieldIndex]);
						if (_fields[fieldIndex].GetValue(selectedGameObject.components[i]) == null)
						{
							//infos[fieldIndex].canShowInEditor = false;
						}
					}

					for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
					{
						infos[_fields.Length + propertyIndex] = new FieldOrPropertyInfo(properties[propertyIndex]);
						if (properties[propertyIndex].GetValue(selectedGameObject.components[i]) == null)
						{
							infos[_fields.Length + propertyIndex].canShowInEditor = false;
						}
					}

					for (int infoIndex = 0; infoIndex < infos.Length; infoIndex++)
						if (inspectorSupportedTypes.Contains(infos[infoIndex].FieldOrPropertyType) == false)
						{
							infos[infoIndex].canShowInEditor = false;
						}
				}
				for (int infoIndex = 0; infoIndex < infos.Length; infoIndex++)
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
					else if (infos[infoIndex].FieldOrPropertyType == typeof(List<GameObject>))
					{
						float itemWidth = 200;
						ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
						ImGui.SetNextItemWidth(itemWidth);

						List<GameObject> listOfGameObjects = (List<GameObject>) infos[infoIndex].GetValue(selectedGameObject.components[i]);
						if (ImGui.CollapsingHeader(infos[infoIndex].FieldOrPropertyType.Name, ImGuiTreeNodeFlags.DefaultOpen))
						{
							for (int j = 0; j < listOfGameObjects.Count; j++)
							{
								ImGui.PushStyleColor(ImGuiCol.TextSelectedBg, Color.Aqua.ToVector4());
								PushNextID();
								bool xClicked = ImGui.Button("x", new System.Numerics.Vector2(ImGui.GetFrameHeight(), ImGui.GetFrameHeight()));

								if (xClicked)
								{
									listOfGameObjects.RemoveAt(j);
									infos[infoIndex].SetValue(selectedGameObject.components[i], listOfGameObjects);
									continue;
								}
								ImGui.SameLine();

								bool selectableClicked = ImGui.Selectable(listOfGameObjects[j].name);
								if (selectableClicked)
								{
									EditorWindow_Hierarchy.I.SelectGameObject(listOfGameObjects[j].id);
									return;
								}

								if (ImGui.BeginDragDropTarget())
								{
									ImGui.AcceptDragDropPayload("GAMEOBJECT", ImGuiDragDropFlags.None);
									string payload = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
									if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && payload.Length > 0)
									{
										GameObject foundGO = Scene.I.GetGameObject(int.Parse(payload));
										listOfGameObjects[j] = foundGO;
										infos[infoIndex].SetValue(selectedGameObject.components[i], listOfGameObjects);
									}

									ImGui.EndDragDropTarget();
								}

								ImGui.PopStyleColor();
							}
						}
					}
					else if (infos[infoIndex].FieldOrPropertyType == typeof(Action))
					{
						float itemWidth = 200;
						ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
						ImGui.SetNextItemWidth(itemWidth);

						Action action = (Action) infos[infoIndex].GetValue(selectedGameObject.components[i]);
						if (ImGui.Button(infos[infoIndex].FieldOrPropertyType.Name))
						{
							action.Invoke();
						}
					}
					else if (infos[infoIndex].FieldOrPropertyType == typeof(Texture) && selectedGameObject.components[i] is SpriteRenderer)
					{
						float itemWidth = 200;
						ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
						ImGui.SetNextItemWidth(itemWidth);

						string textureName = Path.GetFileName((selectedGameObject.components[i] as SpriteRenderer).texture.path);

						bool clicked = ImGui.Button(textureName, new Vector2(ImGui.GetContentRegionAvail().X, 20));
						//ImiGui.Text(textureName);
						if (clicked)
						{
							EditorWindow_Browser.I.GoToFile((selectedGameObject.components[i] as SpriteRenderer).texture.path);
						}

						if (ImGui.BeginDragDropTarget())
						{
							ImGui.AcceptDragDropPayload("CONTENT_BROWSER_TEXTURE", ImGuiDragDropFlags.None);
							string payload = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
							if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && payload.Length > 0)
							{
								payload = Path.GetRelativePath("Assets", payload);

								textureName = payload;

								(selectedGameObject.components[i] as SpriteRenderer).LoadTexture(textureName);
							}

							ImGui.EndDragDropTarget();
						}
					}
					else if (infos[infoIndex].FieldOrPropertyType == typeof(Material))
					{
						float itemWidth = 200;
						ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
						ImGui.SetNextItemWidth(itemWidth);

						string materialPath = Path.GetFileName((selectedGameObject.components[i] as Renderer).material.path);

						bool clicked = ImGui.Button(materialPath, new Vector2(ImGui.GetContentRegionAvail().X, 20));
						if (clicked)
						{
							EditorWindow_Browser.I.GoToFile(materialPath);
						}

						if (ImGui.BeginDragDropTarget())
						{
							ImGui.AcceptDragDropPayload("CONTENT_BROWSER_MATERIAL", ImGuiDragDropFlags.None);
							string payload = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
							if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && payload.Length > 0)
							{
								payload = payload;
								string materialName = Path.GetFileName(payload);
								Material draggedMaterial = MaterialAssetManager.LoadMaterial(payload);
								if (draggedMaterial.shader == null)
								{
									Debug.Log("No Shader attached to material.");
								}
								else
								{
									(selectedGameObject.components[i] as Renderer).material = draggedMaterial;
								}
								// load new material
							}

							ImGui.EndDragDropTarget();
						}
					}
					else if (infos[infoIndex].FieldOrPropertyType == typeof(GameObject))
					{
						float itemWidth = 200;
						ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
						ImGui.SetNextItemWidth(itemWidth);

						GameObject goObject = infos[infoIndex].GetValue(selectedGameObject.components[i]) as GameObject;
						string fieldGoName = goObject?.name ?? "";
						bool clicked = ImGui.Button(fieldGoName, new Vector2(ImGui.GetContentRegionAvail().X, 20));
						if (clicked && goObject != null)
						{
							EditorWindow_Hierarchy.I.SelectGameObject(goObject.id);
							return;
						}

						if (ImGui.BeginDragDropTarget())
						{
							ImGui.AcceptDragDropPayload("GAMEOBJECT", ImGuiDragDropFlags.None);
							string payload = Marshal.PtrToStringAnsi(ImGui.GetDragDropPayload().Data);
							if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && payload.Length > 0)
							{
								GameObject foundGO = Scene.I.GetGameObject(int.Parse(payload));
								infos[infoIndex].SetValue(selectedGameObject.components[i], foundGO);
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

						bool fieldValue = (bool) infos[infoIndex].GetValue(selectedGameObject.components[i]);

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

						float fieldValue = (float) infos[infoIndex].GetValue(selectedGameObject.components[i]);

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

						int fieldValue = (int) infos[infoIndex].GetValue(selectedGameObject.components[i]);

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

						string fieldValue = infos[infoIndex].GetValue(selectedGameObject.components[i]).ToString();

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

		bool justOpened = false;
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

			bool enterPressed = ImGui.InputText("", ref addComponentPopupText, 100, ImGuiInputTextFlags.EnterReturnsTrue);


			if (addComponentPopupText.Length > 0)
			{
				List<Type> componentTypes = typeof(Component).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract).ToList();

				for (int i = 0; i < componentTypes.Count; i++)
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
				List<Type> componentTypes = typeof(Component).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract).ToList();

				for (int i = 0; i < componentTypes.Count; i++)
					if (ImGui.Button(componentTypes[i].Name))
					{
						selectedGameObject.AddComponent(componentTypes[i]);
						ImGui.CloseCurrentPopup();
					}
			}

			ImGui.EndPopup();
		}
	}
}