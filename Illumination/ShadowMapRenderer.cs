using System;
using System.Collections.Generic;
using System.Linq;
using Illumination.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Illumination
{
    class ShadowMapRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Camera2D _camera;
        private readonly RenderTargetFactory _renderTargetFactory;
        private readonly RenderTarget2D _renderTarget;
        private readonly PolygonBatch _polygonBatch;

        public ShadowMapRenderer(GraphicsDevice graphicsDevice, Camera2D camera, RenderTargetFactory renderTargetFactory)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _renderTargetFactory = renderTargetFactory ?? throw new ArgumentNullException(nameof(renderTargetFactory));

            _renderTarget = _renderTargetFactory.CreateRenderTarget();
            _polygonBatch = new PolygonBatch(_graphicsDevice, _camera);
        }

        public RenderTarget2D Draw(HashSet<Polygon> shadowPolygons)
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            var polygonsVertexData = new List<VertexPositionColor>();

            var vertexColor = Color.White;

            if (shadowPolygons.Any())
            {
                foreach (var polygon in shadowPolygons)
                {
                    var vertices = polygon.Vertices;
                    polygonsVertexData.Add(new VertexPositionColor(new Vector3(vertices[0], 0), vertexColor));
                    polygonsVertexData.Add(new VertexPositionColor(new Vector3(vertices[1], 0), vertexColor));
                    polygonsVertexData.Add(new VertexPositionColor(new Vector3(vertices[2], 0), vertexColor));
                }

                _polygonBatch.DrawColoredPolygons(polygonsVertexData.ToArray());
            }

            return _renderTarget;
        }
    }
}
