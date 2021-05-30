using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Illumination.Utility
{
    public static class AngleHelper
    {
        public static Angle CalculateAngle(Vector2 a, Vector2 b)
        {
            return new Angle((float)Math.Atan2(b.Y - a.Y, b.X - a.X), AngleType.Radian);
        }
    }
}
