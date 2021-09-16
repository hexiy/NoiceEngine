using System;

namespace Engine
{
    public static class Rendom
    {
        public static Random rnd = new Random();

        public static int Next(int min,int max)
        {
            return rnd.Next(max-min)+min;
        }
        public static float Next(float max)
        {
            return (float)rnd.NextDouble() * max;
        }
    }
}
