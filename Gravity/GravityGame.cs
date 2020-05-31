using System;
using System.Collections.Generic;
using System.Text;
using Gravity.Components;
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
        private const float CAMERA_MAX_ZOOM = 1.5f;
        private const float CAMERA_MIN_ZOOM = 0.02f;
        private const float CAMERA_ZOOM_STEP = .02f;
        private const float CAMERA_REDUCED_ZOOM_STEP = .002f;
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

            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _camera = new OrthographicCamera(GraphicsDevice)
            {
                MaximumZoom = CAMERA_MAX_ZOOM,
                MinimumZoom = CAMERA_MIN_ZOOM
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
                .AddSystem(new CameraAnchorSystem(_camera))
                .AddSystem(new TrajectorySystem())
                .AddSystem(new TrajectoryRenderingSystem(GraphicsDevice, _camera))
                .AddSystem(new CelestialsRenderingSystem(GraphicsDevice, _spriteFont, _camera))
                .Build();

            _celestialBodyBuilder = new CelestialBodyBuilder(_world);

            var starMass = 100f;

            _celestialBodyBuilder
                .Reset()
                .WithMass(starMass)
                .WithRadius(100f)
                .WithTrajectory()
                .WithPosition(new Vector2(0))
                .WithColor(new Color(230, 57, 70))
                //.WithVelocity(Vector2.Zero)
                .Build();

            var v1 = Universe.CalculateFirstCosmicVelocity(starMass, 2000f);
            var v2 = Universe.CalculateEscapeVelocity(starMass, 2000f);

            var planet = _celestialBodyBuilder
                .Reset()
                .WithMass(25f)
                .WithRadius(25f)
                .WithPosition(new Vector2(2000f, 0f))
                .WithVelocity(new Vector2(0, v1))
                .WithTrajectory()
                .WithColor(new Color(241, 250, 238))
                .Build();

            planet.Attach(new CameraAnchorComponent());

            _celestialBodyBuilder
                .Reset()
                .WithMass(5f)
                .WithRadius(5f)
                .WithPosition(new Vector2(2500f, 0f))
                .WithVelocity(new Vector2(0f, v1 + Universe.CalculateFirstCosmicVelocity(25f, 500f)))
                .WithTrajectory()
                .WithColor(new Color(29, 53, 87))
                .Build();
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }

            if (InputHandler.IsKeyPressed(Keys.Enter)
                && InputHandler.IsKeyPressed(Keys.RightAlt))
            {
                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
            }

            if (InputHandler.IsWheelScrolledDown)
            {
                if (_camera.Zoom <= CAMERA_ZOOM_STEP)
                {
                    _camera.ZoomIn(CAMERA_REDUCED_ZOOM_STEP);
                }
                else
                {
                    _camera.ZoomIn(CAMERA_ZOOM_STEP);
                }
            }
            else if (InputHandler.IsWheelScrolledUp)
            {
                if (_camera.Zoom <= CAMERA_ZOOM_STEP)
                {
                    _camera.ZoomOut(CAMERA_REDUCED_ZOOM_STEP);
                }
                else
                {
                    _camera.ZoomOut(CAMERA_ZOOM_STEP);
                }
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
            _graphics.GraphicsDevice.Clear(new Color(69, 123, 157));

            _world.Draw(gameTime);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_spriteFont, $"Camera {{ Zoom:{_camera.Zoom} }}", CalculateCameraInfoPosition(), Color.Black);
            _spriteBatch.DrawString(_spriteFont, _fpsComponent.FramesPerSecond.ToString(), CalculateFpsPosition(), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 CalculateFpsPosition()
        {
            var textSize = _spriteFont.MeasureString(_fpsComponent.FramesPerSecond.ToString());
            return new Vector2(_graphics.PreferredBackBufferWidth - textSize.X - 5, 5);
        }

        private Vector2 CalculateCameraInfoPosition()
        {
            return new Vector2(5f);
        }
    }
}
