using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class PositionComponent
    {
        public Vector2 Value { get; set; }

        public PositionComponent(Vector2 value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
