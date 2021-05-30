using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    class PlanetTextureGenerator
    {
        private readonly Game _game;

        public PlanetTextureGenerator(Game game)
        {
            _game = game;
        }

        public Texture2D Generate(int radius, Color color)
        {
            var width = radius * 2 + 1;
            var height = radius * 2 + 1;

            var texture = new Texture2D(_game.GraphicsDevice, width, height);

            var noise = GenerateWhiteNoise(width, height);

            var pixels = new Color[width * height];
            texture.GetData<Color>(pixels);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var center = texture.Bounds.Center.ToVector2();
                    var point = new Vector2(i, j);
                    var distance = Vector2.Distance(center, point);

                    if (distance <= radius)
                    {
                        var index = j + width * i;

                        var pixel = color;
                        pixel = Color.Multiply(pixel, noise[i][j]);
                        pixel.A = byte.MaxValue;

                        pixels[index] = pixel;
                    }
                }
            }

            texture.SetData<Color>(pixels);

            return texture;
        }

        float[][] GenerateWhiteNoise(int width, int height)
        {
            Random random = new Random(0); //Seed to 0 for testing
            float[][] noise = GetEmptyArray(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i][j] = (float)random.NextDouble() % 1;
                }
            }

            return noise;
        }

        private float[][] GetEmptyArray(int width, int height)
        {
            float[][] result = new float[width][];

            for (int i = 0; i < width; i++)
            {
                result[i] = new float[height];
            }

            return result;
        }
    }
}
