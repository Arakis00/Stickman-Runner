using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheRunner
{
    // Development Version 1.0.2
    // Controls Playback of Animation
    // Requirement 11.2.0
    struct AnimationPlayer
    {
        // Development Version 1.0.2
        // Gets the Animation Which is Currently Playing
        // Requirement 11.2.0
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        // Development Version 1.0.2
        // Gets the Index of the Current Frame of the Animation
        // Requirement 11.1.1
        int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
        }

        // Development Version 1.0.2
        // The Amount of Time in Seconds That the Current Frame Has Been Shown
        // Requirement 11.1.3
        private float time;

        // Development Version 1.0.2
        // Gets the Texture Origin at the Bottom Center of the Frame
        // Requirement 11.1.0
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        // Development Version 1.0.2
        // Begins or Continues Playback of an Animation
        // Requirement 11.2.0
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it
            if (Animation == animation)
                return;

            // Start the new animation
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
        }

        // Development Version 1.0.2
        // Advances the Time Position and Draws the Current Frame of the Animation
        // Requirements 11.1.3 and 11.1.7
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            if (!Animation.getRunning()) // Animation not running
            {
                // Do nothing since animation isn't running
            }
            else // Animation running
            {
                // Development Version 1.0.2
                // Process Passing Time According to Frame Time
                // Requirement 11.1.3
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    // Development Version 1.0.2
                    // Advance the Frame Index; Looping or Clamping as Appropriate
                    // Requirement 11.1.4
                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + 1) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                    }
                }
            }

            // Development Version 1.0.2
            // Calculate the Source Rectangle of the Current Frame
            // Requirement 11.1.2
            Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
