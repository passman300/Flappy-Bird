using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WangC_MP1
{
    public class PipeManager
    {
        // number of pipes in the pipe list
        private const int PIPE_CNT = 3;

        // X space between the bottom and top of a pipe
        private const int PIPE_X_SPC = 200;

        // max and min (factor) of the pipe's Y location
        private const int PIPE_MAX_Y = 0;
        private const int PIPE_MIN_Y_FACTR = 88;
        private const int PIPE_RST_X = 600;

        // the vertical displacement for random generation
        private const int PIPE_VER_DIS = 400;

        // random instance
        private Random rng = new Random();

        // list of pipes
        public Pipe[] pipes = new Pipe[PIPE_CNT];

        // temporary storage of the y location of a pipe during generation
        private int randLocY;


        /// <summary>
        /// constructor of the pipe manager class
        /// </summary>
        /// <param name="spriteBatch"> passes global SpriteBatch </param>
        public PipeManager(SpriteBatch spriteBatch)
        {
            InitlizePipes(spriteBatch);

            randLocY = rng.Next(-1 * Pipe.HEIGHT + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);

            RstPipes();
        }

        /// <summary>
        /// method to create pipes into the pipe List
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void InitlizePipes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < PIPE_CNT; i++)
            {
                pipes[i] = new Pipe(spriteBatch);
            }
        }

        /// <summary>
        /// method to update the pipes location and generation
        /// </summary>
        public void UpdatePipes()
        {
            for (int i = 0; i < PIPE_CNT; i++)
            {
                pipes[i].SetLoc(pipes[i].GetLoc().X - Game1.SCRL_SPD, pipes[i].GetLoc().Y);

                // check if the pipe is out of bounds
                if (pipes[i].GetRecRight() <= 0)
                {
                    GeneratePipe(i);
                }
            }
        }

        /// <summary>
        /// method to rest the pipes location and passed attributive
        /// </summary>
        public void RstPipes()
        {
            randLocY = rng.Next(-1 * Pipe.HEIGHT + PIPE_MIN_Y_FACTR, 0);

            // first pipe X is 600;
            pipes[0].SetLoc(PIPE_RST_X, randLocY);

            for (int i = 1; i < PIPE_CNT; i++)
            {
                randLocY = (int)MathHelper.Clamp(rng.Next(-1 * PIPE_VER_DIS, PIPE_VER_DIS) + pipes[i - 1].GetLoc().Y, -1 * Pipe.HEIGHT + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);

                pipes[i].SetLoc(pipes[i - 1].GetLoc().X + Pipe.WIDTH + PIPE_X_SPC, randLocY);

                pipes[i].isPassed = false;
            }
        }

        /// <summary>
        /// method that checks the collision between the pipe's rectangles and another rectangle
        /// </summary>
        /// <param name="rec"> rectangle of other object </param>
        /// <returns> whether true/false if the pipes collied with another rectangle </returns>
        public bool CheckCollision(Rectangle rec)
        {
            for (int i = 0; i < PIPE_CNT; i++)
            {
                if (pipes[i].IsCollision(rec))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// method that checks whether if a pipe has passed the bird
        /// </summary>
        /// <param name="rec"> rectangle of the other object (bird) </param>
        /// <returns> whether true/false if the pipe has passed the bird </returns>
        public bool CheckPassed(Rectangle rec)
        {
            for (int i = 0; i < PIPE_CNT; i++)
            {
                if (rec.Right >= pipes[i].GetRecRight() && !pipes[i].isPassed)
                {
                    pipes[i].isPassed = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// method that generates the pipes back with respect the last pipe's Y value
        /// </summary>
        /// <param name="index"> the index of the pipe which is generated again </param>
        private void GeneratePipe(int index)
        {
            // reset pip passed
            pipes[index].isPassed = false;

            // the right most pipe would be one index smaller
            int rightMost = index - 1;
            if (index == 0)
            {
                rightMost = PIPE_CNT - 1;
            }

            randLocY = (int)MathHelper.Clamp(rng.Next(-1 * PIPE_VER_DIS, PIPE_VER_DIS) + pipes[rightMost].GetLoc().Y, -1 * Pipe.HEIGHT + PIPE_MIN_Y_FACTR, PIPE_MAX_Y);

            // reposition image back to the other end of the other image
            pipes[index].SetLoc(pipes[rightMost].GetLoc().X + Pipe.WIDTH + PIPE_X_SPC, randLocY);
        }

        /// <summary>
        /// method to draw all the pipes in the pipe List
        /// </summary>
        public void DrawPipes()
        {
            for (int i = 0; i < PIPE_CNT; i++)
            {
                pipes[i].Draw();
            }
        }

    }
}
