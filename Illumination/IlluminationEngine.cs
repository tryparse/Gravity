using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using Illumination.Utility;
using Illumination.Utility.Extensions;
using Illumination.Visibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Illumination
{
    public class IlluminationEngine
    {
        private const string _baseName = "Illumination.Properties.resources";

        private readonly Game _game;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Camera2D _camera;

        private SpriteBatch _spriteBatch;
        private ShadowMapRenderer _shadowMapRenderer;
        private LightMapRenderer _lightMapRenderer;

        private Effect _lightMapEffect;
        private Effect _combineEffect;
        private SpriteFont _debugFont;

        private RenderTargetFactory _renderTargetFactory;
        private RenderTarget2D _lightMapRenderTarget;
        private RenderTarget2D _finalRenderTarget;

        private VisibilityCalculator _visibilityCalculator;
        private List<PointLight> _pointLights;
        private Dictionary<PointLight, HashSet<Polygon>> _lightTriangles;

        public bool IsDebugMode { get; set; }

        public IlluminationEngine(Game game, Camera2D camera, bool isDebugMode = false)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _graphicsDevice = _game.GraphicsDevice;
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));

            IsDebugMode = isDebugMode;

            Initialize();
        }

        private void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            
            LoadContent();

            _renderTargetFactory = new RenderTargetFactory(_graphicsDevice);

            _pointLights = new List<PointLight>();

            _shadowMapRenderer = new ShadowMapRenderer(_graphicsDevice, _camera, _renderTargetFactory);
            _lightMapRenderer = new LightMapRenderer(_graphicsDevice, _camera, _renderTargetFactory, _lightMapEffect);

            // this renderTarget should be not cleared on graphicDevice.SetRenderTarget and should be cleared manually every frame
            _lightMapRenderTarget = _renderTargetFactory.CreateRenderTarget(RenderTargetUsage.PreserveContents);
            _finalRenderTarget = _renderTargetFactory.CreateRenderTarget();

            _visibilityCalculator = new VisibilityCalculator();
        }

        private void LoadContent()
        {
            var contentManager = new ResourceContentManager(_game.Services, new ResourceManager(_baseName, Assembly.GetExecutingAssembly()));
            _debugFont = contentManager.Load<SpriteFont>("joystix");
            _lightMapEffect = contentManager.Load<Effect>("IlluminationLightMap");
            _combineEffect = contentManager.Load<Effect>("IlluminationCombine");
        }

        public void Update(List<PointLight> pointLights, List<VisibilitySegment> obstacles)
        {
            _pointLights = pointLights ?? throw new ArgumentNullException(nameof(pointLights));
            _lightTriangles = new Dictionary<PointLight, HashSet<Polygon>>();

            foreach (var light in _pointLights)
            {
                _lightTriangles[light] = _visibilityCalculator.Calculate(light.WorldPosition, light.Radius, obstacles);
            }
        }

        public RenderTarget2D DrawLightMap()
        {
            _graphicsDevice.SetRenderTarget(_lightMapRenderTarget);
            _graphicsDevice.Clear(Color.Black);

            foreach (var light in _pointLights)
            {
                var shadowMapRenderTarget = _shadowMapRenderer.Draw(_lightTriangles[light]);
                var lightMapRenderTarget = _lightMapRenderer.Draw(light, shadowMapRenderTarget, _lightTriangles[light], IsDebugMode);

                _graphicsDevice.SetRenderTarget(_lightMapRenderTarget);
                _spriteBatch.Begin(blendState: BlendState.Additive);
                _spriteBatch.Draw(lightMapRenderTarget, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }

            return _lightMapRenderTarget;
        }

        public RenderTarget2D Combine(RenderTarget2D colorRenderTarget, Color ambientColor)
        {
            _graphicsDevice.SetRenderTarget(_finalRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _combineEffect.TrySetParameter("LightMapTexture", _lightMapRenderTarget);
            _combineEffect.TrySetParameter("AmbientColor", ambientColor.ToVector4());

            _spriteBatch.Begin(effect: _combineEffect, blendState: BlendState.Additive);
            _spriteBatch.Draw(colorRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            return _finalRenderTarget;
        }
    }
}
