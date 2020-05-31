using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity.Systems
{
    class CelestialsMovementSystem : EntityProcessingSystem
    {
        ComponentMapper<PositionComponent> _positionMapper;
        ComponentMapper<MassComponent> _massMapper;
        ComponentMapper<GravityForceComponent> _gravityForceMapper;
        ComponentMapper<VelocityComponent> _velocityMapper;

        public CelestialsMovementSystem()
            : base(new AspectBuilder().All(typeof(PositionComponent), typeof(MassComponent), typeof(GravityForceComponent), typeof(VelocityComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _massMapper = mapperService.GetMapper<MassComponent>();
            _gravityForceMapper = mapperService.GetMapper<GravityForceComponent>();
            _velocityMapper = mapperService.GetMapper<VelocityComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            if (!Universe.TimeIsRunning)
            {
                return;
            }

            var position = _positionMapper.Get(entityId);
            var mass = _massMapper.Get(entityId);
            var gravityForce = _gravityForceMapper.Get(entityId);
            var velocity = _velocityMapper.Get(entityId);

            var gravityAcceleration = Vector2.Divide(gravityForce.Value, mass.Value);
            var dTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;

            velocity.Value += gravityAcceleration * dTime * Universe.TimeSpeed;
            position.Value += velocity.Value * dTime * Universe.TimeSpeed;
        }
    }
}
