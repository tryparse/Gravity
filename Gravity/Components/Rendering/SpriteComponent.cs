using MonoGame.Extended.Sprites;

namespace Gravity.Components.Rendering
{
    class SpriteComponent
    {
        public Sprite Sprite { get; private set; }

        public SpriteComponent(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}
