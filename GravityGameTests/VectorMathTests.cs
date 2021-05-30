using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GravityGameTests
{
    [TestClass()]
    public class VectorMathTests
    {
        [TestMethod()]
        public void VectorMultiplyingTest()
        {
            var vector = new Vector2(100, 0);

            var unitVector = new Vector2(1, 0);

            var result = Vector2.Multiply(vector, unitVector);

            Assert.AreEqual(100, result.X);
            Assert.AreEqual(0, result.Y);
        }
    }
}
