using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    class Universe
    {
        public const float G = 6.67f;

        public static float CalculateGravityForce(float mass1, float mass2, float distanceSquared)
        {
            return G * (mass1 * mass2) / distanceSquared;
        }

        public static float CalculateEscapeVelocity(float mass, float radius, float h)
        {
            var v2 = MathF.Sqrt(2 * G * mass / (radius + h));

            return v2;
        }

        public static float TimeSpeed = 100;

        public static bool TimeIsRunning = false;
    }
}
