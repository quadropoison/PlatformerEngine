using System.IO;
using SFML.Graphics;

namespace PlatformerEngine
{
    class Content
    {
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
              
        private const string ContentDirName = "content";
        private const string FontsDirName = "fonts";
        private const string BackgroundsDirName = "backgrounds";
        private const string SpritesDirName = "sprites";

        private static readonly string FontsDirPath = Path.Combine(CurrentDirectory, ContentDirName, FontsDirName);
        private static readonly string BackgroundsDirPath = Path.Combine(CurrentDirectory, ContentDirName, BackgroundsDirName);
        private static readonly string SpritesDirPath = Path.Combine(CurrentDirectory, ContentDirName, SpritesDirName);

        public static Font BitwiseFont { get; private set; }
        public static Texture Background { get; private set; }       
        public static Texture Tiles { get; private set; }
        public static Texture Player { get; private set; }

        public static void LoadContent()
        {
            BitwiseFont = new Font(FontsDirPath + "\\bitwise.ttf");
            Background = new Texture(BackgroundsDirPath + "\\city.png");
            Tiles = new Texture(SpritesDirPath + "\\Example.png");
            Player = new Texture(SpritesDirPath + "\\SlyBunk.png");
        }
    }
}
