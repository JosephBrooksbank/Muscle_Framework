using System;
using KinectDemo.Scripts;

namespace MyKinectGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MyGame Game = new MyGame())
            {
                Game.Run();                
            }
        }
    }
#endif
}

