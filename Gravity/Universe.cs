using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    public class Universe
    {
        public const float G = 6.6740831f;

        static Random _random = new Random();

        public static float CalculateGravityForce(float mass1, float mass2, float distance)
        {
            return G * (mass1 * mass2) / MathF.Pow(distance, 2);
        }

        public static float CalculateFirstCosmicVelocity(float mass, float distance)
        {
            var v1 = MathF.Sqrt(G * mass / distance);

            return v1;
        }

        public static Vector2 CalculateFirstCosmicVelocity(float firstBodyMass, Vector2 firstBodyPosition, Vector2 secondBodyPosition)
        {
            var distance = Vector2.Distance(firstBodyPosition, secondBodyPosition);
            var unitVector = Vector2.Normalize(firstBodyPosition - secondBodyPosition);
            var direction = Vector2.Normalize(new Vector2(unitVector.Y, -unitVector.X));

            var velocity = Vector2.Multiply(direction, MathF.Sqrt(G * firstBodyMass / distance));

            return velocity;
        }

        public static float CalculateEscapeVelocity(float mass, float distance)
        {
            var v2 = MathF.Sqrt(2 * G * (mass / distance));

            return v2;
        }

        public static float CalculateLagrangianPoint1(float M1, float M2, float R)
        {
            var alpha = M2 / (M1 + M2);
            
            var l1 = R * (1 - MathF.Pow(alpha / 3f, 1f / 3f));

            return l1;
        }

        public static float CalculateLagrangianPoint2(float M1, float M2, float R)
        {
            var alpha = M2 / (M1 + M2);

            var l2 = R * (1 + MathF.Pow(alpha / 3f, 1f / 3f));

            return l2;
        }

        public static float CalculateLagrangianPoint3(float M1, float M2, float R)
        {
            var alpha = M2 / (M1 + M2);

            var l2 = -R * (1 + (5f / 12f) * alpha);

            return l2;
        }

        public static Vector2 GetRandomOrbitCoordinate(Vector2 center, float radius)
        {
            var a = _random.NextAngle();

            var x = (float)(center.X + radius * MathF.Cos(a));
            var y = (float)(center.Y + radius * MathF.Sin(a));

            return new Vector2(x, y);
        }

        public static float TimeSpeed = 200f;

        public static bool TimeIsRunning = false;
    }
}
