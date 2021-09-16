using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scripts
{
      public class TextRenderer : Renderer
      {
            [LinkableComponent]
            public Text text;
            
            public float textSize = 12;

            public override void Draw(SpriteBatch batch)
            {
                  if (GameObject == null) { return; }
                  if (text == null) { return; }
                  
                  //drawRect.Offset(-boxCollider.rect.Size.Width * transform.anchor.X, -boxCollider.rect.Size.Height * transform.anchor.Y);
                  Vector3 stringSize = Scene.I.spriteFont.MeasureString (text.Value);
                  batch.DrawString(Scene.I.spriteFont, text.Value,
                              transform.position, Color, transform.rotation.Z, stringSize/2, transform.scale*(textSize/12), SpriteEffects.None, 0);
            }
      }
}
