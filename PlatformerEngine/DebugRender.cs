using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace PlatformerEngine
{
    class DebugRender
    {
        public static bool isEnabled;

        static List<Drawable> objects = new List<Drawable>();

        private static void AddRectangle(float x, float y, float w, float h, Color color)
        {
            if (!isEnabled) return;

            var obj = new RectangleShape(new Vector2f(w, h));
            obj.Position = new Vector2f(x, y);
            obj.FillColor = Color.Transparent;
            obj.OutlineColor = color;
            obj.OutlineThickness = 1;
            objects.Add(obj);
        }

        public static void AddRectangle(FloatRect rect, Color color)
        {
            AddRectangle(rect.Left, rect.Top, rect.Width, rect.Height, color);
        }

        private static void AddPositionText(string name, int positionX, int positionY, Color color)
        {
            var obj = new Text();
            obj.DisplayedString = $"{name} position is X: {positionX} Y: {positionY}";
            obj.CharacterSize = 14;
            obj.Position = new Vector2f(10, 370);
            obj.Color = color;
            obj.Style = Text.Styles.StrikeThrough;
            obj.Font = Content.BitwiseFont;            
            objects.Add(obj);
        }

        private static void DrawTextBackgroundSprite()
        {
            var obj = new Sprite(Content.TextBackground)
            {
                Position = new Vector2f(0, 365),
                Scale = new Vector2f(0.38f, 0.05f)
            };

            objects.Add(obj);
        }

        public static void AddPositionText(string name, Vector2i position,  Color color)
        {
            DrawTextBackgroundSprite();
            AddPositionText(name, (int)position.X, (int)position.Y, color);
        }

        public static void Draw(RenderTarget target)
        {
            if (!isEnabled) return;

            foreach (var obj in objects)
            {
                target.Draw(obj);
            }

            objects.Clear();
        }
    }
}
