using System;
using Microsoft.Xna.Framework.Graphics;

namespace Illumination.Utility
{
    class PolygonBatch
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect _effect;
        private readonly Camera2D _camera;

        public PolygonBatch(GraphicsDevice graphicsDevice, Camera2D camera)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            _effect = new BasicEffect(_graphicsDevice)
            {
                VertexColorEnabled = true
            };

            _camera = camera;

            UpdateViewMatrix();
        }

        public void DrawColoredPolygons(VertexPositionColor[] vertices)
        {
            if (vertices.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(vertices)}.Length == 0");
            }

            UpdateViewMatrix();

            SetVertexBuffer(vertices);
            
            DrawUserPrimitives(vertices, PrimitiveType.TriangleList, vertices.Length / 3);
        }

        private void DrawUserPrimitives(VertexPositionColor[] vertices, PrimitiveType primitiveType, int primitiveCount)
        {
            foreach (var effectPass in _effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                _graphicsDevice.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0, primitiveCount);
            }
        }

        private void SetVertexBuffer(VertexPositionColor[] vertices)
        {
            using (var vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.None))
            {
                vertexBuffer.SetData(vertices);
                _graphicsDevice.SetVertexBuffer(vertexBuffer);
            }
        }

        public void DrawPolygons(VertexPositionColor[] vertices)
        {
            if (vertices.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(vertices)}.Length == 0");
            }

            UpdateViewMatrix();

            SetVertexBuffer(vertices);

            DrawUserPrimitives(vertices, PrimitiveType.LineList, vertices.Length / 2);
        }

        private void UpdateViewMatrix()
        {
            _effect.Projection = _camera.GetProjectionMatrix();
            _effect.World = _camera.GetWorldMatrix();
            _effect.View = _camera.GetViewMatrix();
        }
    }
}
