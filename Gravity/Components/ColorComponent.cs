using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class ColorComponent
    {
        public Color Value { get; set; }

        public ColorComponent(Color color)
        {
            Value = color;
        }
    }
}
