using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Gravity.Systems
{
    class CollisionDetectSystem : EntityUpdateSystem
    {
        ComponentMapper<CircleColliderComponent> _collisionMapper;
        ComponentMapper<MassComponent> _massMapper;

        public CollisionDetectSystem()
            : base(new AspectBuilder().All(typeof(CircleColliderComponent), typeof(MassComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _collisionMapper = mapperService.GetMapper<CircleColliderComponent>();
            _massMapper = mapperService.GetMapper<MassComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Universe.TimeIsRunning)
            {
                return;
            }

            foreach (var entityId in ActiveEntities)
            {
                var collisionBody = _collisionMapper.Get(entityId);
                var collisionBodyMass = _massMapper.Get(entityId);

                foreach (var otherEntityId in ActiveEntities)
                {
                    if (otherEntityId == entityId)
                    {
                        return;
                    }

                    var otherBody = _collisionMapper.Get(otherEntityId);
                    var otherBodyMass = _collisionMapper.Get(otherEntityId);

                    if (collisionBody.CircleShape.Intersects(otherBody.CircleShape))
                    {
                        
                    }
                }
            }
        }
    }
}
