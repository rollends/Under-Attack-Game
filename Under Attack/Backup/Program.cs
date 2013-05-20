using System;

namespace UnderAttack
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main( string[] args )
        {
            using (UnderAttack game = new UnderAttack())
            {
                game.Run();
            }
        }
    }
}

