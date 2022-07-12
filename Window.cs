using Dear_ImGui_Sample;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine;

internal class Window : GameWindow
{
	public RenderTexture bloomDownscaledRenderTexture;
	private ImGuiController imGuiController;
	public RenderTexture postProcessRenderTexture;
	public RenderTexture sceneRenderTexture;

	public Window() : base(GameWindowSettings.Default,
	                       new NativeWindowSettings
	                       {Size = new Vector2i(1920, 1027), APIVersion = new Version(4, 6)})
	{
		I = this;

		WindowState = WindowState.Maximized;
		//WindowState = WindowState.Fullscreen;
	}

	public static Window I { get; private set; }

	protected override void OnLoad()
	{
		base.OnLoad();

		Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

		imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);


		ShaderCache.CreateShaders();

		Editor.I.Init();
		Scene.I.Start();

		sceneRenderTexture = new RenderTexture(Camera.I.size);
		postProcessRenderTexture = new RenderTexture(Camera.I.size);
		bloomDownscaledRenderTexture = new RenderTexture(Camera.I.size);
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
		Debug.StartTimer("Scene Update");
		Scene.I.Update();
		Debug.EndTimer("Scene Update");

		if (Global.EditorAttached)
		{
			Editor.I.Update();
		}

		base.OnUpdateFrame(args);
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		Debug.CountStat("Draw Calls", 0);
		Debug.StartTimer("Scene Render");

		GL.ClearColor(0, 0, 0, 0);
		GL.Clear(ClearBufferMask.ColorBufferBit);

		sceneRenderTexture.Bind(); // start rendering to sceneRenderTexture
		GL.Viewport(0, 0, (int) Camera.I.size.X, (int) Camera.I.size.Y);

		GL.Enable(EnableCap.Blend);
		Scene.I.Render();
		GL.Disable(EnableCap.Blend);

		sceneRenderTexture.Unbind(); // end rendering to sceneRenderTexture

		postProcessRenderTexture.Bind();
		GL.ClearColor(0, 0, 0, 0);

		GL.Clear(ClearBufferMask.ColorBufferBit);

		// draw sceneRenderTexture.colorAttachment with post process- into postProcessRenderTexture target
		postProcessRenderTexture.RenderWithPostProcess(sceneRenderTexture.colorAttachment);
		postProcessRenderTexture.RenderSnow(sceneRenderTexture.colorAttachment);

		postProcessRenderTexture.Unbind();

		var bloom_enabled = true;
		var sampleSize = 0.1f;
		if (bloom_enabled)
		{
			bloomDownscaledRenderTexture.Bind();
			GL.ClearColor(0, 0, 0, 1);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			// draw postProcessRenderTexture.colorAttachment downscaled with bloom- into bloomDownscaledRenderTexture target
			//bloomDownscaledRenderTexture.RenderBloom(postProcessRenderTexture.colorAttachment, sampleSize);
			bloomDownscaledRenderTexture.Unbind();

			// now we need to draw bloomDownscaledRenderTexture.colorAttachment upscaled

			postProcessRenderTexture.Bind();
			// draw sceneRenderTexture with post process
			postProcessRenderTexture.Render(bloomDownscaledRenderTexture.colorAttachment, 1 / sampleSize);

			postProcessRenderTexture.Unbind();
		}

		// we use postProcessRenderTexture to draw the final image in imgui

		Debug.EndTimer("Scene Render");
		// ------------- IMGUI -------------
		imGuiController.Update(this, (float) e.Time);
		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

		imGuiController.WindowResized(ClientSize.X, ClientSize.Y);

		Editor.I.Draw();
		imGuiController.Render();
		// ------------- IMGUI -------------


		SwapBuffers();
		base.OnRenderFrame(e);


		//Debug.ClearTimers();
		Debug.ClearStats();
	}

	protected override void OnTextInput(TextInputEventArgs e)
	{
		base.OnTextInput(e);

		imGuiController.PressChar((char) e.Unicode);
	}

	protected override void OnMouseWheel(MouseWheelEventArgs e)
	{
		base.OnMouseWheel(e);

		imGuiController.MouseScroll(new Vector2(e.OffsetX, e.OffsetY));
	}
}