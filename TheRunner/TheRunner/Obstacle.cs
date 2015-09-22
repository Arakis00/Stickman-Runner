using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TheRunner
{
    // Development Version 1.0.4
    // Defines Obstacle Class
    // Requirement 10.0.0
    class Obstacle
    {
        // Development Version 1.0.4
        // Declare Obstacle Variables
        // Requirements 10.0.0 and 10.2.0
        private int type, width, height;
        private int xPosition, yPosition;
        private Rectangle frame;
        private bool hit;

        // Development Version 1.0.4
        // Creates Obstacle and Sets Position
        // Requirements 10.0.0 and 10.3.6
        public void createObstacle(int typ)
        {
            type = typ;
            xPosition = 850; // Far right of screen
            hit = false;
            updateFrame();
        }

        // Development Version 1.0.4
        // Get Obstacle X Position
        // Requirement 10.6.0
        public int getXPosition()
        {
            return xPosition;
        }

        // Development Version 1.0.4
        // Set Obstacle X Position
        // Requirement 10.6.0
        public void setXPosition(int x)
        {
            xPosition = x;
        }

        // Development Version 1.0.4
        // Get Obstacle Y Position
        // Requirement 10.6.0
        public int getYPosition()
        {
            return yPosition;
        }

        // Development Version 1.0.4
        // Get Obstacle Y Position
        // Requirement 10.6.0
        public void setYPosition(int y)
        {
            yPosition = y;
        }

        // Development Version 1.0.4
        // Get Obstacle Tyoe
        // Requirement 10.6.0
        public int getType()
        {
            return type;
        }

        // Development Version 1.0.4
        // Get Obstacle Frame
        // Requirement 10.6.0
        public Rectangle getFrame()
        {
            updateFrame();
            return frame;
        }

        // Development Version 1.0.4
        // Get Obstacle Width
        // Requirement 10.6.0
        public int getWidth()
        {
            return width;
        }

        // Development Version 1.0.4
        // Get Obstacle Height
        // Requirement 10.6.0
        public int getHeight()
        {
            return height;
        }

        // Development Version 1.0.4
        // Set Obstacle Hit Status
        // Requirement 10.6.0
        public void setHitStatus(bool htSts)
        {
            hit = htSts;
        }

        // Development Version 1.0.4
        // Get Obstacle Hit Status
        // Requirement 10.6.0
        public bool getHitStatus()
        {
            return hit;
        }

        // Development Version 1.0.4
        // Update Obstacle Frame
        // Requirement 10.6.1
        private void updateFrame()
        {
            // Development Version 1.0.4
            // Type is a Hurdle
            // Requirement 10.2.3
            if (type == 1)
            {
                //yPosition = 340;
                width = 34;
                height = 72;

                // Development Version 1.0.4
                // Set Frame to Allow Runner to Jump Over Hurdle
                // Requirement 1.1.1
                frame = new Rectangle(xPosition + width / 2 - 2, yPosition + height / 4 - 4, 4, height / 2);
            }
            // Development Version 1.0.4
            // Type is a Finish Line
            // Requirement 10.2.4
            else if (type == 2)
            {
                width = 65;
                height = 107;

                frame = new Rectangle(xPosition + width / 2 - 2, yPosition - 100, 4, height + 100);
            }
            // Development Version 1.0.4
            // Type is an Overhead Bar
            // Requirement 10.2.2
            else
            {
                width = 33;
                height = 104;

                // Development Version 1.0.4
                // Set Frame to Allow Runner to Duck Under Overhead Bar
                // Requirement 1.1.2
                frame = new Rectangle(xPosition + width / 2 - 2, yPosition + height / 4 - 6, 4, height / 4);
            }
        }

    }
}
