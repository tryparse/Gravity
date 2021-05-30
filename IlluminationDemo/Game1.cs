using System;
using System.Collections.Generic;
using System.Linq;
using Illumination;
using Illumination.Utility;
using Illumination.Visibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace IlluminationDemo
{
    public class Game1 : Game
    {
        private const float DeltaZoom = .05f;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private InputHandlerComponent _inputHandlerComponent;

        private RenderTarget2D DefaultRenderTarget;
        private IlluminationEngine _illuminationEngine;

        private VisibilitySegment[] _visibilitySegments;
        private RectangleF[] _obstacles;
        private Random _random;

        private List<PointLight> _lights;
        private bool _isFixedRedLight = false;

        private Camera2D _camera;
        private Texture2D _background;
        private SpriteFont _spriteFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1000,
                PreferredBackBufferHeight = 1000
            };

            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            DefaultRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            _camera = new Camera2D(GraphicsDevice);

            _illuminationEngine = new IlluminationEngine(this, _camera, true);

            _lights = new List<PointLight>
            {
                new PointLight
                {
                    Radius = 200f,
                    Color = Color.Red
                }
                //new PointLight
                //{
                //    WorldPosition = new Vector2(550, 250),
                //    Radius = 200f,
                //    Color = Color.Blue
                //},
                //new PointLight
                //{
                //    WorldPosition = new Vector2(350, 500),
                //    Radius = 300f,
                //    Color = Color.Green
                //},
                //new PointLight
                //{
                //    WorldPosition = new Vector2(850, 600),
                //    Radius = 350f,
                //    Color = Color.White
                //}
            };

            _obstacles = new RectangleF[1];
            _random = new Random();

            _visibilitySegments = new VisibilitySegment[4 * _obstacles.Length];

            for (int i = 0; i < _obstacles.Length; i++)
            {
                var _rectangle = new RectangleF(_random.Next(10, 990), _random.Next(10, 990), _random.Next(100, 200), _random.Next(100, 200));
                _obstacles[i] = _rectangle;

                var corners = _rectangle.GetCorners();

                _visibilitySegments[i * 4 + 0] = new VisibilitySegment(corners[0], corners[1]);
                _visibilitySegments[i * 4 + 1] = new VisibilitySegment(corners[1], corners[2]);
                _visibilitySegments[i * 4 + 2] = new VisibilitySegment(corners[2], corners[3]);
                _visibilitySegments[i * 4 + 3] = new VisibilitySegment(corners[3], corners[0]);
            }

            _inputHandlerComponent = new InputHandlerComponent(this);
            Components.Add(_inputHandlerComponent);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _background = Content.Load<Texture2D>("Pixel_grey");
            _spriteFont = Content.Load<SpriteFont>("joystix");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateCamera(gameTime);

            if (!_isFixedRedLight)
            {
                _lights.First().WorldPosition = _camera.ScreenToWorld(_inputHandlerComponent.MouseState.Position.ToVector2());
            }
            
            _illuminationEngine.Update(
                _lights,
                _visibilitySegments.ToList());

            base.Update(gameTime);
        }

        void UpdateCamera(GameTime gameTime)
        {
            if (_inputHandlerComponent.IsWheelScrolledDown)
            {
                _camera.ZoomIn(DeltaZoom);
            }
            else if (_inputHandlerComponent.IsWheelScrolledUp)
            {
                _camera.ZoomOut(DeltaZoom);
            }

            if (_inputHandlerComponent.IsKeyDown(Keys.Up))
            {
                _camera.Move(new Vector2(0, -3) / _camera.Zoom);
            }
            else if (_inputHandlerComponent.IsKeyDown(Keys.Down))
            {
                _camera.Move(new Vector2(0, 3) / _camera.Zoom);
            }

            if (_inputHandlerComponent.IsKeyDown(Keys.Left))
            {
                _camera.Move(new Vector2(-3, 0) / _camera.Zoom);
            }
            else if (_inputHandlerComponent.IsKeyDown(Keys.Right))
            {
                _camera.Move(new Vector2(3, 0) / _camera.Zoom);
            }

            if (_inputHandlerComponent.IsMouseLeftKeyPressed)
            {
                _isFixedRedLight = !_isFixedRedLight;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(DefaultRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            _spriteBatch.End();

            var lightMapRenderTarget = _illuminationEngine.DrawLightMap();
            var finalWithAmbientColorAndLight = _illuminationEngine.Combine(DefaultRenderTarget, new Color(new Vector4(.05f, .05f, .05f, 1f)));

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            _spriteBatch.Draw(finalWithAmbientColorAndLight, Vector2.Zero, Color.White);

            DrawLightMap(lightMapRenderTarget);
            DrawObstacles();
            DrawDebugInfo();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebugInfo()
        {
            _spriteBatch.DrawString(_spriteFont, $"CameraPosition: {_camera.Position}", Vector2.Zero, Color.Red);
            _spriteBatch.DrawString(_spriteFont, $"CameraZoom: {_camera.Zoom}", Vector2.Zero + new Vector2(0, 10), Color.Red);
            _spriteBatch.DrawString(_spriteFont, $"MousePosition: {_inputHandlerComponent.MouseState.Position}", Vector2.Zero + new Vector2(0, 20), Color.Red);
        }

        private void DrawLightMap(RenderTarget2D lightMapRenderTarget)
        {
            _spriteBatch.Draw(lightMapRenderTarget, new Rectangle(GraphicsDevice.Viewport.Width - 400, 0, 400, 400), Color.White);
        }

        private void DrawObstacles()
        {
            foreach (var obstacle in _obstacles)
            {
                _spriteBatch.DrawRectangle(_camera.WorldToScreen(obstacle), Color.White, 2f);
            }
        }
    }
}
