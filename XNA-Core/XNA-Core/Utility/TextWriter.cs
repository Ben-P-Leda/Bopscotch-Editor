using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leda.Core
{
    public sealed class TextWriter
    {
        private static SpriteFont _font = null;
        private static float _scale = 1.0f;
        private static Vector2 _padding = Vector2.Zero;

        public static Rectangle LastTextArea { get; private set; }

        public static void SetDefaults(SpriteFont font, float scale)
        {
            SetDefaults(font, scale, 0.0f);
        }

        public static void SetDefaults(SpriteFont font, float scale, float padding)
        {
            _font = font;
            _scale = scale;
            _padding.Y = padding;

            LastTextArea = Rectangle.Empty;
        }

        public static void Write(string text, SpriteBatch spriteBatch, Vector2 position, Color color, float depth, Alignment alignment)
        {
            if (_font != null) { Write(text, spriteBatch, _font, position, color, _scale, depth, alignment); }
        }

        public static void Write(string text, SpriteBatch spriteBatch, Vector2 position, Color color, float scale, float depth, Alignment alignment)
        {
            if (_font != null) { Write(text, spriteBatch, _font, position, color, scale, depth, alignment); }
        }

        public static void Write(string text, SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color, float scale,
            float depth, Alignment alignment)
        {
            DrawText(text, spriteBatch, font, position, color, GetOriginForAlignment(text, font, alignment), scale, depth);
        }

        public static void Write(string text, SpriteBatch spriteBatch, Vector2 position, Color innerColor, Color outlineColor,
            float outlineThickness, float depth, Alignment alignment)
        {
            if (_font != null) { Write(text, spriteBatch, _font, position, innerColor, outlineColor, outlineThickness, _scale, depth, alignment); }
        }

        public static void Write(string text, SpriteBatch spriteBatch, Vector2 position, Color innerColor, Color outlineColor,
            float outlineThickness, float scale, float depth, Alignment alignment)
        {
            if (_font != null) { Write(text, spriteBatch, _font, position, innerColor, outlineColor, outlineThickness, scale, depth, alignment); }
        }

        public static void Write(string text, SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color innerColor, Color outlineColor,
            float outlineThickness, float scale, float depth, Alignment alignment)
        {
            Vector2 origin = GetOriginForAlignment(text, font, alignment);

            DrawText(text, spriteBatch, font, position, outlineColor, origin + (new Vector2(-1.0f, -1.0f) * scale * outlineThickness),
                scale, depth + Outline_Render_Depth_Offset);
            DrawText(text, spriteBatch, font, position, outlineColor, origin + (new Vector2(1.0f, -1.0f) * scale * outlineThickness),
                scale, depth + Outline_Render_Depth_Offset);
            DrawText(text, spriteBatch, font, position, outlineColor, origin + (new Vector2(1.0f, 1.0f) * scale * outlineThickness),
                scale, depth + Outline_Render_Depth_Offset);
            DrawText(text, spriteBatch, font, position, outlineColor, origin + (new Vector2(-1.0f, 1.0f) * scale * outlineThickness),
                scale, depth + Outline_Render_Depth_Offset);

            DrawText(text, spriteBatch, font, position, innerColor, origin, scale, depth);
        }

        private static Vector2 GetOriginForAlignment(string text, SpriteFont font, Alignment alignment)
        {
            if (alignment == Alignment.Center) { return new Vector2(font.MeasureString(text).X / 2.0f, 0.0f); }
            else if (alignment == Alignment.Right) { return new Vector2(font.MeasureString(text).X, 0.0f); }
            return Vector2.Zero;
        }

        private static void DrawText(string text, SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color, Vector2 origin,
            float scale, float depth)
        {
            spriteBatch.DrawString(font, text, position + (_padding * scale), color, 0.0f, origin, scale, SpriteEffects.None, depth);
            CalculateLastTextArea(text, font, position + (_padding * scale), origin, scale);
        }

        private static void CalculateLastTextArea(string text, SpriteFont font, Vector2 position, Vector2 origin, float scale)
        {
            LastTextArea = new Rectangle( (int)(position.X - (origin.X * scale)), (int)position.Y, 
                (int)(font.MeasureString(text).X * scale), (int)(font.MeasureString(text).Y * scale));
        }

        public enum Alignment
        {
            Left,
            Center,
            Right
        }

        private const float Outline_Render_Depth_Offset = 0.0001f;
    }
}
