namespace Scripts;

public class InspectorObjectReferencesTest : Component
{
	[Show] public float x;
	[Show] public GameObject go1;
	[Show] public GameObject go2;
	[Show] public GameObject go3;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Update()
	{
		if (go1 != null)
		{
			go1.transform.scale = Vector3.One *3* (float)Math.Sin(Time.elapsedTime);
		}
		if (go2 != null)
		{
			go2.transform.scale = Vector3.One *3* (float)Math.Sin(Time.elapsedTime+0.3f);
		}
		if (go3 != null)
		{
			go3.transform.scale = Vector3.One *3* (float)Math.Sin(Time.elapsedTime+0.7f);
		}
		base.Update();
	}
}