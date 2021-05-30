using System;
using System.Collections.Generic;
using System.Text;
using Gravity.Components;
using Gravity.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Gravity.Systems.Rendering
{
    class CelestialsSpriteRenderingSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteFont _spriteFont;
        private readonly SpriteBatch _spriteBatch;
        private readonly OrthographicCamera _camera;
        ComponentMapper<PositionComponent> _positionMapper;
        ComponentMapper<RadiusComponent> _radiusMapper;
        ComponentMapper<SpriteComponent> _spriteMapper;

        public CelestialsSpriteRenderingSystem(GraphicsDevice graphicsDevice, SpriteFont spriteFont, OrthographicCamera camera)
            : base(new AspectBuilder()
                  .All(typeof(PositionComponent), typeof(RadiusComponent), typeof(SpriteComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteFont = spriteFont;
            _camera = camera;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            foreach (var id in ActiveEntities)
            {
                var position = _positionMapper.Get(id);
                var radius = _radiusMapper.Get(id);
                var sprite = _spriteMapper.Get(id);
                var scale = new Vector2(
                    radius.Value * 2 / sprite.Sprite.TextureRegion.Width);

                _spriteBatch.Draw(sprite.Sprite, position.Value, 0f, scale);
            }

            _spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _radiusMapper = mapperService.GetMapper<RadiusComponent>();
            _spriteMapper = mapperService.GetMapper<SpriteComponent>();
        }
    }

    class CelestialsPrimitiveRenderingSystem : EntityDrawSystem
    {
        private readonly Color DefaultColor = Color.Red;

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

        public CelestialsPrimitiveRenderingSystem(GraphicsDevice graphicsDevice, SpriteFont spriteFont, OrthographicCamera camera)
            : base(new AspectBuilder()
                  .All(typeof(PositionComponent), typeof(RadiusComponent)))
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _spriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _spriteBatch = new SpriteBatch(_graphicsDevice);

            LoadContent();
        }

        private void LoadContent()
        {

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
                    : DefaultColor;

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

                _spriteBatch.DrawString(_spriteFont, $"ID={id}", _camera.WorldToScreen(position.Value), DefaultColor);
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
