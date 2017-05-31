using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PlatformerEngine
{
    internal class Player : CharacterBase, IAnimatable
    {
        public Clock AnimationClock { get; set; }
        public Animation CurrentAnimation { get; set; }
        public float AnimationSpeed { get; set; }

        public int Dirrection
        {
            set
            {
                var dir = value >= 0 ? 1 : -1;
                Scale = new Vector2f(dir, 1);
            }
            get
            {
                var dir = Scale.X >= 0 ? 1 : -1;
                return dir;
            }
        }

        private const float PlayerMoveSpeed = 4f;
        private const float PlayerMoveSpeedAcceleration = 0.2f;
        private const float PlayerJumpSpeedAcceleration = 1.8f;
        private const int frameSize = 32;

        private bool isMoveLeft;
        private bool isMoveRight;
        private bool isMoveUp;
        private bool isJumping;
        private bool isFalling;

        public int PlayerPositionX { get; set; }
        public int PlayerPositionY { get; set; }

        private readonly World world;

        private FloatRect playerRect;
        private FloatRect bottomRectangle;

        public Text PlayerPosition { get; set; }

        private bool isWallByLeftSide;
        private bool isWallByRightSide;
        private bool isRoof;

        public Player(World world) : base(Content.Player, frameSize)
        {
            Anim_Right = new Animation(0, 0, 4);
            Anim_Left = new Animation(0, 0, 4);

            AnimationClock = new Clock();

            this.world = world;
        }

        private void SetOutputWithPlayerPisition()
        {
            PlayerPosition = new Text
            {
                Font = Content.BitwiseFont,
                DisplayedString = $"X: {PlayerPositionX} Y: {PlayerPositionY}",
                CharacterSize = 9,
                Color = Color.White,
                Style = Text.Styles.Regular,
                Position = new Vector2f(90,2)
                
            };
        }

        public override void Update()
        {
            PlayerPositionX = (int)Position.X;
            PlayerPositionY = (int)Position.Y;

            SetOutputWithPlayerPisition();

            CurrentAnimation = null;

            var deltaTime = AnimationClock.Restart().AsSeconds();

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

            var isMove = isMoveRight || isMoveLeft || isMoveUp;

            var isJump = isMoveUp && !isJumping;

            if (isJump)
            {
                CurrentAnimation = null;
                isJumping = true;

                if (velocity.Y > -8)
                    velocity.Y += 0.1f;

                if (velocity.Y > 15)
                    velocity.Y = 15;

                movement.Y = -PlayerJumpSpeedAcceleration - PlayerMoveSpeed - velocity.Y*deltaTime;
                velocity.Y = velocity.Y + Gravity*deltaTime;
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

                    if (velocity.Y > 15)
                        velocity.Y = 15;

                    movement.X -= PlayerMoveSpeedAcceleration + PlayerMoveSpeed*deltaTime;
                    Dirrection = -1;
                }

                if (isMoveRight)
                {
                    CurrentAnimation = Anim_Right;

                    if (movement.X < 0)
                        movement.X = 0;

                    if (velocity.Y > -5)
                        velocity.Y += 0.1f;

                    if (velocity.Y > 15)
                        velocity.Y = 15;

                    movement.X += PlayerMoveSpeedAcceleration + PlayerMoveSpeed*deltaTime;
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

            var nextPosition = Position + velocity - CharacterSprite.Origin;
            playerRect = new FloatRect(nextPosition, new Vector2f(32, 32));

            var px = (int) ((Position.X - CharacterSprite.Origin.X + 32/2)/Tile.TileSize);
            var py = (int) ((Position.Y + 32)/Tile.TileSize);
            var bottomTile = world.GetTile(px, py);

            if (bottomTile != null)
            {
                bottomRectangle = new FloatRect(bottomTile.Position, new Vector2f(Tile.TileSize, Tile.TileSize));
                DebugRender.AddRectangle(bottomRectangle, Color.White);
                isFalling = !playerRect.Intersects(bottomRectangle);
            }

            if (!isFalling)
                velocity.Y = 0;

            UpdatePhisicsWall(playerRect, px, py, deltaTime);
        }

        private void UpdatePhisicsWall(FloatRect playerRect, int px, int py, float deltaTime)
        {
            Tile[] walls =
            {
                world.GetTile(px - 1, py - 1),
                world.GetTile(px - 1, py - 2),
                world.GetTile(px - 1, py - 3),
                world.GetTile(px + 1, py - 1),
                world.GetTile(px + 1, py - 2),
                world.GetTile(px + 1, py - 3)
            };

            Tile[] wallsByLeftSide =
            {
                world.GetTile(px - 1, py - 1),
                world.GetTile(px - 1, py - 2),
                world.GetTile(px - 1, py - 3)
            };


            Tile[] wallsByRightSide =
            {
                world.GetTile(px + 1, py - 1),
                world.GetTile(px + 1, py - 2),
                world.GetTile(px + 1, py - 3)
            };

            Tile[] roofTile =
            {
                world.GetTile(px, py - 3)
            };

            foreach (var tile in wallsByLeftSide)
            {
                if (tile != null)
                {
                    isWallByLeftSide = true;
                    break;
                }

                isWallByLeftSide = false;
            }

            foreach (var tile in wallsByRightSide)
            {
                if (tile != null)
                {
                    isWallByRightSide = true;
                    break;
                }

                isWallByRightSide = false;
            }

            foreach (var tile in roofTile)
            {
                if (tile != null)
                {
                    isRoof = true;
                    break;
                }

                isRoof = false;
            }

            foreach (var tile in walls)
            {
                if (tile == null) continue;

                var wallTileRect = new FloatRect(tile.Position, new Vector2f(Tile.TileSize, Tile.TileSize));
                DebugRender.AddRectangle(wallTileRect, Color.Green);

                if (playerRect.Intersects(wallTileRect))
                {
                    var isWallTop = this.playerRect.CheckCollisionSideTop(wallTileRect);
                    var isWallBottom = this.playerRect.CheckCollisionSideBottom(wallTileRect);
                    var isSideCollision = this.playerRect.CheckCollisionSideLeftAndRight(wallTileRect);

                    if (isSideCollision)
                    {
                        if (isWallByRightSide && isWallByLeftSide
                            && isWallBottom && isWallTop
                            && !isJumping && !isFalling
                            && (!isMoveLeft || isMoveLeft) && (!isMoveRight || isMoveRight))
                        {
                            movement.Y = (bottomRectangle.Top - bottomRectangle.Height -
                                          (playerRect.Top + playerRect.Height))*(deltaTime/2)*4;
                            velocity.Y = 0;
                            break;
                        }

                        if (isMoveRight)
                        {
                            if (isFalling && !isJumping && !isWallByRightSide)
                            {
                                movement.Y = velocity.Y*deltaTime;
                                velocity.Y = velocity.Y + Gravity*deltaTime;
                                break;
                            }

                            if (isFalling && !isJumping && isWallByRightSide)
                            {
                                movement.X = 0;
                                movement.Y = velocity.Y*deltaTime;
                                velocity.Y = velocity.Y + Gravity*deltaTime;
                                break;
                            }

                            if (!isFalling && !isJumping && isWallByRightSide)
                            {
                                movement.X = velocity.X*deltaTime;
                                velocity.X = 0;
                                break;
                            }
                        }

                        if (isMoveLeft)
                        {
                            if (isFalling && !isJumping && !isWallByLeftSide)
                            {
                                movement.Y = velocity.Y*deltaTime;
                                velocity.Y = velocity.Y + Gravity*deltaTime;
                                break;
                            }

                            if (isFalling && !isJumping && isWallByLeftSide)
                            {
                                movement.X = 0;
                                movement.Y = velocity.Y*deltaTime;
                                velocity.Y = velocity.Y + Gravity*deltaTime;
                                break;
                            }

                            if (!isFalling && !isJumping && isWallByLeftSide)
                            {
                                movement.X = velocity.X*deltaTime;
                                velocity.X = 0;
                                break;
                            }

                            if (isFalling && isJumping && isWallByLeftSide)
                            {
                                movement.X = 0;
                                velocity.X = 0;
                                break;
                            }
                        }
                    }

                    if (isRoof)
                    {
                        if (isWallBottom && !isFalling)
                        {
                            velocity.Y = 0;
                            break;
                        }

                        if (!isFalling)
                        {
                            velocity.Y = 0;
                            break;
                        }

                        if (isFalling && isJumping)
                        {
                            movement.Y = velocity.Y*deltaTime;
                            velocity.Y = velocity.Y + Gravity*deltaTime;
                            velocity.X = 0;
                            break;
                        }
                    }
                }
            }
        }

        public void PlayAnimation(Clock animationClock, ref IntRect characterSpriteRectangle, Animation currentAnimation,
            float animationSpeed, int frameSize)
        {
            if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
            {
                if (currentAnimation != null)
                {
                    characterSpriteRectangle.Top = currentAnimation.offsetTop;

                    if (characterSpriteRectangle.Left == (currentAnimation.numFrames - 1)*frameSize)
                        characterSpriteRectangle.Left = 0;
                    else
                        characterSpriteRectangle.Left += frameSize;
                }

                animationClock.Restart();
            }
        }
    }
}
