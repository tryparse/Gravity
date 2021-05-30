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
        private readonly Color DefaultColor = Color.Red;
        private readonly float DefaultThickness = 1f;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly OrthographicCamera _camera;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<TrajectoryComponent> _trajectoryData;
        private ComponentMapper<ColorComponent> _colorData;

        public TrajectoryRenderingSystem(GraphicsDevice graphicsDevice, OrthographicCamera camera)
            : base(new AspectBuilder().All(typeof(TrajectoryComponent)))
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));

            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var thickness = Math.Max(DefaultThickness, DefaultThickness / _camera.Zoom);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            foreach (var entityId in ActiveEntities)
            {
                var color = _colorData.Get(entityId)?.Value ?? DefaultColor;
                color.A = byte.MaxValue / 3;

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
