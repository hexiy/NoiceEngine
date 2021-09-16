using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Scripts
{
    public class PolygonRenderer : Renderer
    {
        //[LinkableComponent]
        public PolygonShape polygonCollider;
        public bool editingPoints = false;

        public override void Awake()
        {
            polygonCollider = GetComponent<PolygonShape>();
            base.Awake();
        }
        public override void Draw(SpriteBatch batch)
        {
            if (GameObject == null || polygonCollider == null) { return; }

            for (int i = 0; i < polygonCollider.Points.Count; i++)
            {
                Vector2 point1 = polygonCollider.Points[i];
                Vector2 point2 = i + 1 >= polygonCollider.Points.Count ? polygonCollider.Points[0] : polygonCollider.Points[i + 1];
                batch.DrawLine(TransformToWorld(point1), TransformToWorld(point2), Color, 3);
            }
            batch.DrawPoint(polygonCollider.Center, Color, 4);

            base.Draw(batch);
        }
    }
}
