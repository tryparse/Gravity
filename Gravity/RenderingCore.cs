using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    class RenderingCore
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly RenderTarget2D _colorMapRenderTarget;
        private readonly RenderTarget2D _shadowMapRenderTarget;

        public GraphicsDevice GraphicsDevice => _graphicsDevice;

        public RenderTarget2D ColorMapRenderTarget2D => _colorMapRenderTarget;

        public RenderTarget2D ShadowMapRenderTarget2D => _shadowMapRenderTarget;

        public RenderingCore(GraphicsDevice graphicsDevice, int width, int height)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            _colorMapRenderTarget = new RenderTarget2D(graphicsDevice, width, height, true, SurfaceFormat.Color, DepthFormat.None);

            _shadowMapRenderTarget = new RenderTarget2D(graphicsDevice, width, height, true, SurfaceFormat.Color, DepthFormat.None);
        }
    }
}
