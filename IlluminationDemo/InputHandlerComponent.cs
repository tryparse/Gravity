using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IlluminationDemo
{
    interface IInputHandler
    {
        KeyboardState KeyboardState { get; }

        MouseState MouseState { get; }

        bool IsKeyReleased(Keys key);

        bool IsKeyPressed(Keys key);

        bool IsKeyDown(Keys key);

        bool IsWheelScrolledDown { get; }

        bool IsWheelScrolledUp { get; }

        bool IsMouseLeftKeyPressed { get; }
    }

    public class InputHandlerComponent : GameComponent, IInputHandler
    {
        private KeyboardState _keyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _mouseState;
        private MouseState _previousMouseState;

        public InputHandlerComponent(Game game) : base(game)
        {
        }

        public KeyboardState KeyboardState => _keyboardState;

        public MouseState MouseState => _mouseState;

        public override void Initialize()
        {
            base.Initialize();

            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
            {
                return;
            }

            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        public void Flush()
        {
            _previousKeyboardState = _keyboardState;
        }

        public bool IsKeyReleased(Keys key)
        {
            return Game.IsActive
                && _keyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return Game.IsActive
                && _keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return Game.IsActive
                && _keyboardState.IsKeyDown(key);
        }

        public bool IsWheelScrolledDown =>
            Game.IsActive
            && _mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue;

        public bool IsWheelScrolledUp =>
            Game.IsActive
            && _mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue;

        public bool IsMouseLeftKeyPressed =>
            Game.IsActive
            && _mouseState.LeftButton == ButtonState.Pressed
            && _previousMouseState.LeftButton == ButtonState.Released;
    }
}
