using System;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PlatformerEngine
{
    class Player : CharacterBase, IAnimatable
    {

        public Clock AnimationClock { get; set; }
        public Animation CurrentAnimation { get; set; }
        public float AnimationSpeed { get; set; }

        public int Dirrection
        {
            set
            {
                int dir = value >= 0 ? 1 : -1;
                Scale = new Vector2f(dir, 1);
            }
            get
            {
                int dir = Scale.X >= 0 ? 1 : -1;
                return dir;
            }
        }

        private const float PlayerMoveSpeed = 4f;
        private const float PlayerMoveSpeedAcceleration = 0.2f;
        private const float PlayerJumpSpeedAcceleration = 0.5f;
        private const int frameSize = 32;

        private bool isMoveLeft; 
        private bool isMoveRight;
        private bool isMoveUp;
        private bool isJumping;
        private bool isFalling;

        private World world;

        private FloatRect playerRect;
        private FloatRect tileRect;

        public Player(World world) : base(Content.Player, frameSize)
        {
            Anim_Right = new Animation(0, 0, 4);
            Anim_Left = new Animation(0, 0, 4);

            AnimationClock = new Clock();

            this.world = world;
        }

        public override void Update()
        {
            CurrentAnimation = null;

            float deltaTime = AnimationClock.Restart().AsSeconds();

            CharacterSprite.TextureRect = characterSpriteRect;

            UpdateMovement(deltaTime);

            UpdatePhisics(deltaTime);

            //Console.WriteLine($"{velocity.Y}");
            //Console.WriteLine($"{velocity.X}");

            PlayAnimation(AnimationClock, ref characterSpriteRect, CurrentAnimation, AnimationSpeed, frameSize);

            base.Update();
          
            if (Position.Y > Program.Window.Size.Y)
                Spawn();
        }

        private void UpdateMovement(float deltaTime)
        {
            isJumping = false;

            isMoveLeft = Keyboard.IsKeyPressed(Keyboard.Key.A);
            isMoveRight = Keyboard.IsKeyPressed(Keyboard.Key.D);
            isMoveUp = Keyboard.IsKeyPressed(Keyboard.Key.W);

            bool isMove = isMoveRight || isMoveLeft || isMoveUp;

            bool isJump = (isMoveUp && !isJumping);

            if (isJump)
            {
                CurrentAnimation = null;
                isJumping = true;

                if (velocity.Y > -5)
                    velocity.Y += 0.1f;

                movement.Y = -PlayerJumpSpeedAcceleration - PlayerMoveSpeed - velocity.Y * deltaTime;
                velocity.Y = velocity.Y + Gravity * deltaTime;            
            }

            if (isMove)
            {
                if (isMoveLeft)
                {
                    CurrentAnimation = Anim_Left;                    
                   
                    if (movement.X > 0)
                        movement.X = 0;

                    if (velocity.Y > -5)
                        velocity.Y += 0.1f;

                    movement.X -= PlayerMoveSpeedAcceleration + PlayerMoveSpeed * deltaTime;
                    Dirrection = -1;
                }

                if (isMoveRight)
                {
                    CurrentAnimation = Anim_Right;

                    if (movement.X < 0)
                        movement.X = 0;

                    if (velocity.Y > -5)
                        velocity.Y += 0.1f;

                    movement.X += PlayerMoveSpeedAcceleration + PlayerMoveSpeed * deltaTime;
                    Dirrection = 1;
                }

                if (movement.X > PlayerMoveSpeed)
                    movement.X = PlayerMoveSpeed;
                else if (movement.X < -PlayerMoveSpeed)
                    movement.X = -PlayerMoveSpeed;
            }
            else
            {
                movement = new Vector2f();
            }

        }

        private void UpdatePhisics(float deltaTime)
        {
            isFalling = true;

            velocity += new Vector2f(0, 0.15f);

            Vector2f nextPosition = this.Position + velocity - CharacterSprite.Origin;
            playerRect = new FloatRect(nextPosition, new Vector2f(32, 32));

            int px = (int)((this.Position.X - CharacterSprite.Origin.X + 32 / 2) / Tile.TileSize);
            int py = (int)((this.Position.Y + 32) / Tile.TileSize);
            Tile tile = world.GetTile(px, py);

            if (tile != null)
            {
                tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TileSize, Tile.TileSize));

                DebugRender.AddRectangle(tileRect, Color.White);

                isFalling = !playerRect.Intersects(tileRect);
            }

            if (!isFalling)
            {
                velocity.Y = (tileRect.Top - tileRect.Height - (playerRect.Top + playerRect.Height)) * (deltaTime / 2) * 4;
                velocity.Y = 0;
            }

            UpdatePhisicsWall(playerRect, px, py, deltaTime);
        }

        private void UpdatePhisicsWall(FloatRect playerRect, int px, int py, float dt)
        {
            Tile[] walls = new Tile[]
            {
                world.GetTile(px - 1, py - 1),
                world.GetTile(px - 1, py - 2),
                world.GetTile(px - 1, py - 3),
     
                world.GetTile(px + 1, py - 1),
                world.GetTile(px + 1, py - 2),
                world.GetTile(px + 1, py - 3),
            };

            foreach (Tile tile in walls)
            {
                if (tile == null) continue;

                FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TileSize, Tile.TileSize));
                DebugRender.AddRectangle(tileRect, Color.Green);

                if (playerRect.Intersects(tileRect))
                {
                    bool isWallBehind = this.playerRect.CheckCollisionSideRight(this.tileRect);
                    bool isWallTop = this.playerRect.CheckCollisionSideTop(this.tileRect);
                    bool isWallBottom  = this.playerRect.CheckCollisionSideBottom(this.tileRect); 
                    bool isSideCollision = this.playerRect.CheckCollisionSideLeftAndRight(this.tileRect);
                    bool isWallAhead = this.playerRect.CheckCollisionSideLeft(this.playerRect);

                    float speed = Math.Abs(movement.X);

                    if (isSideCollision)
                    {
                           if (isMoveRight)
                           {
                               if (isWallBehind && isFalling && isWallAhead)
                               {
                                   movement.X = 0;
                                   movement.Y = velocity.Y*dt;
                                   velocity.Y = +5 - speed;
                                   break;
                               }

                                movement.X = (playerRect.Left - (tileRect.Left - this.tileRect.Width));
                                velocity.X = 0;
                                break;
                           }

                           if (isMoveLeft)
                           {

                               if (isWallBehind && isFalling && isWallAhead)
                               {
                                   movement.X = 0;
                                   movement.Y = velocity.Y * dt;
                                   velocity.Y = +5 - speed;
                                   break;
                                }
                               
                                if (isWallBehind && !isJumping && !isFalling && !isMoveLeft)
                               {
                                  movement.X = (playerRect.Width - tileRect.Left);
                                  velocity.X = 0;
                                  break;
                               }

                               if (isWallBehind && !isJumping && !isFalling && isMoveLeft)
                               {
                                   movement.X = (playerRect.Left - (tileRect.Left - this.tileRect.Width));
                                   velocity.X = 0;
                                   break;
                               }
                               
                               if (isWallBehind && !isJumping && isFalling && isMoveLeft)
                               {
                                   movement.X = (playerRect.Left - (tileRect.Left - this.tileRect.Width));
                                   velocity.X = 0;
                                   break;
                               }

                            movement.X = ((tileRect.Left + tileRect.Width) - playerRect.Left);
                            velocity.X = 0;                            

                            }
                        }

                    if (isWallTop)
                    {
                        if(isWallBottom && !isFalling)
                        {
                            velocity.Y = 0;
                            break;
                        }

                        if (isFalling && isJumping)
                        {
                            movement.Y = ((tileRect.Height + playerRect.Top) - tileRect.Top);
                            velocity.X = 0;
                            break;
                        }

                        if (!isFalling)
                        {
                            velocity.Y = 0;
                            break;
                        }
                    }
                }
            }
        }

        public void PlayAnimation(Clock animationClock, ref IntRect characterSpriteRectangle, Animation currentAnimation, float animationSpeed, int frameSize)
        {
            if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
            {
                if (currentAnimation != null)
                {
                    characterSpriteRectangle.Top = currentAnimation.offsetTop;

                    if (characterSpriteRectangle.Left == (currentAnimation.numFrames - 1) * frameSize)
                        characterSpriteRectangle.Left = 0;
                    else
                        characterSpriteRectangle.Left += frameSize;
                }

                animationClock.Restart();
            }
        }
    }
}
