using Gravity.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Systems.Rendering
{
    class CelestialListDrawSystem : EntityDrawSystem
    {
        private const int LIST_ITEM_POSITION_MARGIN = 20;
        private const string ANCHOR_MARK = "*";
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteFont _spriteFont;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<ColorComponent> _colorData;
        private ComponentMapper<CameraAnchorComponent> _cameraAnchorData;

        private Color DefaultColor = Color.Red;

        public CelestialListDrawSystem(GraphicsDevice graphicsDevice, SpriteFont spriteFont) : base(new AspectBuilder().All(typeof(CelestialBodyTag)))
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _spriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));

            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var listItemPosition = new Vector2(10, 100);

            _spriteBatch.Begin();

            foreach(var entityId in ActiveEntities)
            {
                var color = _colorData.Get(entityId)?.Value ?? DefaultColor;
                var isAnchorForCamera = _cameraAnchorData.Get(entityId) != null;
                var anchorText = isAnchorForCamera ? ANCHOR_MARK : string.Empty;

                _spriteBatch.DrawString(_spriteFont, $"ID={entityId}{anchorText}", listItemPosition, color);

                listItemPosition.Y += LIST_ITEM_POSITION_MARGIN;
            }

            _spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _colorData = mapperService.GetMapper<ColorComponent>();
            _cameraAnchorData = mapperService.GetMapper<CameraAnchorComponent>();
        }
    }
}
