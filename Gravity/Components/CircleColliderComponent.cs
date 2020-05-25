using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class CircleColliderComponent
    {
        public CircleF CircleShape { get; set; }

        public CircleColliderComponent(CircleF circle)
        {
            CircleShape = circle;
        }
    }
}
