using System.Collections.Generic;
using System.Xml.Serialization;

namespace Scripts;

public class InspectorObjectReferencesTest : Component
{
	[Show] public List<GameObject> gos = new List<GameObject>();

	[XmlIgnore] public Action AddGameObjectToList;

	public override void Awake()
	{
		AddGameObjectToList = () =>
		{
			GameObject go = GameObject.Create(name: "testGO" + Scene.I.gameObjects.Count);
			go.Awake();
			gos.Add(go);
		};
		base.Awake();
	}

	public override void Update()
	{
		/*
		if (go1 != null)
		{
			go1.transform.scale = Vector3.One * 3 * (float) Math.Sin(Time.elapsedTime);
		}

		if (go2 != null)
		{
			go2.transform.scale = Vector3.One * 3 * (float) Math.Sin(Time.elapsedTime + 0.3f);
		}

		if (go3 != null)
		{
			go3.transform.scale = Vector3.One * 3 * (float) Math.Sin(Time.elapsedTime + 0.7f);
		}
		*/

		base.Update();
	}
}