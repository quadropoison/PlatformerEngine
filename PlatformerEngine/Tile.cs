using System;
using SFML.Graphics;
using SFML.System;

namespace PlatformerEngine
{
    enum TileType
    {
        NONE, 
        GROUND
    }

    class Tile: Transformable, Drawable
    {
        public const int TileSize = 8;

        TileType type = TileType.GROUND;
        RectangleShape rectangleShape;

        Tile topTile;
        Tile bottomTile;
        Tile leftTile;
        Tile rightTile;

        public Tile TopTile
        {
            set
            {
                topTile = value;
                UpdateView();
            }
            get { return topTile; }
        }
        public Tile BottomTile
        {
            set
            {
                bottomTile = value;
                UpdateView();
            }
            get { return bottomTile; }
        }
        public Tile LeftTile
        {
            set
            {
                leftTile = value;
                UpdateView();
            }
            get { return leftTile; }
        }
        public Tile RightTile
        {
            set
            {
                rightTile = value;
                UpdateView();
            }
            get { return rightTile; }
        }

        public Tile(TileType type, Tile topTile, Tile bottomTile, Tile leftTile, Tile rightTile)
        {
            this.type = type;

            if (topTile != null)
            {
                TopTile = topTile;
                TopTile.BottomTile = this;
            }
            if (bottomTile != null)
            {
                BottomTile = bottomTile;
                BottomTile.TopTile = this;
            }
            if (leftTile != null)
            {
                LeftTile = leftTile;
                LeftTile.RightTile = this;
            }
            if (rightTile != null)
            {
                RightTile = rightTile;
                RightTile.LeftTile = this;
            }

            rectangleShape = new RectangleShape(new Vector2f(TileSize, TileSize));

            switch (type)
            {
                case TileType.GROUND:
                    rectangleShape.Texture = Content.Tiles;
                    break;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            if (topTile != null && bottomTile != null && leftTile != null && rightTile != null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(1 + i, 1);
            }

            else if (topTile == null && bottomTile == null && leftTile == null && rightTile == null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(9 + i, 3);
            }

            // .. //
            else if (topTile == null && bottomTile != null && leftTile != null && rightTile != null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(9 + i, 4);
            }

            else if (topTile != null && bottomTile == null && leftTile != null && rightTile != null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(1 + i, 1);
            }

            else if (topTile != null && bottomTile != null && leftTile == null && rightTile != null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(1 + i, 1);
            }

            else if (topTile != null && bottomTile != null && leftTile != null && rightTile == null)
            {
                int i = Program.Random.Next(0, 3);
                rectangleShape.TextureRect = GetTextureRect(1 + i, 1);
            }

            // .. //

        }

        public IntRect GetTextureRect(int i, int j)
        {
            int x = i * TileSize + i * 2; 
            int y = j * TileSize + j * 2; 
            return new IntRect(x, y, TileSize, TileSize);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(rectangleShape, states);
        }
    }
}
