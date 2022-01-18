using Engine;


namespace Scripts
{
	public class BoxShape : Shape
	{
		[ShowInEditor]
		public Vector2 size;

		[ShowInEditor]
		public bool automaticSize;


		public override void Update()
		{
			base.Update();
		}
	}
}
