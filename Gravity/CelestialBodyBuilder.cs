using Gravity.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gravity
{
    interface ICelestialBodyBuilder
    {
        ICelestialBodyBuilder Reset();

        ICelestialBodyBuilder WithPosition(Vector2 position);

        ICelestialBodyBuilder WithRadius(float radius);

        ICelestialBodyBuilder WithMass(float mass);

        ICelestialBodyBuilder WithVelocity(Vector2 velocity);

        ICelestialBodyBuilder WithColor(Color color);

        ICelestialBodyBuilder WithTrajectory();

        Entity Build();
    }

    class CelestialBodyBuilder : ICelestialBodyBuilder
    {
        World _world;

        private PositionComponent _positionComponent;
        private MassComponent _massComponent;
        private RadiusComponent _radiusComponent;

        private bool _withVelocity = false;
        private VelocityComponent _velocityComponent;

        private GravityForceComponent _gravityForceComponent;

        private bool _withTrajectory = false;
        private TrajectoryComponent _trajectoryComponent;

        private bool _withColor = false;
        private ColorComponent _colorComponent;

        public CelestialBodyBuilder(World world)
        {
            _world = world;

            Reset();
        }

        public ICelestialBodyBuilder Reset()
        {
            _positionComponent = new PositionComponent(Vector2.Zero);
            _massComponent = new MassComponent(0f);
            _radiusComponent = new RadiusComponent(0f);
            _gravityForceComponent = new GravityForceComponent(Vector2.Zero);

            return this;
        }

        public ICelestialBodyBuilder WithPosition(Vector2 position)
        {
            _positionComponent.Value = position;

            return this;
        }

        public ICelestialBodyBuilder WithRadius(float radius)
        {
            _radiusComponent.Value = radius;

            return this;
        }

        public ICelestialBodyBuilder WithMass(float mass)
        {
            _massComponent.Value = mass;

            return this;
        }

        public ICelestialBodyBuilder WithVelocity(Vector2 velocity)
        {
            _velocityComponent = new VelocityComponent(velocity);
            _withVelocity = true;

            return this;
        }

        public ICelestialBodyBuilder WithColor(Color color)
        {
            _colorComponent = new ColorComponent(color);
            _withColor = true;

            return this;
        }

        public ICelestialBodyBuilder WithTrajectory()
        {
            _trajectoryComponent = new TrajectoryComponent();
            _withTrajectory = true;

            return this;
        }

        public Entity Build()
        {
            var entity = _world.CreateEntity();

            entity.Attach(_positionComponent);
            entity.Attach(_massComponent);
            entity.Attach(_radiusComponent);
            entity.Attach(_gravityForceComponent);

            if (_withVelocity)
            {
                entity.Attach(_velocityComponent);
            }

            if (_withTrajectory)
            {
                entity.Attach(_trajectoryComponent);
            }

            if (_withColor)
            {
                entity.Attach(_colorComponent);
            }

            return entity;
        }
    }
}
