using System.Collections.Generic;

namespace Scripts;

public class PolygonShape : Shape
{
	[System.Xml.Serialization.XmlIgnore]
	public Action onPointsEdit;// = Engine.ColliderEditor.GetInstance().ToggleEditing;

	/// <summary>
	/// LOCAL points
	/// </summary>

	[Show]
	public List<Vector2> Points { get; } = new List<Vector2>();
	public List<Vector2> OriginalPoints { get; } = new List<Vector2>();

	public List<Vector2> Edges { get; } = new List<Vector2>() { new Vector2(0, 0) };
	public Vector2 Position = new Vector2(0, 0);

	public int highlightEdgeIndex = 0;
	public override void Awake()
	{
		BuildEdges();
		base.Awake();
	}
	public void BuildEdges()
	{
		if (OriginalPoints.Count == 0 || Points.Count != OriginalPoints.Count)
		{
			OriginalPoints.Clear();
			OriginalPoints.AddRange(Points.ToArray());
		}
		Vector2 p1;
		Vector2 p2;
		Edges.Clear();
		for (int i = 0; i < Points.Count; i++)
		{
			p1 = Points[i];
			if (i + 1 >= Points.Count)
			{
				p2 = Points[0];
			}
			else
			{
				p2 = Points[i + 1];
			}
			Edges.Add(p2 - p1);
		}
	}

	/// <summary>
	/// Returns center in WORLD
	/// </summary>
	public Vector2 Center
	{
		get
		{
			float totalX = 0;
			float totalY = 0;
			for (int i = 0; i < Points.Count; i++)
			{
				totalX += Points[i].X;
				totalY += Points[i].Y;
			}

			return this.TransformToWorld(new Vector2(totalX / (float)Points.Count, totalY / (float)Points.Count));
		}
	}
	float lastRotation = 0;
	public override void Update()
	{
		if (transform.rotation.Z != lastRotation)
		{
			SetRotation(transform.rotation.Z);
			lastRotation = transform.rotation.Z;

		}
		base.Update();
	}
	public void SetRotation(float angle)
	{
		//if (angle > 0.01 || float.IsNaN(angle)) { return; }
		for (int i = 0; i < Points.Count; i++)
		{

			Vector2 originalPoint = OriginalPoints[i];

			Points[i] = new Vector2(originalPoint.X * (float)Math.Cos(-angle) - originalPoint.Y * (float)Math.Sin(-angle), originalPoint.X * (float)Math.Sin(-angle) + originalPoint.Y * (float)Math.Cos(-angle));
		}
		BuildEdges();
	}
}

