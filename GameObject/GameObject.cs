﻿using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Engine
{
	public class GameObject
	{
		[Hide] public int indexInHierarchy = 0;

		[XmlIgnore]
		public GameObject Parent
		{
			get
			{
				int index = Scene.I.GetGameObjectIndexInHierarchy(parentID);
				if (index != -1)
				{ return Scene.I.gameObjects[index]; }
				else
				{
					return null;
				}
			}
			set
			{
				int index = Scene.I.GetGameObjectIndexInHierarchy(parentID);

				parentID = (int)value.id;
				if (index != -1)
				{
					Scene.I.gameObjects[Scene.I.GetGameObjectIndexInHierarchy(parentID)] = value;
				}
			}
		}
		public int parentID { get; set; } = -1;

		public bool updateWhenDisabled = false;
		public bool alwaysUpdate = false;
		private object ComponentsLock = new object();
		public delegate void ComponentAdded(GameObject gameObject, Component component);
		public event ComponentAdded OnComponentAdded;

		//Destroying 
		public delegate void Destroyed(GameObject gameObject);
		public event Destroyed OnDestroyed;
		public float destroyTimer = 2;
		private bool destroy = false;

		[System.ComponentModel.DefaultValue(false)]
		public bool awoken = false;
		public bool started = false;
		public int id = -1;
		public string name = "";
		public bool selected = false;
		public bool silent = false;
		public bool dynamicallyCreated = false;
		public bool active = true;

		//[System.Xml.Serialization.XmlArrayItem(type: typeof(Component))]
		[System.Xml.Serialization.XmlIgnore]
		public List<Component> components = new List<Component>();

		[System.Xml.Serialization.XmlIgnore] public Transform transform { get; set; }

		//private List<Component> ComponentsWaitingToBePaired = new List<Component>();

		public void Setup()
		{
			if (id == -1)
			{
				id = IDsManager.gameObjectNextID;
				IDsManager.gameObjectNextID++;
			}



			Scene.I.AddGameObjectToScene(this);
		}
		public static GameObject Create(Vector2? position = null, Vector2? scale = null, string name = "", bool linkComponents = true, bool _silent = false)
		{
			GameObject go = new GameObject();

			go.name = name;
			go.silent = _silent;
			go.Setup();
			if (position != null)
			{
				go.transform.position = position.Value;
			}
			if (scale != null)
			{
				go.transform.scale = scale.Value;
			}
			return go;

		}
		public GameObject()
		{
			OnDestroyed += RemoveFromLists;
			OnDestroyed += DestroyChildren;

			OnComponentAdded += LinkComponents;
			OnComponentAdded += InvokeOnComponentAddedOnComponents;
			OnComponentAdded += CheckForTransformComponent;
		}
		private void DestroyChildren(GameObject go)
		{
			for (int i = 0; i < Scene.I.gameObjects.Count; i++)
			{
				if (Scene.I.gameObjects[i].Parent == go)
				{
					Scene.I.gameObjects[i].Destroy();
				}
			}
		}
		private void CheckForTransformComponent(GameObject gameObject, Component component)
		{
			if (component is Transform)
			{
				transform = component as Transform;
			}
		}

		void InvokeOnComponentAddedOnComponents(GameObject go, Component comp)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnNewComponentAdded(comp);
			}
		}

		public void SetParent(GameObject par)
		{
			transform.rotation -= par.transform.rotation;
			transform.position = par.transform.position + (par.transform.position - transform.position);
			transform.initialAngleDifferenceFromParent = transform.rotation - par.transform.rotation;
			Parent = par;
		}
		public void LinkComponents(GameObject gameObject, Component component)
		{
			if (component.GetType() == typeof(BoxRenderer) || component.GetType() == typeof(BoxShape))
			{
				if (gameObject.name == "Button")
				{
					var a = 0;
				}
			}
			for (int compIndex1 = 0; compIndex1 < components.Count; compIndex1++)
			{
				if (components[compIndex1] == component) { continue; }

				// BoxRenderer -> Renderer -> Component
				// BoxShape    -> Shape    -> Component

				//                Renderer containts BoxShape

				// so go through component AND all of the parent classes, if we find a fitting type of comp2 type and it's linkable, assign it, and continue

				// shape might be added first, not linked to anything that was added before, so do the same but reversed- for component

				{
					Type sourceType1 = components[compIndex1].GetType();
					Type sourceType2 = component.GetType();

					FieldInfo[] infos = sourceType1.GetFields();
					for (int i = 0; i < infos.Length; i++)
					{
						var a = infos[i].GetCustomAttribute<LinkableComponent>();
						var b = infos[i].GetType();
						if (infos[i].GetCustomAttribute<LinkableComponent>() != null && infos[i].FieldType == sourceType2) // we found field that can be connected- its LinkableComponent attributed and has a type of component2
						{
							infos[i].SetValue(components[compIndex1], component);

							Type parentType = sourceType1;
							while (parentType.BaseType != null && parentType.BaseType.Name.Equals("Component") == false)// while we  arent in component, go to parent class and find all fields there
							{
								parentType = parentType.BaseType;

								FieldInfo[] parentClassInfos = parentType.GetFields();
								for (int j = 0; j < parentClassInfos.Length; j++)
								{
									if (parentClassInfos[j].GetCustomAttribute<LinkableComponent>() != null && infos[i].FieldType == sourceType2) // found linkable field in parent class
									{
										parentClassInfos[j].SetValue(components[compIndex1], component);
									}
								}
							}
						}
					}
				}

				{
					Type sourceType1 = component.GetType();
					Type sourceType2 = components[compIndex1].GetType();

					FieldInfo[] infos = sourceType1.GetFields();
					for (int i = 0; i < infos.Length; i++)
					{
						if (infos[i].GetCustomAttribute<LinkableComponent>() != null && infos[i].FieldType == sourceType2) // we found field that can be connected- its LinkableComponent attributed and has a type of component2
						{
							infos[i].SetValue(component, components[compIndex1]);

							Type parentType = sourceType2;
							while (parentType.BaseType != null && parentType.BaseType.Name.Equals("Component") == false)// while we  arent in component, go to parent class and find all fields there
							{
								parentType = parentType.BaseType;

								FieldInfo[] parentClassInfos = parentType.GetFields();
								for (int j = 0; j < parentClassInfos.Length; j++)
								{
									if (parentClassInfos[j].GetCustomAttribute<LinkableComponent>() != null && infos[i].FieldType == sourceType2) // found linkable field in parent class
									{
										parentClassInfos[j].SetValue(component, components[compIndex1]);
									}
								}
							}
						}
					}
				}

			}
		}
		/// <summary>
		/// give every found component in class its gameobject and transform reference
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="component"></param>
		public void InitializeMemberComponents(Component component)
		{

			component.transform = transform;
			component.GameObject = this;
			return;
			Type sourceType = component.GetType();

			// fields that are derived from Component
			List<FieldInfo> componentFields = new List<FieldInfo>();

			// Find all fields that derive from Component
			componentFields.AddRange(sourceType.GetFields().Where(info => info.FieldType.IsSubclassOf(typeof(Component))));

			List<FieldInfo> gameObjectFields = new List<FieldInfo>();
			List<FieldInfo> transformFields = new List<FieldInfo>();


			for (int i = 0; i < componentFields.Count; i++)
			{
				PropertyInfo gameObjectFieldInfo = componentFields[0].FieldType.GetProperty("gameObject");
				PropertyInfo transformFieldInfo = componentFields[0].FieldType.GetProperty("transform");

				gameObjectFieldInfo?.SetValue(component, this);
				transformFieldInfo?.SetValue(component, transform);
			}
		}

		public virtual void Awake()
		{

			if (transform == null && GetComponent<Transform>() == null)
			{
				transform = AddComponent<Transform>();
			}

			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].awoken == false)
				{
					components[i].Awake();
					components[i].awoken = true;
				}
			}

			awoken = true;
			Start();
		}
		public virtual void PreSceneSave()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].PreSceneSave();
			}
		}
		public virtual void Start()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].Start();
			}
			started = true;
		}
		private void RemoveFromLists(GameObject gameObject)
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null)
			{
				for (int i = 0; i < rb.touchingRigidbodies.Count; i++)
				{
					rb.touchingRigidbodies[i].touchingRigidbodies.Remove(rb);
				}
			}
			lock (ComponentsLock)
			{
				for (int i = 0; i < components.Count; i++)
				{
					components[i].OnDestroyed();
				}
				components.Clear();
			}

			Scene.I.OnGameObjectDestroyed(this);
		}
		public void Destroy(float? delay = null)
		{
			if (delay == null)
			{
				OnDestroyed?.Invoke(this);
			}
			else
			{
				destroy = true;
				destroyTimer = (float)delay;
			}
		}

		public virtual void Update()
		{
			if (active == false && updateWhenDisabled == false)
			{ return; }
			if (destroy == true)
			{
				destroyTimer -= Time.deltaTime;
				if (destroyTimer < 0)
				{
					destroy = false;
					OnDestroyed?.Invoke(this);
					return;
				}
			}

			UpdateComponents();
		}
		public virtual void FixedUpdate()
		{
			if (active == false && updateWhenDisabled == false)
			{ return; }

			FixedUpdateComponents();
		}
		public Component AddExistingComponent(Component comp)
		{
			comp.GameObject = this;
			comp.gameObjectID = id;

			components.Add(comp);

			OnComponentAdded?.Invoke(this, comp);
			if (awoken) { comp.Awake(); }

			/* for (int i = 0; i < ComponentsWaitingToBePaired.Count; i++)
			 {
				   if (ComponentsWaitingToBePaired[i].GetType() == type)
				   {
						 ComponentsWaitingToBePaired[i] = component;
						 ComponentsWaitingToBePaired.RemoveAt(i);
						 break;
				   }
			 }*/
			return comp;
		}
		public Component AddComponent<Component>() where Component : Scripts.Component, new()
		{
			Component component = new Component();

			return AddComponent(component.GetType()) as Component;
		}
		public Component AddComponent(Type type)
		{

			/* if ((transform != null || GetComponent<Transform>() != null) && type == typeof(Transform)) { 
					return null;
			  }*/
			var component = (Scripts.Component)Activator.CreateInstance(type);

			if (component.allowMultiple == false && GetComponent(type))
			{
				component = null;
				return GetComponent(type);
			}
			component.GameObject = this;

			components.Add(component);

			OnComponentAdded?.Invoke(this, component);
			if (awoken && component.awoken == false) { component.Awake(); }
			if (started && component.started == false) { component.Start(); }

			/* for (int i = 0; i < ComponentsWaitingToBePaired.Count; i++)
			 {
				   if (ComponentsWaitingToBePaired[i].GetType() == type)
				   {
						 ComponentsWaitingToBePaired[i] = component;
						 ComponentsWaitingToBePaired.RemoveAt(i);
						 break;
				   }
			 }*/
			return component;
		}
		public void RemoveComponent(int index)
		{
			active = false;
			components[index].OnDestroyed();
			components.RemoveAt(index);
			active = true;

		}
		public void RemoveComponent(Component component)
		{
			active = false;
			component.OnDestroyed();
			components.Remove(component);
			active = true;

		}
		public void RemoveComponent<T>() where T : Component
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					components[i].OnDestroyed();
					components.RemoveAt(i);
				}
			}
		}
		public void RemoveComponent(Type type)
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].GetType() == type)
				{
					components[i].OnDestroyed();
					components.RemoveAt(i);
					return;
				}
			}
		}
		public T GetComponent<T>(int? index = null) where T : Component
		{
			int k = index == null ? 0 : (int)index;
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					if (k == 0)
					{
						return components[i] as T;
					}
					else
					{
						k--;
					}
				}
			}
			return null;
		}
		public bool HasComponent<T>() where T : Component
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					return true;
				}
			}
			return false;
		}
		public List<T> GetComponents<T>() where T : Component
		{
			List<T> componentsToReturn = new List<T>();
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					componentsToReturn.Add(components[i] as T);
				}
			}
			return componentsToReturn;
		}
		public Component GetComponent(Type type)
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].GetType() == type)
				{
					return components[i];
				}
			}
			return null;
		}
		public List<Component> GetComponents(Type type)
		{
			List<Component> componentsToReturn = new List<Component>();
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].GetType() == type)
				{
					componentsToReturn.Add(components[i]);
				}
			}
			return componentsToReturn;
		}
		private void UpdateComponents()
		{
			lock (ComponentsLock)
			{
				for (int i = 0; i < components.Count; i++)
				{
					if (components[i].enabled && components[i].awoken)
					{
						components[i].Update();
					}
				}
			}
		}
		private void FixedUpdateComponents()
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].enabled && components[i].awoken)
				{
					components[i].FixedUpdate();
				}
			}
		}
		/* public void RegisterComponentPair(Component comp, Type type)
		 {
			   comp = GetComponent(type);
			   if (comp == null)
			   {
					 ComponentsWaitingToBePaired.Add(comp);
			   }
		 }*/

		public void Render()
		{
			if (active == false) { return; }
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is Renderer && components[i].enabled && components[i].awoken && active)
					(components[i] as Renderer).Render();
			}
		}

		public Vector3 TransformToWorld(Vector3 localPoint)
		{
			return localPoint + transform.position;
		}
		public Vector3 TransformToLocal(Vector3 worldPoint)
		{
			return worldPoint - transform.position;
		}

	}
}
