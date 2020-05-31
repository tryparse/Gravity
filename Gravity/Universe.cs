using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    class Universe
    {
        public const float G = 6.6740831f;

        public static float CalculateGravityForce(float mass1, float mass2, float distance)
        {
            return G * (mass1 * mass2) / MathF.Pow(distance, 2);
        }

        public static float CalculateFirstCosmicVelocity(float mass, float distance)
        {
            var v1 = MathF.Sqrt(G * mass / distance);

            return v1;
        }

        public static float CalculateEscapeVelocity(float mass, float distance)
        {
            var v2 = MathF.Sqrt(2 * G * (mass / distance));

            return v2;
        }

        public static float TimeSpeed = 500;

        public static bool TimeIsRunning = false;
    }
}
