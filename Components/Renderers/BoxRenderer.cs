using Engine;

namespace Scripts
{
	public class BoxRenderer : Renderer
	{
		public bool fill = false;
		[System.Xml.Serialization.XmlIgnore]
		[LinkableComponent]
		public BoxShape boxShape;


		public override void Render()
		{
			if (GameObject == null || boxShape == null)
			{
				return;
			}
		}
	}
}