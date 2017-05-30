using System.Security.Cryptography.X509Certificates;
using SFML.Graphics;
using SFML.System;

namespace PlatformerEngine
{
    class Chunk: Transformable, Drawable
    {
        public const int ChunkSize = 24;

        private Tile[][] tiles;

        private Vector2i ChunkPos;

        public Chunk(Vector2i chunkPos)
        {
            this.ChunkPos = chunkPos;

            Position = new Vector2f(chunkPos.X * ChunkSize * Tile.TileSize, chunkPos.Y * ChunkSize * Tile.TileSize);

            tiles = new Tile[ChunkSize][];
            
            for (int i = 0; i < ChunkSize; i++)
                tiles[i] = new Tile[ChunkSize];
        }

        public void SetTile(TileType type, int x, int y, Tile topTile, Tile bottomTile, Tile leftTile, Tile rightTile)
        {
            tiles[x][y] = new Tile(type, topTile, bottomTile, leftTile, rightTile);
            tiles[x][y].Position = new Vector2f(x * Tile.TileSize, y * Tile.TileSize) + Position;
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ChunkSize || y >= ChunkSize)
                return null;

            return tiles[x][y];
                                    
        }      

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    if (tiles[x][y] == null) continue;

                    target.Draw(tiles[x][y]);
                }
            }
        }
    }
}
