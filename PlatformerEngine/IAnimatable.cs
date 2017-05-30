using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerEngine
{
    interface IAnimatable
    {
        Clock AnimationClock { get; set; }

        Animation CurrentAnimation { get; set; }

        float AnimationSpeed { get; set; }

        void PlayAnimation(Clock animationClock, ref IntRect characterSpriteRectangle, Animation currentAnimation, float animationSpeed, int frameSize);
    }
}
