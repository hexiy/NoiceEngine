using Engine;


namespace Scripts
{
	public class LineRenderer : Renderer
	{
		[ShowInEditor] public float StrokeSize { get; set; } = 2;
		public bool lineStarted = false;

		[LinkableComponent]
		public LineShape lineCollider;

		public override void Awake()
		{
			lineCollider = GetComponent<LineShape>();
			base.Awake();
		}
		public override void Draw()
		{
			if (GameObject == null || lineCollider == null) { return; }

			// batch.DrawLine(lineCollider.GetLineStart(), lineCollider.GetLineEnd(), Color, StrokeSize);

			// base.Draw(batch);
		}
	}
}
