﻿using Microsoft.Xna.Framework;

namespace Engine
{
    public class Particle
    {
		public bool visible = false;
        public Vector2 originalPosition = new Vector2(0, 0);
        public Vector2 localPosition = new Vector2(0, 0);
        public Vector2 velocity = new Vector2(0, 0);
        public Color color = Color.White;
        public float lifetime = 0;
        public float radius = 10;
        public Particle()
        {
        }
        /*public override void Update()
        {
            float dist = Vector2.Distance(MouseInput.Position, transform.Position);
            Vector2 dirFromMouse = Vector2.Normalize(transform.Position - MouseInput.Position);
            if (dist < 30)
            {
                transform.Position = Vector2.Lerp(transform.Position, transform.Position + dirFromMouse * 30, Time.deltaTime * 10);
                particleRenderer.Color = Color.Red;
            }
            else
            {
                particleRenderer.Color = Color.White;

                transform.Position = Vector2.Lerp(transform.Position, originalPosition, Time.deltaTime * 10);
            }
            particleRenderer.Color = new Color(255f/(dist*15f),0,0,255);

            base.Update();
        }*/
    }
}
