using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System.Linq;

namespace Gravity.Systems
{
    class CameraAnchorSystem : EntityUpdateSystem
    {
        private readonly OrthographicCamera _gameCamera;

        private ComponentMapper<PositionComponent> _positionData;

        public CameraAnchorSystem(OrthographicCamera gameCamera)
            : base(new AspectBuilder().All(typeof(PositionComponent), typeof(CameraAnchorComponent)))
        {
            _gameCamera = gameCamera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionData = mapperService.GetMapper<PositionComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if (!ActiveEntities.IsEmpty)
            {
                var entityId = ActiveEntities
                    .First();

                _gameCamera.LookAt(_positionData.Get(entityId).Value);
            }
        }
    }
}
