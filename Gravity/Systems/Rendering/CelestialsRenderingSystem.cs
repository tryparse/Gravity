using System;
using System.Collections.Generic;
using System.Text;
using Gravity.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Gravity.Systems.Rendering
{
    class CelestialsRenderingSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _spriteFont;
        private readonly OrthographicCamera _camera;

        ComponentMapper<PositionComponent> _positionMapper;
        ComponentMapper<RadiusComponent> _radiusMapper;
        ComponentMapper<GravityForceComponent> _gravityForceMapper;
        ComponentMapper<VelocityComponent> _velocityMapper;
        ComponentMapper<ColorComponent> _colorMapper;

        private readonly int DefaultThickness = 2;

        public CelestialsRenderingSystem(GraphicsDevice graphicsDevice, SpriteFont spriteFont, OrthographicCamera camera)
            : base(new AspectBuilder()
                  .All(typeof(PositionComponent), typeof(RadiusComponent)))
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _spriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawCelestials();

            DrawIds();
        }

        private void DrawCelestials()
        {
            var thickness = Math.Max(DefaultThickness, DefaultThickness / _camera.Zoom);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            foreach (var id in ActiveEntities)
            {
                var position = _positionMapper.Get(id);
                var radius = _radiusMapper.Get(id);
                var shape = new CircleF(position.Value, radius.Value);
                var screenBounds = _camera.BoundingRectangle;
                var color = _colorMapper.Has(id)
                    ? _colorMapper.Get(id).Value
                    : Color.Red;

                if (screenBounds.Intersects(shape))
                {
                    var sides = radius.Value <= 10
                        ? 16
                        : 32;

                    _spriteBatch.DrawCircle(position.Value, radius.Value, sides, color, thickness);
                }
                else
                {
                    var markerPosition = new Vector2
                    {
                        X = MathHelper.Clamp(position.Value.X, screenBounds.Left + 3, screenBounds.Right - 3),
                        Y = MathHelper.Clamp(position.Value.Y, screenBounds.Top + 3, screenBounds.Bottom - 3)
                    };

                    var markerRadius = 5 / _camera.Zoom;

                    _spriteBatch.DrawCircle(markerPosition, markerRadius, 10, color, thickness);
                }
            }

            _spriteBatch.End();
        }

        private void DrawIds()
        {
            _spriteBatch.Begin();

            foreach (var id in ActiveEntities)
            {
                var position = _positionMapper.Get(id);

                _spriteBatch.DrawString(_spriteFont, $"ID={id}", _camera.WorldToScreen(position.Value), Color.Red);
            }

            _spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _radiusMapper = mapperService.GetMapper<RadiusComponent>();
            _gravityForceMapper = mapperService.GetMapper<GravityForceComponent>();
            _velocityMapper = mapperService.GetMapper<VelocityComponent>();
            _colorMapper = mapperService.GetMapper<ColorComponent>();
        }
    }
}
