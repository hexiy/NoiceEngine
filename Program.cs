using System;
using Dear_ImGui_Sample;
using Engine;

namespace Noice
{
	public static class Program
	{
		static void Main()
		{
			Scene scene = new Scene();
			scene.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
			scene.updateTime = 100;
			scene.renderTime = 100;

			scene.Run();
		}
	}
}
