using System;

namespace Kaboom
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Kaboom game = new Kaboom())
            {
                game.Run();
            }
        }
    }
#endif
}

