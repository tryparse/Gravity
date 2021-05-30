using System;
using Microsoft.Xna.Framework.Graphics;

namespace Illumination
{
    class RenderTargetFactory
    {
        private GraphicsDevice _graphicsDevice;

        public RenderTargetFactory(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public RenderTarget2D CreateRenderTarget(RenderTargetUsage renderTargetUsage = RenderTargetUsage.DiscardContents)
        {
            return new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                true,
                _graphicsDevice.PresentationParameters.BackBufferFormat,
                _graphicsDevice.PresentationParameters.DepthStencilFormat,
                _graphicsDevice.PresentationParameters.MultiSampleCount,
                renderTargetUsage);
        }
    }
}
