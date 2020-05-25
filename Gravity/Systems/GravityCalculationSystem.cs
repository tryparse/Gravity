using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gravity.Systems
{
    class GravityCalculationSystem : EntityUpdateSystem
    {
        ComponentMapper<PositionComponent> _positionMapper;
        ComponentMapper<MassComponent> _massMapper;
        ComponentMapper<GravityForceComponent> _gravityForceMapper;

        public GravityCalculationSystem()
            : base(new AspectBuilder().All(typeof(PositionComponent), typeof(MassComponent), typeof(GravityForceComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _massMapper = mapperService.GetMapper<MassComponent>();
            _gravityForceMapper = mapperService.GetMapper<GravityForceComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Universe.TimeIsRunning)
            {
                return;
            }

            foreach (var id in ActiveEntities)
            {
                var calculatedBodyPosition = _positionMapper.Get(id);
                var calculatedBodyMass = _massMapper.Get(id);
                var calculatedBodyGravityForce = _gravityForceMapper.Get(id);
                var resultForce = Vector2.Zero;

                foreach (var otherBodyId in ActiveEntities.Where(x => x != id))
                {
                    var otherBodyPosition = _positionMapper.Get(otherBodyId);
                    var otherBodyMass = _massMapper.Get(otherBodyId);

                    var sqrDistance = Vector2.DistanceSquared(otherBodyPosition.Value, calculatedBodyPosition.Value);
                    var forceDirection = Vector2.Normalize(otherBodyPosition.Value - calculatedBodyPosition.Value);
                    var force = forceDirection * Universe.CalculateGravityForce(otherBodyMass.Value, calculatedBodyMass.Value, sqrDistance);

                    resultForce += force;
                }

                calculatedBodyGravityForce.Value = resultForce;
            }
        }

        
    }
}
