using SFML.System;

namespace PlatformerEngine
{
    class Game
    {
        World world;
        Player player;             

        public Game()
        {
            world = new World();
            world.GenerateWorld();

            player = new Player(world) {StartPosition = new Vector2f(37, 1)};

            player.Spawn();

            DebugRender.isEnabled = true;
        }

        public void Update()
        {
            player.Update();
        }

        public void Draw()
        {
            Program.Window.Draw(world);
            Program.Window.Draw(player);
            Program.Window.Draw(player.PlayerPosition);
            DebugRender.Draw(Program.Window);
        }
    }
}
