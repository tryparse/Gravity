using System;
using System.Collections.Generic;
using System.Text;
using Gravity.Systems;
using Gravity.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Gravity
{
    class GravityGame : Game
    {
        private const float DELTA_ZOOM = .05f;
        private readonly GraphicsDeviceManager _graphics;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private World _world;

        private ICelestialBodyBuilder _celestialBodyBuilder;

        private FramesPerSecondCounterComponent _fpsComponent;

        public GravityGame()
        {
            IsMouseVisible = true;

            _graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            _fpsComponent = new FramesPerSecondCounterComponent(this);
            
            Components.Add(_fpsComponent);
            Components.Add(new InputHandler(this));

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 736;
            _graphics.ApplyChanges();

            _camera = new OrthographicCamera(GraphicsDevice)
            {
                MaximumZoom = 1.5f,
                MinimumZoom = 0.05f
            };

            _camera.LookAt(Vector2.Zero);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content/bin";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Fonts/joystix");

            InitializeGameWorld();

            base.LoadContent();
        }

        private void InitializeGameWorld()
        {
            _world = new WorldBuilder()
                .AddSystem(new GravityCalculationSystem())
                .AddSystem(new CelestialsMovementSystem())
                .AddSystem(new TrajectorySystem())
                .AddSystem(new TrajectoryRenderingSystem(GraphicsDevice, _camera))
                .AddSystem(new CelestialsRenderingSystem(GraphicsDevice, _spriteFont, _camera))
                .Build();

            _celestialBodyBuilder = new CelestialBodyBuilder(_world);

            var starMass = 500f;
            var starRadius = 500f;

            _celestialBodyBuilder
                .Reset()
                .WithMass(starMass)
                .WithRadius(starRadius)
                .WithTrajectory()
                .WithPosition(new Vector2(0))
                .WithColor(Color.Yellow)
                .Build();

            _celestialBodyBuilder
                .Reset()
                .WithMass(25f)
                .WithRadius(25f)
                .WithPosition(new Vector2(3000f, 0f))
                .WithVelocity(new Vector2(0f, Universe.CalculateEscapeVelocity(500f, 500f, 2500f)))
                .WithTrajectory()
                .WithColor(Color.DarkBlue)
                .Build();

            //_celestialBodyBuilder
            //    .Reset()
            //    .WithMass(5f)
            //    .WithRadius(5f)
            //    .WithPosition(new Vector2(3200f, 0f))
            //    .WithVelocity(new Vector2(0f, -.2f))
            //    .WithTrajectory()
            //    .WithColor(Color.Gray)
            //    .Build();

            //_celestialBodyBuilder
            //    .Reset()
            //    .WithMass(25f)
            //    .WithRadius(25f)
            //    .WithPosition(new Vector2(1000, -50))
            //    .WithVelocity(new Vector2(.7f, .7f))
            //    .WithTrajectory()
            //    .WithColor(Color.IndianRed)
            //    .Build();
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.IsWheelScrolledDown)
            {
                _camera.ZoomIn(DELTA_ZOOM);
            }
            else if (InputHandler.IsWheelScrolledUp)
            {
                _camera.ZoomOut(DELTA_ZOOM);
            }

            if(InputHandler.IsKeyDown(Keys.Up))
            {
                _camera.Move(new Vector2(0, -3) / _camera.Zoom);
            }
            else if (InputHandler.IsKeyDown(Keys.Down))
            {
                _camera.Move(new Vector2(0, 3) / _camera.Zoom);
            }

            if (InputHandler.IsKeyDown(Keys.Left))
            {
                _camera.Move(new Vector2(-3, 0) / _camera.Zoom);
            }
            else if (InputHandler.IsKeyDown(Keys.Right))
            {
                _camera.Move(new Vector2(3, 0) / _camera.Zoom);
            }

            if (InputHandler.IsKeyPressed(Keys.Space))
            {
                Universe.TimeIsRunning = !Universe.TimeIsRunning;
            }

            _world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            _spriteBatch.Begin();
            
            _spriteBatch.DrawString(_spriteFont, _fpsComponent.FramesPerSecond.ToString(), CalculateFpsPosition(), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 CalculateFpsPosition()
        {
            var textSize = _spriteFont.MeasureString(_fpsComponent.FramesPerSecond.ToString());
            return new Vector2(_graphics.PreferredBackBufferWidth - textSize.X - 5, 5);
        }
    }
}
