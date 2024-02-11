using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WangC_MP1
{
    public class Pipe
    {
        // top and bottom sates of a pipe class
        public const int TOP = 0;
        public const int BOTTOM = 1;

        // Y displacement between the top and bottom pipes
        private const int PIPE_Y_SPC = 210;

        // height of the pipe image
        public const int HEIGHT = 540;
        public const int WIDTH = 104;

        // local sprite batch instance
        private SpriteBatch spriteBatch;

        // boolean on whether if the pipe has been passed
        public bool isPassed;

        // rectangle and location of the top and bottom pipe
        private Rectangle[] pipeRecs = new Rectangle[2];
        private Vector2[] pipeLoc = new Vector2[2];


        /// <summary>
        /// constructor of the pipe class
        /// </summary>
        /// <param name="spriteBatch"> passed global SpriteBatch to store in local SpriteBatch </param>
        public Pipe(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            pipeRecs[TOP] = new Rectangle(0, 0, Assets.topPipeImg.Width, Assets.topPipeImg.Height);
            pipeRecs[BOTTOM] = new Rectangle(0, 0, Assets.bottomPipeImg.Width, Assets.bottomPipeImg.Height);
        }

        /// <summary>
        /// method to set the location of the pipe with respect to the top pipe
        /// </summary>
        /// <param name="loc"> location on where the top pipe is set to </param>
        public void SetLoc(Vector2 loc)
        {
            pipeLoc[TOP] = loc;
            pipeLoc[BOTTOM].X = loc.X;
            pipeLoc[BOTTOM].Y = loc.Y + HEIGHT + PIPE_Y_SPC;

            pipeRecs[TOP].Location = pipeLoc[TOP].ToPoint();
            pipeRecs[BOTTOM].Location = pipeLoc[BOTTOM].ToPoint();
        }

        /// <summary>
        /// method to set the X and Y of the pipe with respect to the top pipe
        /// </summary>
        /// <param name="x"> X value of the location on where the location of the pipe is </param>
        /// <param name="y"> Y value of the location on where the location of the pipe is </param>
        public void SetLoc(float x, float y)
        {
            pipeLoc[TOP].X = x;
            pipeLoc[TOP].Y = y;
            pipeLoc[BOTTOM].X = x;
            pipeLoc[BOTTOM].Y = y + HEIGHT + PIPE_Y_SPC;

            pipeRecs[TOP].X = (int)pipeLoc[TOP].X;
            pipeRecs[TOP].Y = (int)pipeLoc[TOP].Y;
            pipeRecs[BOTTOM].X = (int)pipeLoc[BOTTOM].X;
            pipeRecs[BOTTOM].Y = (int)pipeLoc[BOTTOM].Y;
        }

        /// <summary>
        /// method to get the location of the top pipe
        /// </summary>
        /// <returns> a vector2 of the pipe location </returns>
        public Vector2 GetLoc()
        {
            return pipeLoc[TOP];
        }

        /// <summary>
        /// method to get the right of the pipe rectangles
        /// since the right of the top and bottom pipes are the same, it just returns the top's rectangles right
        /// </summary>
        /// <returns> integer value of the right of the top rectangle </returns>
        public int GetRecRight()
        {
            return pipeRecs[TOP].Right;
        }

        /// <summary>
        ///  method that returns if there is collisions between a pipe and another rectangle
        /// </summary>
        /// <param name="otherRec"> another rectangle </param>
        /// <returns> true if there is collision, false otherwise </returns>
        public bool IsCollision(Rectangle otherRec)
        {
            if (pipeRecs[TOP].Intersects(otherRec) || pipeRecs[BOTTOM].Intersects(otherRec))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// method that draw the top and bottom of the pipe
        /// </summary>
        public void Draw()
        {
            spriteBatch.Draw(Assets.topPipeImg, pipeRecs[TOP], Color.White);
            spriteBatch.Draw(Assets.bottomPipeImg, pipeRecs[BOTTOM], Color.White);
        }


    }
}
