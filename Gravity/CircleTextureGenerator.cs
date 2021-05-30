using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    class CircleTextureGenerator
    {
        public Texture2D Generate(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            var texture = new Texture2D(graphicsDevice, width, height);
            var pixels = new Color[width * height];
            texture.GetData(pixels);

            

            texture.SetData(pixels);

            return texture;
        }
    }
}
