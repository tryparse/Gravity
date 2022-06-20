using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Illumination.Utility;
using Illumination.Utility.Extensions;
using System.Collections.Generic;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using System.Linq;

namespace Illumination
{
    class LightMapRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly PolygonBatch _polygonBatch;
        private readonly Camera2D _camera;
        private readonly RenderTargetFactory _renderTargetFactory;
        private readonly RenderTarget2D _renderTarget;
        private Effect _lightMapEffect;

        public LightMapRenderer(GraphicsDevice graphicsDevice, Camera2D camera, RenderTargetFactory renderTargetFactory, Effect lightMapEffect)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _renderTargetFactory = renderTargetFactory ?? throw new ArgumentNullException(nameof(renderTargetFactory));
            _lightMapEffect = lightMapEffect ?? throw new ArgumentNullException(nameof(lightMapEffect));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _polygonBatch = new PolygonBatch(_graphicsDevice, camera);
            _renderTarget = _renderTargetFactory.CreateRenderTarget();
        }

        public RenderTarget2D Draw(PointLight light, RenderTarget2D shadowMapRenderTarget, HashSet<Polygon> lightPolygons, bool isDebug)
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            if (isDebug)
            {
                if (lightPolygons.Any())
                {
                    var segmentsData = new List<VertexPositionColor>();

                    foreach (var polygon in lightPolygons)
                    {
                        var p1 = polygon.Vertices[0];
                        var p2 = polygon.Vertices[1];
                        var p3 = polygon.Vertices[2];

                        segmentsData.Add(new VertexPositionColor(new Vector3(p1.X, p1.Y, 0), light.Color));
                        segmentsData.Add(new VertexPositionColor(new Vector3(p2.X, p2.Y, 0), light.Color));
                        segmentsData.Add(new VertexPositionColor(new Vector3(p1.X, p1.Y, 0), light.Color));
                        segmentsData.Add(new VertexPositionColor(new Vector3(p3.X, p3.Y, 0), light.Color));
                        segmentsData.Add(new VertexPositionColor(new Vector3(p2.X, p2.Y, 0), light.Color));
                        segmentsData.Add(new VertexPositionColor(new Vector3(p3.X, p3.Y, 0), light.Color));
                    }

                    _polygonBatch.DrawPolygons(segmentsData.ToArray());
                }
            }
            else
            {
                _lightMapEffect.TrySetParameter("LightColor", light.Color.ToVector4());
                _lightMapEffect.TrySetParameter("LightWorldPosition", light.WorldPosition);
                _lightMapEffect.TrySetParameter("LightRadius", light.Radius);
                _lightMapEffect.TrySetParameter("World", _camera.GetWorldMatrix());
                _lightMapEffect.TrySetParameter("View", _camera.GetViewMatrix());
                _lightMapEffect.TrySetParameter("Projection", _camera.GetProjectionMatrix());

                _spriteBatch.Begin(effect: _lightMapEffect);
                _spriteBatch.Draw(shadowMapRenderTarget, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }

            return _renderTarget;
        }
    }
}
