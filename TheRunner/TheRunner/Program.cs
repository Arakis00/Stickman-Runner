using System;

namespace TheRunner
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheRunner game = new TheRunner())
            {
                game.Run();
            }
        }
    }
#endif
}

