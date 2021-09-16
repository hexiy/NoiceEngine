using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Engine
{
	[XmlRootAttribute("GameObject")]
	public class GameObject
	{
		[XmlIgnore]
		public GameObject Parent
		{
			get
			{
				int index = Scene.I.GetGameObjectIndex(parentID);
				if (index != -1)
				{ return Scene.I.gameObjects[index]; }
				else
				{
					return null;
				}
			}
			set
			{
				int index = Scene.I.GetGameObjectIndex(parentID);

				parentID = (int)value.ID;
				if (index != -1)
				{
					Scene.I.gameObjects[Scene.I.GetGameObjectIndex(parentID)] = value;
				}
			}
		}
		public int parentID { get; set; } = -1;

		public bool updateWhenDisabled = false;
		private object ComponentsLock = new object();
		public delegate void ComponentAdded(GameObject gameObject, Component component);
		public event ComponentAdded OnComponentAdded;

		//Destroying 
		public delegate void Destroyed(GameObject gameObject);
		public event Destroyed OnDestroyed;
		public float destroyTimer = 2;
		private bool destroy = false;

		[System.ComponentModel.DefaultValue(false)]
		public bool Awoken { get; set; } = false;
		[ShowInEditor] public int ID { get; set; } = -1;
		[ShowInEditor] public string Name { get; set; } = "";
		public bool selected = false;
		public bool silent = false;

		public bool Active { get; set; } = true;

		//[System.Xml.Serialization.XmlArrayItem(type: typeof(Component))]
		[System.Xml.Serialization.XmlIgnore]
		public List<Component> Components = new List<Component>();

		public List<GameObject> GameObjects = new List<GameObject>();

		public bool mouseOver = false;

		public Transform transform { get; set; }

		//private List<Component> ComponentsWaitingToBePaired = new List<Component>();

		void Setup(bool linkComponents = true)
		{
			if (ID == -1)
			{
				ID = IDsManager.gameObjectNextID;
				IDsManager.gameObjectNextID++;
			}



			Scene.I.OnGameObjectCreated(this);
		}
		public static GameObject Create(Vector2? position = null, Vector2? scale = null, string name = "", bool linkComponents = true, bool _silent = false)
		{
			GameObject go = new GameObject();

			go.Name = name;
			go.silent = _silent;
			go.Setup(linkComponents);
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

			OnComponentAdded += LinkComponents;
			OnComponentAdded += InvokeOnComponentAddedOnComponents;
			OnComponentAdded += CheckForTransformComponent;
		}

		private void CheckForTransformComponent(GameObject gameObject, Component component)
		{
			if(component is Transform)
			{
				transform = component as Transform;
			}
		}

		void InvokeOnComponentAddedOnComponents(GameObject go, Component comp)
		{
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].OnNewComponentAdded();
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
			for (int index1 = 0; index1 < Components.Count; index1++)
			{
				for (int index2 = 0; index2 < Components.Count; index2++)
				{
					if (index1 == index2) { continue; }

					Type sourceType1 = Components[index1].GetType();
					Type sourceType2 = Components[index2].GetType();

					FieldInfo fieldInfo = null;
					FieldInfo[] infos = sourceType1.GetFields();
					for (int i = 0; i < infos.Length; i++)
					{
						if (infos[i].GetCustomAttribute<LinkableComponent>() != null)
						{
							fieldInfo = infos[i];
							break;
						}
					}
					while (fieldInfo == null && sourceType1.BaseType != null && sourceType1.BaseType.Name.Equals("Component") == false)
					{
						infos = sourceType1.BaseType.GetFields();
						for (int i = 0; i < infos.Length; i++)
						{
							if (infos[i].GetCustomAttribute<LinkableComponent>() != null)
							{
								fieldInfo = infos[i];
								break;
							}
						}
						sourceType1 = sourceType1.BaseType;
					}

					// get deepest class,but not Component, so if we have CircleCollider, we get Collider
					while (sourceType2.BaseType != null && sourceType2.BaseType.Name.Equals("Component") == false)
					{
						sourceType2 = sourceType2.BaseType;
					}
					if (fieldInfo != null && (fieldInfo.FieldType.IsSubclassOf(sourceType2) || fieldInfo.FieldType == sourceType2) && fieldInfo.GetValue(Components[index1]) == null)
					{
						if (GetComponents(Components[index1].GetType()).IndexOf(Components[index1]) == GetComponents(Components[index2].GetType()).IndexOf(Components[index2]))
						{
							fieldInfo.SetValue(Components[index1], Components[index2]);
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

			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i].awoken == false)
				{
					Components[i].Awake();
					Components[i].awoken = true;
				}
			}

			Awoken = true;
			Start();
		}
		public virtual void Start()
		{
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].Start();
			}
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

				Physics.rigidbodies.Remove(rb);

			}
			lock (ComponentsLock)
			{
				for (int i = 0; i < Components.Count; i++)
				{
					Components[i].OnDestroyed();
				}
				Components.Clear();
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
			if (Active == false && updateWhenDisabled == false)
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
		public Component AddExistingComponent(Component comp)
		{
			comp.GameObject = this;
			comp.gameObjectID = ID;

			Components.Add(comp);

			OnComponentAdded?.Invoke(this, comp);
			if (Awoken) { comp.Awake(); }

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

			Components.Add(component);

			OnComponentAdded?.Invoke(this, component);
			if (Awoken && component.awoken == false) { component.Awake(); }

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
			Active = false;
			Components[index].OnDestroyed();
			Components.RemoveAt(index);
			Active = true;

		}
		public void RemoveComponent<T>() where T : Component
		{
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i] is T)
				{
					Components[i].OnDestroyed();
					Components.RemoveAt(i);
				}
			}
		}
		public void RemoveComponent(Type type)
		{
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i].GetType() == type)
				{
					Components[i].OnDestroyed();
					Components.RemoveAt(i);
					return;
				}
			}
		}
		public T GetComponent<T>(int? index = null) where T : Component
		{
			int k = index == null ? 0 : (int)index;
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i] is T)
				{
					if (k == 0)
					{
						return Components[i] as T;
					}
					else
					{
						k--;
					}
				}
			}
			return null;
		}
		public List<T> GetComponents<T>() where T : Component
		{
			List<T> componentsToReturn = new List<T>();
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i] is T)
				{
					componentsToReturn.Add(Components[i] as T);
				}
			}
			return componentsToReturn;
		}
		public Component GetComponent(Type type)
		{
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i].GetType() == type)
				{
					return Components[i];
				}
			}
			return null;
		}
		public List<Component> GetComponents(Type type)
		{
			List<Component> componentsToReturn = new List<Component>();
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i].GetType() == type)
				{
					componentsToReturn.Add(Components[i]);
				}
			}
			return componentsToReturn;
		}
		private void UpdateComponents()
		{
			lock (ComponentsLock)
			{
				for (int i = 0; i < Components.Count; i++)
				{
					if (Components[i].enabled && Components[i].awoken)
					{
						Components[i].Update();
					}
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

		public void Draw(SpriteBatch batch)
		{
			if (Active == false) { return; }
			for (int i = 0; i < Components.Count; i++)
			{
				if (Components[i] is Renderer && Components[i].enabled && Components[i].awoken && Active)
					(Components[i] as Renderer).Draw(batch);
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
