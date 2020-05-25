using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class MassComponent
    {
        public float Value { get; set; }

        public MassComponent(float mass)
        {
            Value = mass;
        }
    }
}
