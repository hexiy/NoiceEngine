using OpenTK.Windowing.Common;
using Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Dear_ImGui_Sample;

namespace Engine
{
	class Window : GameWindow
	{
		public static Window I { get; private set; }
		ImGuiController imGuiController;

		public Window() : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(1500, 800), APIVersion = new Version(4, 6) })
		{
			I = this;
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

			imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
			Editor.I.Init();

			Scene.I.Start();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			// Update the opengl viewport
			//GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			// Tell ImGui of the new size
			imGuiController.WindowResized(ClientSize.X, ClientSize.Y);
		}
		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			Scene.I.Update();
			Editor.I.Update();
			base.OnUpdateFrame(args);
		}
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			GL.Viewport(0, (int)Window.I.ClientSize.Y - (int)Camera.I.size.Y, (int)Camera.I.size.X, (int)Camera.I.size.Y);

			Scene.I.Render();

			imGuiController.Update(this, (float)e.Time);
			GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
			imGuiController.WindowResized(ClientSize.X, ClientSize.Y);

			Editor.I.Draw();

			imGuiController.Render();

			SwapBuffers();
		}

		protected override void OnTextInput(TextInputEventArgs e)
		{
			base.OnTextInput(e);

			imGuiController.PressChar((char)e.Unicode);
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			imGuiController.MouseScroll(e.Offset);
		}
	}
}