
using OpenTK.Graphics.OpenGL;

namespace Engine;

public class Brush : Component
{

	public override void Update()
	{
		int spawn = 0;
		if (MouseInput.IsButtonDown(MouseInput.Buttons.Button1))
		{
			spawn = 1;
		}
		if (MouseInput.IsButtonDown(MouseInput.Buttons.Button2))
		{
			spawn = 2;
		}

		if (spawn != 0)
		{
			GameObject go = GameObject.Create();
			go.dynamicallyCreated = true;

			go.AddComponent<CircleShape>();
			go.GetComponent<CircleShape>().radius = spawn == 1 ? 10 : 30;

			go.AddComponent<BoxShape>();
			go.GetComponent<BoxShape>().size = new Vector2(2, 2) * go.GetComponent<CircleShape>().radius;


			go.AddComponent<BoxRenderer>();

			go.AddComponent<Rigidbody>();
			go.GetComponent<Rigidbody>().isStatic = spawn == 2;
			go.transform.position = MouseInput.WorldPosition;
			go.Awake();
			//go.GetComponent<SpriteRenderer>().additive=true;
			if (spawn == 1) go.GetComponent<BoxRenderer>().color = new Color(1, 1, 1, 0.9f);
			if (spawn == 2) go.GetComponent<BoxRenderer>().color = new Color(0.5f, 0.6f, 0.4f, 1f);

			go.transform.pivot = new Vector2(0.5f, 0.5f);
		}
	}
}
