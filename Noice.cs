using System;
using Dear_ImGui_Sample;
using Engine;

namespace Noice
{
	public static class Noice
	{
		static Serializer serializer;
		static Scene scene;

		static void Main()
		{
			serializer = new Serializer();
			scene = new Scene();

			_ = new Editor();

			Window window = new Window();

			window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;

			window.Run();
		}
	}
}
