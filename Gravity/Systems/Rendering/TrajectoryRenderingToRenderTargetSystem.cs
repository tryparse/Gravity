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
    class TrajectoryRenderingToRenderTargetSystem : EntityDrawSystem
    {
        private readonly Color DefaultColor = Color.White;
        private readonly float DefaultThickness = 1f;
        private readonly RenderingCore _renderingCore;
        private readonly OrthographicCamera _camera;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<TrajectoryComponent> _trajectoryData;
        private ComponentMapper<ColorComponent> _colorData;

        public TrajectoryRenderingToRenderTargetSystem(RenderingCore renderingCore, OrthographicCamera camera)
            : base(new AspectBuilder().All(typeof(TrajectoryComponent)))
        {
            _renderingCore = renderingCore ?? throw new ArgumentNullException(nameof(renderingCore));
            
            _spriteBatch = new SpriteBatch(renderingCore.GraphicsDevice);
            _camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            var thickness = Math.Max(DefaultThickness, DefaultThickness / _camera.Zoom);

            _renderingCore.GraphicsDevice.SetRenderTarget(_renderingCore.ColorMapRenderTarget2D);

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

            _renderingCore.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trajectoryData = mapperService.GetMapper<TrajectoryComponent>();
            _colorData = mapperService.GetMapper<ColorComponent>();
        }
    }
}
