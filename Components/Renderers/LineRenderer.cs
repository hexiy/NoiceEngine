using Engine;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
        public override void Draw(SpriteBatch batch)
        {
            if (GameObject == null || lineCollider == null) { return; }

            batch.DrawLine(lineCollider.GetLineStart(), lineCollider.GetLineEnd(), Color, StrokeSize);
            /*batch.DrawPoint(lineCollider.GetLineStart(), Color.Red, StrokeSize * 4);
            batch.DrawPoint(lineCollider.GetLineEnd(), Color.Red, StrokeSize * 4);*/

            base.Draw(batch);
        }
    }
}
