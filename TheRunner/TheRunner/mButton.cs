using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheRunner
{
    // Development Version 2.0.3
    // Define Button Class
    // Requirement 13.0.0
    class mButton
    {
        // Development Version 2.0.3
        // Declare Button Graphic, Size and Position Variables
        // Requirements 13.3.0 and 13.3.1
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        public Vector2 size;

        // Development Version 2.0.3
        // Declare Fade, Clicked and Mouse State Variables
        // Requirements 13.4.0 and 13.5.0
        bool down; //determines whether button alpha is increasing or decreasing
        public bool isClicked;
        MouseState lastMouseState, currentMouseState; //to store last and current mouse state to detect clicks

        // Development Version 2.0.3
        // Color to Cycle on the Button When Hovering/Selected
        // Requirement 13.4.0
        Color color = new Color(255, 255, 255, 255);

        // Development Version 2.0.3
        // Constructor for Button Objects Class
        // Requirement 13.4.0
        public mButton(Texture2D newTexture, GraphicsDevice graphics)
        {
            texture = newTexture;

            // Development Version 2.0.3
            // Temporarily Assign Button Size Based on Viewport Size
            // Requirement 13.3.1
            size = new Vector2(graphics.Viewport.Width / 8, graphics.Viewport.Height / 30);
        }

        // Development Version 2.0.3
        // Update Game Buttons
        // Requirement 13.2.0
        public void Update(MouseState mouse)
        {
            // Development Version 2.0.3
            // Clear Old Saved Clicks and Manage Mouse-Button Interaction
            // Requirement 13.5.0
            isClicked = false;
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState(); // Get the current mouse state
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            // Development Version 2.0.3
            // Changes Button Alpha Value to Fade-Out if Currently Focused
            // Requirements 5.1.1 and 13.4.0
            if (mouseRectangle.Intersects(rectangle))
            {
                if (color.A >= 255)
                    down = false;
                if (color.A <= 0)
                    down = true;
                if (down)
                    color.A += 3;
                else
                    color.A -= 3;

                // Development Version 2.0.3
                // Select Button if Player Left-Clicks the Mouse
                // Requirement 5.1.2
                if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    isClicked = true; 
            }
            // Development Version 2.0.3
            // Changes Button Alpha Value to Fade-In if Focus Has Left
            // Requirement 13.4.0
            else if (color.A < 255)
            {
                color.A += 3;
                isClicked = false;
            }
        }

        // Development Version 2.0.3
        // Sets Button Position
        // Requirement 13.3.2
        public void setPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        // Development Version 2.0.3
        // Sets Button Size
        // Requirement 13.3.2
        public void setSize(Vector2 sz)
        {
            size = sz;
        }

        // Development Version 2.0.3
        // Sets Button Image
        // Requirement 13.3.2
        public void changeImage(Texture2D image)
        {
            texture = image;
        }

        // Development Version 2.0.3
        // Draws Button
        // Requirement 13.3.3
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }
    }
}
