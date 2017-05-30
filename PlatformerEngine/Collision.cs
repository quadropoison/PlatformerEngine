using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace PlatformerEngine
{
    public static class Collision
    {
        private static float playerLeft;
        private static float playerRight;
        private static float playerTop;
        private static float playerBottom;

        private static float wallLeft;
        private static float wallRight;
        private static float wallTop;
        private static float wallBottom;

        public static bool CheckCollision(this FloatRect player, FloatRect otherObject)
        {
            playerLeft = player.Left;
            playerRight = playerLeft + player.Width;
            playerTop = player.Top;
            playerBottom = playerTop + player.Height;

            wallLeft = otherObject.Left;
            wallRight = wallLeft + otherObject.Width;
            wallTop = otherObject.Top;
            wallBottom = wallTop + otherObject.Height;

            if (playerRight < wallLeft || playerLeft > wallRight || playerTop > wallBottom || playerBottom < wallTop)
            {
                return false;
            }

            return true;
        }

        public static bool CheckCollisionSideRight(this FloatRect player, FloatRect otherObject)
        {
            playerLeft = player.Left;
            playerRight = playerLeft + player.Width;

            wallLeft = otherObject.Left;

            if (playerRight < wallLeft)
            {
                return false;
            }

            return true;
        }

        public static bool CheckCollisionSideLeft(this FloatRect player, FloatRect otherObject)
        {
            playerLeft = player.Left;

            wallLeft = otherObject.Left;
            wallRight = wallLeft + otherObject.Width;

            if (playerLeft > wallRight)
            {
                return false;
            }

            return true;
        }

        public static bool CheckCollisionSideTop(this FloatRect player, FloatRect otherObject)
        {
            playerTop = player.Top;

            wallTop = otherObject.Top;
            wallBottom = wallTop + otherObject.Height;

            if (playerTop > wallBottom)
            {
                return false;
            }

            return true;
        }

        public static bool CheckCollisionSideBottom(this FloatRect player, FloatRect otherObject)
        {
            playerTop = player.Top;
            playerBottom = playerTop + player.Height;

            wallTop = otherObject.Top;

            if (playerBottom < wallTop)
            {
                return false;
            }

            return true;
        }

        public static bool CheckCollisionSideLeftAndRight(this FloatRect player, FloatRect otherObject)
        {
            playerLeft = player.Left;
            playerRight = playerLeft + player.Width;

            wallLeft = otherObject.Left;
            wallRight = wallLeft + otherObject.Width;

            if (playerRight < wallLeft || playerLeft > wallRight)
            {
                return false;
            }

            return true;
        }
    }
}
