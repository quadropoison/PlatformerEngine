using SFML.Graphics;
using SFML.System;

namespace PlatformerEngine
{
    abstract class CharacterBase : Transformable, Drawable
    {
        public Vector2f StartPosition;

        private Clock deltaTime;
        private int frameSize;

        protected Sprite CharacterSprite { get; set; }
        protected IntRect characterSpriteRect;
        protected Texture texture;

        protected Vector2f velocity;
        protected Vector2f movement;

        protected Animation Anim_Up;
        protected Animation Anim_Left;
        protected Animation Anim_Down;
        protected Animation Anim_Right;

        protected const float Gravity = 9.8f;

        public CharacterBase(Texture texture, int frameSize)
        {
            this.frameSize = frameSize;
            this.texture = texture;
            characterSpriteRect = new IntRect(0, 0, 32, 32);
            CharacterSprite = new Sprite(texture, characterSpriteRect);
            CharacterSprite.Origin = new Vector2f(32 / 2, 0);
        }


        public virtual void Update()
        {         
            this.Position += movement + velocity;
        }

        public void Spawn()
        {
            Position = StartPosition;
            velocity = new Vector2f();
            movement = new Vector2f();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(CharacterSprite,states);
        }
    }   
}
