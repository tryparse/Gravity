using Microsoft.Xna.Framework;

namespace Illumination
{
    public class PointLight
    {
        /// <summary>
        /// Light's world position
        /// </summary>
        public Vector2 WorldPosition { get; set; }
        public Color Color { get; set; }
        public float Radius { get; set; }
    }
}
