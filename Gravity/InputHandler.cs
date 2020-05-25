using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class InputHandler : GameComponent
    {
        private static KeyboardState _keyboardState;
        private static KeyboardState _previousKeyboardState;
        private static MouseState _mouseState;
        private static MouseState _previousMouseState;

        public InputHandler(Game game) : base(game)
        {

        }

        public static KeyboardState KeyboardState => _keyboardState;

        public static MouseState MouseState => _mouseState;

        public override void Initialize()
        {
            base.Initialize();

            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        public static void Flush()
        {
            _previousKeyboardState = _keyboardState;
        }

        public static bool IsKeyReleased(Keys key)
        {
            return _keyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        public static bool IsWheelScrolledDown => _mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue;

        public static bool IsWheelScrolledUp => _mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue;
    }
}
