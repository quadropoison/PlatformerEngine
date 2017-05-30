using System;
using SFML.Graphics;
using SFML.Window;

namespace PlatformerEngine
{
    class Program
    {
        private static RenderWindow win;
        public static RenderWindow Window { get { return win; }}
        public static Game Game { get; set; }
        public static Random Random { get; set; }

        static void Main(string[] args)
        {
            win = new RenderWindow(new VideoMode(600, 400), "Render Window");

            win.SetFramerateLimit(60);
            //win.SetVerticalSyncEnabled(true);

            win.Closed += WinClosed;
            win.Resized += WinResized;

            Content.LoadContent();

            Random = new Random();
            Game = new Game();     
           
            while (win.IsOpen)
            {
                win.DispatchEvents();

                Game.Update();

                win.Clear(Color.White);

                Game.Draw();

                win.Display();
            }
        }

        private static void WinResized(object sender, SizeEventArgs e)
        {
            win.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private static void WinClosed(object sender, EventArgs e)
        {
            win.Close();
        }
    }
}
