using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Systems.Rendering
{
    class RenderingSystem : MonoGame.Extended.Entities.Systems.DrawSystem
    {
        private readonly RenderingCore _renderingCore;
        private readonly OrthographicCamera _camera;
        private readonly SpriteBatch _spriteBatch;

        public RenderingSystem(RenderingCore renderingCore, OrthographicCamera camera)
        {
            _renderingCore = renderingCore ?? throw new ArgumentNullException(nameof(renderingCore));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));

            _spriteBatch = new SpriteBatch(_renderingCore.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_renderingCore.ColorMapRenderTarget2D, Vector2.Zero, Color.White);

            _spriteBatch.End();
        }
    }
}
