using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class GravityForceComponent
    {
        public Vector2 Value { get; set; }

        public GravityForceComponent(Vector2 gravityForce)
        {
            Value = gravityForce;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
