using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Scripts;
using Color = Microsoft.Xna.Framework.Color;

namespace Engine
{
	public class EditorWindow_Inspector : IEditorWindow
	{
		public static EditorWindow_Inspector I { get; private set; }
		private GameObject selectedGameObject;
		public void Init()
		{
			I = this;
		}
		public void Update()
		{
		}
		public void SelectGameObject(int gameObjectIndex)
		{
			if (gameObjectIndex == -1)
			{
				selectedGameObject = null;
			}
			else
			{
				selectedGameObject = Scene.I.gameObjects[gameObjectIndex];
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
		public void Draw()
		{
			ImGui.SetNextWindowSize(new Vector2(300, Scene.I.Window.ClientBounds.Height), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(Scene.I.Window.ClientBounds.Width - 300, 0), ImGuiCond.Always, new Vector2(1, 0));
			//ImGui.SetNextWindowBgAlpha (0);
			ImGui.Begin("Inspector", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
			if (ImGui.Button("+"))
			{
				ImGui.OpenPopup("AddComponentPopup");
			}
			if (ImGui.BeginPopupContextWindow("AddComponentPopup"))
			{
				List<Type> componentTypes = typeof(Component).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract).ToList();

				for (int i = 0; i < componentTypes.Count; i++)
				{
					if (ImGui.Button(componentTypes[i].Name))
					{
						selectedGameObject.AddComponent(componentTypes[i]);
						ImGui.CloseCurrentPopup();
					}
				}
				ImGui.EndPopup();
			}
			if (selectedGameObject != null)
			{
				ImGui.SameLine();
				string gameObjectName = selectedGameObject.Name;
				ImGui.SetNextItemWidth(265);
				if (ImGui.InputText("", ref gameObjectName, 100))
				{
					selectedGameObject.Name = gameObjectName;
				}


				for (int i = 0; i < selectedGameObject.Components.Count; i++)
				{
					PushNextID();

					//ImGui.SetNextItemWidth (300);
					ImGui.Checkbox("", ref selectedGameObject.Components[i].enabled);
					ImGui.SameLine();

					if (ImGui.Button("-"))
					{
						selectedGameObject.RemoveComponent(selectedGameObject.Components[i]);
						ImGui.PopID();
						continue;
					}
					ImGui.SameLine();
					if (ImGui.CollapsingHeader(selectedGameObject.Components[i].GetType().Name + "##" + currentID))
					{
						FieldOrPropertyInfo[] infos;
						{
							FieldInfo[] _fields = selectedGameObject.Components[i].GetType().GetFields();
							PropertyInfo[] properties = selectedGameObject.Components[i].GetType().GetProperties();
							infos = new FieldOrPropertyInfo[_fields.Length + properties.Length];

							for (int fieldIndex = 0; fieldIndex < _fields.Length; fieldIndex++)
							{
								infos[fieldIndex] = new FieldOrPropertyInfo(_fields[fieldIndex]);
							}
							for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
							{
								infos[_fields.Length + propertyIndex] = new FieldOrPropertyInfo(properties[propertyIndex]);
							}
						}
						for (int infoIndex = 0; infoIndex < infos.Length; infoIndex++)
						{
							if (infos[infoIndex].canShowInEditor == false) continue;

							PushNextID();

							ImGui.Text(infos[infoIndex].Name);

							if (infos[infoIndex].FieldOrPropertyType == typeof(Vector3))
							{
								float itemWidth = 200;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
								ImGui.SetNextItemWidth(itemWidth);

								Vector3 fieldValue = (Vector3)infos[infoIndex].GetValue(selectedGameObject.Components[i]);

								if (ImGui.DragFloat3("", ref fieldValue, 0.01f))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(Vector2))
							{
								float itemWidth = 200;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
								ImGui.SetNextItemWidth(itemWidth);

								Vector2 fieldValue = (Vector2)infos[infoIndex].GetValue(selectedGameObject.Components[i]);

								if (ImGui.DragFloat2("", ref fieldValue, 0.01f))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(Texture2D))
							{
								float itemWidth = 200;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth - 5);
								ImGui.SetNextItemWidth(itemWidth);

								//Texture2D fieldValue = ((Texture2D)infos[infoIndex].GetValue(selectedGameObject.Components[i]));

								string fieldValue = (selectedGameObject.Components[i] as ITexture).texturePath;

								if (ImGui.InputText("oh", ref fieldValue, 100))
								{
									(selectedGameObject.Components[i] as ITexture).LoadTexture(fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(Color))
							{
								float itemWidth = 200;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth - 5);
								ImGui.SetNextItemWidth(itemWidth);

								Vector4 fieldValue = ((Color)infos[infoIndex].GetValue(selectedGameObject.Components[i])).ToVector4();

								if (ImGui.ColorEdit4("", ref fieldValue))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue.ToColor());
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(bool))
							{
								ImGui.SameLine(ImGui.GetWindowWidth() - 25);

								bool fieldValue = (bool)infos[infoIndex].GetValue(selectedGameObject.Components[i]);

								if (ImGui.Checkbox("", ref fieldValue))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(float))
							{
								float itemWidth = 100;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
								ImGui.SetNextItemWidth(itemWidth);

								float fieldValue = (float)infos[infoIndex].GetValue(selectedGameObject.Components[i]);

								if (ImGui.DragFloat("", ref fieldValue, 0.01f))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(int))
							{
								float itemWidth = 100;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
								ImGui.SetNextItemWidth(itemWidth);

								int fieldValue = (int)infos[infoIndex].GetValue(selectedGameObject.Components[i]);

								if (ImGui.DragInt("", ref fieldValue))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							else if (infos[infoIndex].FieldOrPropertyType == typeof(string))
							{
								float itemWidth = 150;
								ImGui.SameLine(ImGui.GetWindowWidth() - itemWidth);
								ImGui.SetNextItemWidth(itemWidth);

								string fieldValue = infos[infoIndex].GetValue(selectedGameObject.Components[i]).ToString();

								if (ImGui.InputText("", ref fieldValue, 100))
								{
									infos[infoIndex].SetValue(selectedGameObject.Components[i], fieldValue);
								}
							}
							ImGui.PopID();
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
					ImGui.PopID();
				}
			}

			ImGui.End();
			ResetID();
		}
	}
}