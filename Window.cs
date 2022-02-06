﻿using OpenTK.Windowing.Common;
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
		public RenderTexture sceneRenderTexture;
		public RenderTexture postProcessRenderTexture;
		public Window() : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(1920, 1027), APIVersion = new Version(4, 6) })
		{
			I = this;

			WindowState = WindowState.Maximized;
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

			imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

			Editor.I.Init();

			Scene.I.Start();
			sceneRenderTexture = new RenderTexture();
			postProcessRenderTexture = new RenderTexture();

		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			// Update the opengl viewport
			//GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			// Tell ImGui of the new size
			imGuiController?.WindowResized(ClientSize.X, ClientSize.Y);
		}
		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			Scene.I.Update();
			Editor.I.Update();
			base.OnUpdateFrame(args);
		}
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.ClearColor(0.6f, 0.4f, 0.5f, 1.000f);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			sceneRenderTexture.Bind();
			GL.Viewport(0, 0, (int)Camera.I.size.X, (int)Camera.I.size.Y);

			//GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			Scene.I.Render();

			sceneRenderTexture.Unbind();

			postProcessRenderTexture.Bind();


			// draw sceneRenderTexture with post process
			postProcessRenderTexture.RenderWithPostProcess(sceneRenderTexture.colorAttachment);

			postProcessRenderTexture.Unbind();


			// ------------- IMGUI -------------
			imGuiController.Update(this, (float)e.Time);
			GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

			imGuiController.WindowResized(ClientSize.X, ClientSize.Y);

			Editor.I.Draw();
			imGuiController.Render();
			// ------------- IMGUI -------------


			SwapBuffers();
			base.OnRenderFrame(e);
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