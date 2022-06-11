using OpenTK.Windowing.Common;

namespace Noice;

public static class Noice
{
	private static void Main()
	{
		_ = new Serializer();
		_ = new Scene();

		_ = new Editor();
		//_ = new GameWindow();

		var window = new Window();
		BufferCache.CreateBuffers();

		window.VSync = VSyncMode.Off;

		window.Run();
	}
}