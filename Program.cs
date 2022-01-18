using System;
using Engine;

namespace Noice
{
    public static class Program
    {
        static void Main()
        {
            Scene scene = new Scene(1000, 500, "Yay!");
            scene.Run();
        }
    }
}
