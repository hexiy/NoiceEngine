﻿using Engine;
using System.Collections.Generic;

namespace Scripts;

public class Component : IDestroyable
{
	private GameObject gameObject;
	[System.Xml.Serialization.XmlIgnore]
	public GameObject GameObject
	{
		get { return gameObject; }
		set
		{
			gameObject = value;
			gameObjectID = value.id;
		}
	}
	public int gameObjectID;

	[System.Xml.Serialization.XmlIgnore]
	[System.ComponentModel.DefaultValue(false)]
	public bool awoken = false;
	public bool started = false;

	public bool allowMultiple = true;
	public Component()
	{
	}
	[System.Xml.Serialization.XmlIgnore]
	public Transform transform
	{
		get { return GameObject.transform; }
		set { GameObject.transform = value; }
	}
	public bool enabled = true;
	public T GetComponent<T>(int? index = null) where T : Component
	{
		return GameObject.GetComponent<T>(index);
	}

	public bool HasComponent<T>() where T : Component
	{
		return GameObject.HasComponent<T>();
	}
	public List<T> GetComponents<T>() where T : Component
	{
		return GameObject.GetComponents<T>();
	}

	public Vector2 TransformToWorld(Vector2 localPoint)
	{
		return localPoint + transform.position;
	}

	public virtual void Awake()
	{
		awoken = true;
	}
	public virtual void Start()
	{
		started = true;
	}
	public virtual void Update()
	{
	}
	public virtual void FixedUpdate()
	{
	}
	public virtual void OnDestroyed()
	{
	}
	public virtual void PreSceneSave()
	{
	}

	public virtual void OnCollisionEnter(Rigidbody rigidbody)
	{
	}
	public virtual void OnCollisionExit(Rigidbody rigidbody)
	{
	}

	public virtual void OnTriggerEnter(Rigidbody rigidbody)
	{
	}
	public virtual void OnTriggerExit(Rigidbody rigidbody)
	{
	}

	public virtual void OnNewComponentAdded(Component comp)
	{
	}

	public int CompareTo(bool other)
	{
		if (this == null)
		{
			return 0;
		}
		else
		{
			return 1;
		}
	}

	public static implicit operator bool(Component instance)
	{
		if (instance == null)
		{
			return false;
		}
		return true;
	}
}
