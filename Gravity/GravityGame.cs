using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Gravity.Components;
using Gravity.Components.Rendering;
using Gravity.Systems;
using Gravity.Systems.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace Gravity
{
    class GravityGame : Game
    {
        private const bool USE_DEFAULT_CURSOR = true;
        private const float CAMERA_MAX_ZOOM = 1.5f;
        private const float CAMERA_MIN_ZOOM = 0.005f;
        private const float CAMERA_ZOOM_STEP = .025f;
        private const float CAMERA_REDUCED_ZOOM_STEP = .005f;
        private const float CAMERA_DEFAULT_ZOOM = 1f;
        private const float STAR_MASS = 100f;
        private readonly GraphicsDeviceManager _graphics;
        private RenderingCore _renderingCore;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Texture2D _cursorTexture;
        //private PlanetTextureGenerator _planetTextureGenerator;

        private World _world;

        private ICelestialBodyBuilder _celestialBodyBuilder;

        private FramesPerSecondCounterComponent _fpsComponent;

        //private Sprite _testSprite;
        //private Texture2D _testTexture;

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

            _graphics.PreferredBackBufferWidth = 1500;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.IsFullScreen = false;

            _graphics.ApplyChanges();

            _renderingCore = new RenderingCore(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            InitializeCamera();

            //_planetTextureGenerator = new PlanetTextureGenerator(this);

            base.Initialize();
        }

        private void InitializeCamera()
        {
            _camera = new OrthographicCamera(GraphicsDevice)
            {
                MaximumZoom = CAMERA_MAX_ZOOM,
                MinimumZoom = CAMERA_MIN_ZOOM
            };

            _camera.Zoom = CAMERA_DEFAULT_ZOOM;
            _camera.LookAt(Vector2.Zero);
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content/bin";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Fonts/joystix");

            if (!USE_DEFAULT_CURSOR)
            {
                _cursorTexture = Content.Load<Texture2D>("Cursors/Black");

                MouseCursor cursor = MouseCursor.FromTexture2D(_cursorTexture, 40, 38);
                Mouse.SetCursor(cursor);
            }

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
                //.AddSystem(new TrajectoryRenderingSystemWithRenderTarget(_renderingCore, _camera))
                //.AddSystem(new CelestialsSpriteRenderingSystem(GraphicsDevice, _spriteFont, _camera))
                .AddSystem(new CelestialsPrimitiveRenderingSystem(GraphicsDevice, _spriteFont, _camera))
                .AddSystem(new CelestialListDrawSystem(GraphicsDevice, _spriteFont))
                //.AddSystem(new RenderingSystem(_renderingCore, _camera))
                .Build();

            _celestialBodyBuilder = new CelestialBodyBuilder(_world);

            var starPosition = Vector2.Zero;

            var star = new CelestialBodyBuilder(_world)
                .SetMass(STAR_MASS)
                .SetRadius(100f)
                //.WithTrajectoryHistory()
                .SetPosition(starPosition)
                .SetColor(Color.Orange)
                //.SetVelocity(Vector2.Zero)
                .Build();

            //_testTexture = _planetTextureGenerator.Generate(3, Color.Red);
            //_testSprite = new Sprite(_testTexture);
            //star.Attach<SpriteComponent>(new SpriteComponent(new Sprite(_planetTextureGenerator.Generate(5, Color.Red))));

            var planet1Position = Universe.GetRandomOrbitCoordinate(starPosition, 650f);
            var planet2Position = Universe.GetRandomOrbitCoordinate(starPosition, 1500f);
            var planet3Position = Universe.GetRandomOrbitCoordinate(starPosition, 850f);

            var planet1 = new CelestialBodyBuilder(_world)
                .SetMass(0.1f)
                .SetRadius(10f)
                .WithTrajectoryHistory(300)
                .SetPosition(planet1Position)
                .SetColor(Color.Red)
                .SetVelocity(Universe.CalculateFirstCosmicVelocity(STAR_MASS, starPosition, planet1Position))
                .Build();

            //planet1.Attach(new CameraAnchorComponent());

            new CelestialBodyBuilder(_world)
                .SetMass(0.1f)
                .SetRadius(10f)
                .WithTrajectoryHistory(300)
                .SetPosition(planet3Position)
                .SetColor(Color.DarkSalmon)
                .SetVelocity(Universe.CalculateFirstCosmicVelocity(STAR_MASS, starPosition, planet3Position))
                .Build();

            Vector2 planet4Position = Universe.GetRandomOrbitCoordinate(starPosition, 250f);

            new CelestialBodyBuilder(_world)
                .SetMass(0.1f)
                .SetRadius(10f)
                .WithTrajectoryHistory(300)
                .SetPosition(planet4Position)
                .SetColor(Color.GreenYellow)
                .SetVelocity(Universe.CalculateFirstCosmicVelocity(STAR_MASS, starPosition, planet4Position))
                .Build();

            Vector2 planet5Position = Universe.GetRandomOrbitCoordinate(starPosition, 1000f);

            new CelestialBodyBuilder(_world)
                .SetMass(0.1f)
                .SetRadius(10f)
                .WithTrajectoryHistory(300)
                .SetPosition(planet5Position)
                .SetColor(Color.Chocolate)
                .SetVelocity(Universe.CalculateFirstCosmicVelocity(STAR_MASS, starPosition, planet5Position))
                .Build();

            new CelestialBodyBuilder(_world)
                .SetMass(10f)
                .SetRadius(25f)
                .WithTrajectoryHistory(300)
                .SetPosition(planet2Position)
                .SetColor(Color.Black)
                .SetVelocity(Universe.CalculateFirstCosmicVelocity(STAR_MASS, starPosition, planet2Position))
                .Build();

            //SolarSystemInitialize();
        }

        private void SolarSystemInitialize()
        {
            var starMass = 332837f;
            //var starRadius = 109.245f;
            var starRadius = 500f;

            var planetRadiusMultiplyer = 25f;

            var sun = _celestialBodyBuilder
                .Reset()
                .SetMass(starMass)
                .SetRadius(starRadius)
                .WithTrajectoryHistory()
                .SetPosition(new Vector2(0f, 0f))
                .SetColor(new Color(255, 159, 28))
                .SetVelocity(new Vector2(0f, 0f))
                .Build();

            var mercury = _celestialBodyBuilder
                .Reset()
                .SetMass(0.0553f)
                .SetRadius(0.383f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -2000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 2000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(109, 104, 117))
                .Build();

            var venus = _celestialBodyBuilder
                .Reset()
                .SetMass(0.815f)
                .SetRadius(0.950f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -3000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 3000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(131, 56, 236))
                .Build();

            var earth = _celestialBodyBuilder
                .Reset()
                .SetMass(1f)
                .SetRadius(1f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -4000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 4000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(168, 218, 220))
                .Build();

            var mars = _celestialBodyBuilder
                .Reset()
                .SetMass(0.107f)
                .SetRadius(0.532f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -5000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 5000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(230, 57, 70))
                .Build();

            var jupiter = _celestialBodyBuilder
                .Reset()
                .SetMass(317.83f)
                .SetRadius(10.973f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -6000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 6000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(2, 195, 154))
                .Build();

            var saturn = _celestialBodyBuilder
                .Reset()
                .SetMass(95.159f)
                .SetRadius(9.14f * planetRadiusMultiplyer)
                .SetPosition(new Vector2(0f, -7000f))
                .SetVelocity(new Vector2(Universe.CalculateFirstCosmicVelocity(starMass, 7000f), 0f))
                .WithTrajectoryHistory()
                .SetColor(new Color(240, 243, 189))
                .Build();
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }

            if (InputHandler.IsKeyPressed(Keys.Tab))
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
            var fpsText = _fpsComponent.FramesPerSecond.ToString();

            _graphics.GraphicsDevice.Clear(new Color(69, 123, 157));

            _world.Draw(gameTime);

            _spriteBatch.Begin();

            DrawDebugInfo();
            DrawFpsInfo(fpsText);
            DrawRenderingTime(gameTime);

            //_spriteBatch.Draw(_testSprite, new Transform2(new Vector2(500), 0f, Vector2.One * 10));

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawFpsInfo(string fpsText)
        {
            _spriteBatch.DrawString(_spriteFont, fpsText, GetFpsPosition(fpsText), Color.Black);
        }

        private void DrawRenderingTime(GameTime gameTime)
        {
            var text = $"{gameTime.ElapsedGameTime.TotalMilliseconds} ms";
            var textSize = _spriteFont.MeasureString(text);

            _spriteBatch.DrawString(_spriteFont, text, GetFpsPosition("00").Translate(-20, 20), Color.Black);
        }

        private void DrawDebugInfo()
        {
            string text = $"Camera {{ Zoom:{ MathF.Round(_camera.Zoom, 2)} Position:{ _camera.Center }}}";
            text += $"\nMousePosition {{ { InputHandler.MouseState.Position } }}";
            text += $"\nMouseWorldPosition {{ { _camera.ScreenToWorld(InputHandler.MouseState.Position.ToVector2()) } }}";

            _spriteBatch.DrawString(_spriteFont, text, GetCameraInfoPosition(), Color.Black);
        }

        private Vector2 GetFpsPosition(string fpsText)
        {
            var textSize = _spriteFont.MeasureString(fpsText);
            return new Vector2(_graphics.PreferredBackBufferWidth - textSize.X - 5, 5);
        }

        private Vector2 GetCameraInfoPosition()
        {
            return new Vector2(5f);
        }
    }
}
