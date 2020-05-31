using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Systems
{
    class CameraAnchorSystem : EntityProcessingSystem
    {
        private readonly OrthographicCamera _gameCamera;

        private ComponentMapper<PositionComponent> _positionData;

        public CameraAnchorSystem(OrthographicCamera gameCamera)
            : base(new AspectBuilder().One(typeof(PositionComponent), typeof(CameraAnchorComponent)))
        {
            _gameCamera = gameCamera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionData = mapperService.GetMapper<PositionComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            _gameCamera.LookAt(_positionData.Get(entityId).Value);
        }
    }
}
