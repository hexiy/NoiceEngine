namespace Noice;

public static class Noice
{
	static void Main()
	{

		_ = new Serializer();
		_ = new Scene();

		_ = new Editor();

		Window window = new Window();
		BufferCache.CreateBuffers();

		window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;

		window.Run();
	}
}
