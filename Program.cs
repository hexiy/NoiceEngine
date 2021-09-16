using System;
using Engine;

namespace Noice
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Editor _ = new Editor ();
            
            using (var game = new Scene())
                game.Run();
        }
    }
}
