using Gravity.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gravity.Systems
{
    class TrajectorySystem : EntityProcessingSystem
    {
        private ComponentMapper<TrajectoryComponent> _trajectorySystemMapper;
        private ComponentMapper<PositionComponent> _positionMapper;

        public TrajectorySystem()
            : base(new AspectBuilder().All(typeof(TrajectoryComponent), typeof(PositionComponent)))
        {
            
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trajectorySystemMapper = mapperService.GetMapper<TrajectoryComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            if (!Universe.TimeIsRunning)
            {
                return;
            }

            var position = _positionMapper.Get(entityId);
            var trajectory = _trajectorySystemMapper.Get(entityId);

            trajectory.AddHistoryEntry(position.Value);
        }
    }
}
