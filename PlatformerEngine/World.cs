using SFML.Graphics;
using SFML.System;

namespace PlatformerEngine
{
    internal class World : Transformable, Drawable
    {
        public const int WorldSize = 400;
        private readonly Sprite BackgroundSprite;

        private readonly Chunk[][] Chunks;

        private readonly Text GreetingsText;

        public World()
        {
            GreetingsText = new Text
            {
                Font = Content.BitwiseFont,
                DisplayedString = "Alpha v 0.01",
                CharacterSize = 12,
                Color = Color.White,
                Style = Text.Styles.Regular
            };

            BackgroundSprite = new Sprite(Content.Background)
            {
                Position = new Vector2f(0, 0),
                Scale = new Vector2f(1f, 1f)
            };

            Chunks = new Chunk[WorldSize][];

            for (var i = 0; i < WorldSize; i++)
                Chunks[i] = new Chunk[WorldSize];
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(BackgroundSprite);
            target.Draw(GreetingsText);

            for (var x = 0; x < WorldSize; x++)
                for (var y = 0; y < WorldSize; y++)
                {
                    if (Chunks[x][y] == null) continue;

                    target.Draw(Chunks[x][y], states);
                }
        }

        public void GenerateWorld()
        {
            GenerateCouplePlatforms(8);
            GenerateWall(8, 14, 16, 18);
            GenerateWall(4, 6, 12, 14);

            GenerateWall(7, 7, 6, 7);
            GenerateWall(8, 8, 6, 7);

            GenerateWall(2, 2, 6, 7);
            GenerateWall(1, 1, 6, 7);

            GeneratePlatform(11, 16, 7, 9);
        }

        private void GenerateWall(int startPositionX, int lastPositionX, int startPositionY, int lastPositionY)
        {
            for (var x = startPositionX; x <= lastPositionX; x++)
                for (var y = startPositionY; y <= lastPositionY; y++)
                    SetTile(TileType.GROUND, x, y);
        }

        private void GenerateCouplePlatforms(int count)
        {
            var startPositionX = 2;
            var lastPositionX = 8;
            var startPositionY = 8;
            var lastPositionY = 10;

            var step = 6;

            while (count != 0)
            {
                GeneratePlatform(startPositionX, lastPositionX, startPositionY, lastPositionY);

                startPositionX += step;
                lastPositionX += step;
                startPositionY += step;
                lastPositionY += step;

                count--;
            }
        }

        private void GeneratePlatform(int startPositionX, int lastPositionX, int startPositionY, int lastPositionY)
        {
            for (var x = startPositionX; x < lastPositionX; x++)
                for (var y = startPositionY; y < lastPositionY; y++)
                    SetTile(TileType.GROUND, x, y);
        }

        public void SetTile(TileType type, int x, int y)
        {
            var chunk = GetChunk(x, y);
            var tilePos = GetTilePosFromChunk(x, y);

            var topTile = GetTile(x, y - 1);
            var bottomTile = GetTile(x, y + 1);
            var leftTile = GetTile(x - 1, y);
            var rightTile = GetTile(x + 1, y);

            chunk.SetTile(type, tilePos.X, tilePos.Y, topTile, bottomTile, leftTile, rightTile);
        }

        public Tile GetTile(int x, int y)
        {
            var chunk = GetChunk(x, y);

            if (chunk == null)
                return null;

            var tilePos = GetTilePosFromChunk(x, y);

            return chunk.GetTile(tilePos.X, tilePos.Y);
        }

        public Chunk GetChunk(int x, int y)
        {
            var tempX = x/Chunk.ChunkSize;
            var tempY = y/Chunk.ChunkSize;

            if ((tempX >= WorldSize) || (tempY >= WorldSize))
                return null;

            if (Chunks[tempX][tempY] == null)
                Chunks[tempX][tempY] = new Chunk(new Vector2i(tempX, tempY));

            return Chunks[tempX][tempY];
        }

        public Vector2i GetTilePosFromChunk(int x, int y)
        {
            var tempX = x/Chunk.ChunkSize;
            var tempY = y/Chunk.ChunkSize;

            return new Vector2i(x - tempX*Chunk.ChunkSize, y - tempY*Chunk.ChunkSize);
        }
    }
}