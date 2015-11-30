using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leda.Core
{
    public sealed class RenderTools
    {
        public static void Line(SpriteBatch spriteBatch, Texture2D texture, Vector2 origin, Vector2 target, float thickness, Color tint, float depth)
        {
            float Angle = Utility.PointToPointAngle(origin, target);
            Vector2 RenderLine = new Vector2(Vector2.Distance(origin, target), thickness);

            spriteBatch.Draw(texture, origin, null, tint, Angle, new Vector2(0.0f, (thickness * texture.Height) / 2.0f), RenderLine, SpriteEffects.None, depth);
        }
    }
}
