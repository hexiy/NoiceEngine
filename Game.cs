using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	abstract class Game
	{
		protected int InitialWindowWidth { get; set; }
		protected int InitialWindowHeight { get; set; }
		protected string InitialWindowTitle { get; set; }

		public Game(int initialWindowWidth, int initialWindowHeight, string initialWindowTitle)
		{
			this.InitialWindowWidth = initialWindowWidth;
			this.InitialWindowHeight = initialWindowHeight;
			this.InitialWindowTitle = initialWindowTitle;
		}

		public void Run()
		{
			DisplayManager.CreateWindow(InitialWindowWidth, InitialWindowHeight, InitialWindowTitle);

			Initialize();

			LoadContent();

			while (!Glfw.WindowShouldClose(DisplayManager.Window))
			{
				Update();

				Glfw.PollEvents();

				Render();
			}
			DisplayManager.CloseWindow();
		}
		protected abstract void Initialize();
		protected abstract void LoadContent();

		protected abstract void Update();
		protected abstract void Render();
	}
}