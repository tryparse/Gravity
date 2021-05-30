using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Illumination.Utility
{
    public class Camera2D : Camera<Vector2>
    {
        private readonly OrthographicCamera _orthographicCamera;
        private readonly ViewportAdapter _viewportAdapter;
        private readonly BasicEffect _basicEffect;

        public override Vector2 Position
        {
            get => _orthographicCamera.Position;
            set => _orthographicCamera.Position = value;
        }
        public override float Rotation
        {
            get => _orthographicCamera.Rotation;
            set => _orthographicCamera.Rotation = value;
        }
        public override float Zoom 
        { 
            get => _orthographicCamera.Zoom;
            set => _orthographicCamera.Zoom = value;
        }
        public override float MinimumZoom 
        { 
            get => _orthographicCamera.MinimumZoom; 
            set => _orthographicCamera.MinimumZoom = value; 
        }
        public override float MaximumZoom 
        { 
            get => _orthographicCamera.MaximumZoom; 
            set => _orthographicCamera.MaximumZoom = value;
        }

        public override RectangleF BoundingRectangle => _orthographicCamera.BoundingRectangle;

        public override Vector2 Origin 
        { 
            get => _orthographicCamera.Origin; 
            set => _orthographicCamera.Origin = value;
        }

        public override Vector2 Center => _orthographicCamera.Center;

        public Camera2D(GraphicsDevice graphicsDevice)
        {
            _orthographicCamera = new OrthographicCamera(graphicsDevice);
            _viewportAdapter = new DefaultViewportAdapter(graphicsDevice);
            _basicEffect = new BasicEffect(graphicsDevice);
        }

        public override ContainmentType Contains(Vector2 vector2)
        {
            return _orthographicCamera.Contains(vector2);
        }

        public override ContainmentType Contains(Rectangle rectangle)
        {
            return _orthographicCamera.Contains(rectangle);
        }

        public override BoundingFrustum GetBoundingFrustum()
        {
            return _orthographicCamera.GetBoundingFrustum();
        }

        public override Matrix GetInverseViewMatrix()
        {
            return _orthographicCamera.GetInverseViewMatrix();
        }

        public override Matrix GetViewMatrix()
        {
            return _orthographicCamera.GetViewMatrix();
        }

        public Matrix GetProjectionMatrix()
        {
            var viewMatrix = GetViewMatrix();
            var projection = Matrix.CreateOrthographicOffCenter(0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        public Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        public Matrix GetWorldMatrix()
        {
            var calculatedWorldMatrix = _orthographicCamera.GetInverseViewMatrix();

            return calculatedWorldMatrix;
        }

        public override void LookAt(Vector2 position)
        {
            _orthographicCamera.LookAt(position);
        }

        public override void Move(Vector2 direction)
        {
            _orthographicCamera.Move(direction);
        }

        public override void Rotate(float deltaRadians)
        {
            _orthographicCamera.Rotate(deltaRadians);
        }

        public override Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return _orthographicCamera.ScreenToWorld(screenPosition);
        }

        public override Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return _orthographicCamera.WorldToScreen(worldPosition);
        }

        public RectangleF WorldToScreen(RectangleF rectangle)
        {
            return new RectangleF
            {
                Position = WorldToScreen(rectangle.Position),
                Size = rectangle.Size * Zoom
            };
        }

        public override void ZoomIn(float deltaZoom)
        {
            _orthographicCamera.ZoomIn(deltaZoom);
        }

        public override void ZoomOut(float deltaZoom)
        {
            _orthographicCamera.ZoomOut(deltaZoom);
        }
    }
}
