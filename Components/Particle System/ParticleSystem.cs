﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Scripts
{
	public class ParticleSystem : Component
	{
		public new bool allowMultiple = false;

		public object listLock = new object();

		Pool<Particle> pool = new Pool<Particle>(() => new Particle());

		[XmlIgnore] public List<Particle> particles = new List<Particle>(3000);
		private ParticleSystemRenderer renderer;
		[ShowInEditor] public Vector2 StartVelocity { get; set; } = new Vector2(0, 0);
		[ShowInEditor] public float radius { get; set; } = 100;
		[ShowInEditor] public float speed { get; set; } = 2;
		[ShowInEditor] public float StartSize { get; set; } = 20;
		[ShowInEditor] public float EndSize { get; set; } = 0;
		[ShowInEditor] public Color StartColor1 { get; set; } = Color.White;
		[ShowInEditor] public Color StartColor2 { get; set; } = Color.Gray;
		[ShowInEditor] public int MaxParticles { get; set; } = 3000;
		[ShowInEditor] public float MaxLifetime { get; set; } = 1;
		[ShowInEditor] public float StartVelocityVariation { get; set; } = 70;
		[ShowInEditor] public float SpawnRate { get; set; } = 0.5f; // spawn every half second
		[ShowInEditor] public Vector2 SpawnBoundsSize { get; set; } = new Vector2(5, 5); // spawn every half second

		private Random rnd = new Random();
		float time = 0;
		public Particle latestParticle;
		public override void Awake()
		{
			renderer = GameObject.GetComponent<ParticleSystemRenderer>();
			if (renderer == null)
			{
				renderer = GameObject.AddComponent<ParticleSystemRenderer>();
			}
			renderer.particleSystem = this;
			// BuildShape();

			base.Awake();
		}
		public override void Update()
		{
			time += Time.deltaTime;

			while (time - SpawnRate >= 0 && particles.Count < MaxParticles)
			{
				SpawnParticle();
				time -= SpawnRate;
			}
			Parallel.For(0, particles.Count, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 20 }, (i) =>
			{
				lock (listLock)
				{
					if (particles.Count > i && particles[i] != null)
					{
						//particles[i].velocity -= Physics.gravity * Time.deltaTime;

						particles[i].worldPosition += particles[i].velocity * Time.deltaTime;

						particles[i].lifetime += Time.deltaTime;

						//particles[i].color = new Color((int)particles[i].color.R, particles[i].color.G, particles[i].color.B, (int)((0.1f / particles[i].lifetime) * 255));
						/*particles[i].color = new Color((int)((0.1f / particles[i].lifetime) * 255),
							20, 20, (int)((0.1f / particles[i].lifetime) * 255));*/

						//if (particles[i].lifetime < 0.2f)
						//{
						//	particles[i].color = new Color(particles[i].color.R, particles[i].color.G, particles[i].color.B,
						//		((int)(particles[i].lifetime / 0.2f * 255)));
						//}
						//else
						//{
						//	particles[i].color = new Color(particles[i].color.R, particles[i].color.G, particles[i].color.B,
						//			((int)((1 - (particles[i].lifetime / MaxLifetime)) * 255)));
						//}


						particles[i].radius = MathHelper.Lerp(StartSize, EndSize, particles[i].lifetime / MaxLifetime);

						if (particles[i].lifetime > MaxLifetime)
						{
							particles[i].visible = false;
							pool.PutObject(particles[i]);
							particles.RemoveAt(i);
						}
					}
				}
			});

			base.Update();
		}
		void SpawnParticle()
		{
			Particle p = pool.GetObject();
			latestParticle = p;
			p.visible = true;
			p.lifetime = 0;
			p.radius = StartSize;
			p.worldPosition = transform.position;
			p.worldPosition += new Vector2(Rendom.Range(-SpawnBoundsSize.X, SpawnBoundsSize.X), Rendom.Range(-SpawnBoundsSize.Y, SpawnBoundsSize.Y));

			p.velocity = StartVelocity + new Vector2(rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation), rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation));

			p.color = Rendom.ColorRange(StartColor1, StartColor2);
			lock (listLock)
			{
				particles.Add(p);

				if (particles.Count > MaxParticles)
				{
					int num = particles.Count - MaxParticles;
					for (int i = 0; i < num; i++)
					{

						pool.PutObject(particles[i]);
						particles.RemoveAt(i);
					}
					particles.RemoveRange(0, particles.Count - MaxParticles);
				}

			}

		}
	}
}


