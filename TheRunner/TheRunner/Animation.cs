using System;
using Microsoft.Xna.Framework.Graphics;

namespace TheRunner
{
    // Development Version 1.0.2
    // Define Animation Class
    // Requirement 11.0.0
    // DEVELOPER NOTE: Assumes each frame of the animation is as wide as it ia tall. The number
    //        of frames in the animation is inferred from this.
    class Animation
    {
        // Development Version 1.0.2
        // Declare Animation Class Variables
        // Requirement 11.0.0
        Texture2D texture;
        bool running; // Used to stop/resume animations

        // Development Version 1.0.2
        // All Animation Frames Are Arranged Horizontally
        // Requirement 11.1.0
        public Texture2D Texture
        {
            get { return texture; }
        }
        
        // Development Version 1.0.2
        // Duration of Time to Show Each Frame (Frame Time)
        // Requirement 11.1.3
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        // Development Version 1.0.2
        // Allow Looping of Animation When Desired
        // Requirement 11.4.0
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        // Development Version 1.0.2
        // Get the Number of Frames in the Animation
        // Requirement 11.1.1
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        // Development Version 1.0.2
        // Gets the Width of a Frame in the Animation
        // Requirement 11.1.2
        // DEVELOPER NOTE: Assumes square frames
        public int FrameWidth
        {
            get { return Texture.Height; }
        }

        // Development Version 1.0.2
        // Gets the Height of a Frame in the Animation
        // Requirement 11.1.2
        // DEVELOPER NOTE: Assumes square frames
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        // Development Version 1.0.2
        // Constructs a New Animation
        // Requirement 11.0.0
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.running = true; // Set running as default
        }

        // Development Version 1.0.2
        // Set Animation to Run
        // Requirement 11.5.0
        public void setRunning(bool boolVal)
        {
            this.running = boolVal;
        }

        // Development Version 1.0.2
        // Get Animation Run Status
        // Requirement 11.5.0
        public bool getRunning()
        {
            return running;
        }
    }
}
