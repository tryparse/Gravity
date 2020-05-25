using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Components
{
    class RadiusComponent
    {
        private float _value;

        public float Value
        {
            get => _value;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException($"{nameof(value)} can not be negative");
                }

                _value = value;
            }
        }

        public RadiusComponent(float radius)
        {
            Value = radius;
        }
    }
}