#region BACKUP
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Scripts;
//using Microsoft.Xna.Framework;
//using System.Diagnostics;
//using Microsoft.Xna.Framework.Input;
//using System.Threading;
//using MonoGame.Extended;
//using Microsoft.Xna.Framework.Graphics;
//using System.Drawing.Design;
//
//namespace Engine
//{
//    public class ParticleSystem : Component
//    {
//        public object listLock = new object();
//
//        Pool<Particle> pool = new Pool<Particle>(() => new Particle());
//
//        public List<Particle> particles = new List<Particle>(1000000);
//        private ParticleSystemRenderer renderer;
//        private Vector2 StartVelocity = new Vector2(100, -200);
//        [ShowInEditor] public float radius { get; set; } = 200;
//        [ShowInEditor] public float speed { get; set; } = 4;
//        [ShowInEditor] public float StartSize { get; set; } = 10;
//        [System.ComponentModel.Editor(typeof(Editor.ColorPickerEditor), typeof(UITypeEditor))]
//        [ShowInEditor] public Color StartColor { get; set; } = Color.White;
//        private float StartVelocityVariation = 40;
//        private int MaxParticles = 100;
//        public override void Awake()
//        {
//            renderer = gameObject.AddComponent<ParticleSystemRenderer>();
//            renderer.particleSystem = this;
//            base.Awake();
//        }
//        Vector2 lastMousePos = new Vector2(0, 0);
//        public override void Update()
//        {
//            if (Mouse.GetState().RightButton == ButtonState.Pressed)
//            {
//                particles.Clear();
//            }
//            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
//            {
//                lastMousePos = MouseInput.Position;
//            }
//            ///// Space filling
//            /*for (int i = 0; i < 10; i++)
//            {
//
//                Particle p = new Particle();
//                Vector2 dir = (lastMousePos - MouseInput.Position).NormalizedCopy();
//                float l = (lastMousePos - MouseInput.Position).Length();
//                p.position = MouseInput.Position + dir * l * (i / 10);
//                Random rnd = new Random();
//                //p.velocity = StartVelocity + new Vector2(rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation), rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation));
//                p.velocity = (lastMousePos - MouseInput.Position).NormalizedCopy() * 80;
//                particles.Add(p);
//
//                if (particles.Count > MaxParticles)
//                {
//                    particles.RemoveRange(0, particles.Count - MaxParticles);
//                }
//            }*/
//Particle p = pool.GetObject();
//p.lifetime = 0;
//            //p.position = MouseInput.Position;
//            p.radius = StartSize;
//            //float sineY = (float)Math.Sin(Time.elapsedTime * 4) * 200 * (Extensions.Clamp((float)Math.Abs(Math.Sin(Time.elapsedTime)), 0.6f, 1f));
//            //float sineX = (float)Math.Cos(Time.elapsedTime * 4) * 200 * (Extensions.Clamp((float)Math.Abs(Math.Cos(Time.elapsedTime)), 0.6f, 1f));
//
//            Vector2 center = Camera.GetInstance().Size;
//
//float sineX = (float)Math.Cos(Time.elapsedTime * speed) * radius;
//float sineY = (float)Math.Sin(Time.elapsedTime * speed) * radius;
//
//Vector2 wiggle = new Vector2(sineX, sineY).NormalizedCopy() * (float)Math.Sin(Time.elapsedTime * 25) * 10;
//
//            if (Vector2.Distance(lastMousePos, center / 2 + new Vector2(sineX, sineY)) < 250)
//            {
//                wiggle *= 0;
//            }
//
//            p.position = new Vector2(center.X / 2 + sineX + wiggle.X, center.Y / 2 + sineY + wiggle.Y);
//
////p.velocity = StartVelocity + new Vector2(rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation), rnd.Next((int)-StartVelocityVariation, (int)StartVelocityVariation));
////p.velocity = (lastMousePos - MouseInput.Position).NormalizedCopy() * 80;
//p.color = StartColor;
//            p.color = Extensions.ColorFromHSVToXna(Time.elapsedTime* 50, 1, 1);
//            lock (listLock)
//            {
//                particles.Add(p);
//
//                if (particles.Count > MaxParticles)
//                {
//                    int num = particles.Count - MaxParticles;
//                    for (int i = 0; i<num; i++)
//                    {
//
//                        pool.PutObject(particles[i]);
//                        particles.RemoveAt(i);
//                    }
//                    //particles.RemoveRange(0, particles.Count - MaxParticles);
//                }
//            }
//
//            //}
//            Parallel.For(0, particles.Count, (i) =>
//                {
//                    //particles[i].velocity -= Physics.gravity * Time.deltaTime;
//
//                    //particles[i].position += particles[i].velocity * Time.deltaTime;
//
//                    particles[i].lifetime += Time.deltaTime;
//
//                    particles[i].color = new Color((int) particles[i].color.R, particles[i].color.G, particles[i].color.B, (int)((0.1f / particles[i].lifetime) * 255));
//                    /*particles[i].color = new Color((int)((0.1f / particles[i].lifetime) * 255),
//                        20, 20, (int)((0.1f / particles[i].lifetime) * 255));*/
//                    particles[i].color = new Color(particles[i].color.R, particles[i].color.G, particles[i].color.B,
//                        ((int)((0.01f / particles[i].lifetime) * 255)));
//                    particles[i].radius = Extensions.Clamp((1f / particles[i].lifetime* 3), 0, StartSize);
//                });
//
//            /*
//    float dist = Vector2.Distance(MouseInput.Position, particles[i].transform.Position);
//    float hue = dist * 0.8f;
//    float saturation = 1;
//    float value = 1;
//    if (dist > 30)
//    {
//    //saturation = 0.5f;
//    value = 0.1f;
//    }
//    if (dist > 30 + ringOffset && dist < 50 + ringOffset)
//    {
//    if (Extensions.AngleBetween(particles[i].transform.Position, Extensions.Round(MouseInput.Position)) == Math.PI / 180 * 45)
//    {
//        value = MathHelper.Clamp(1 / (ringOffset / 50), 0, 1);
//    }
//
//    }
//    hue = (float)Math.Round(hue / 20) * 20;
//    particles[i].particleRenderer.Color = Extensions.ColorFromHSV(hue + hueOffset, saturation, value).ToOtherColor();
//    });
//    if (ringOffset > 250) { ringOffset = 0; }
//    ringOffset += Time.deltaTime * 200;
//    hueOffset += Time.deltaTime * 400;*/
//            base.Update();
//        }
//    }
//}
#endregion


