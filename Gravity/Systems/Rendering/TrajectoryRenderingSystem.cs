using Gravity.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Systems.Rendering
{
    class TrajectoryRenderingSystem : EntityDrawSystem
    {
        private readonly Color DefaultColor = Color.White;
        private readonly float DefaultThickness = 2f;

        private readonly OrthographicCamera _camera;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<TrajectoryComponent> _trajectoryData;
        private ComponentMapper<ColorComponent> _colorData;

        public TrajectoryRenderingSystem(GraphicsDevice graphicsDevice, OrthographicCamera camera)
            : base(new AspectBuilder().All(typeof(TrajectoryComponent)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            var thickness = Math.Max(DefaultThickness, DefaultThickness / _camera.Zoom);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            foreach (var entityId in ActiveEntities)
            {
                var color = _colorData.Get(entityId)?.Value ?? DefaultColor;

                foreach (var trajectoryLine in _trajectoryData.Get(entityId).History)
                {
                    _spriteBatch.DrawLine(trajectoryLine.From, trajectoryLine.To, color, thickness);
                }
            }

            _spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trajectoryData = mapperService.GetMapper<TrajectoryComponent>();
            _colorData = mapperService.GetMapper<ColorComponent>();
        }
    }
}
