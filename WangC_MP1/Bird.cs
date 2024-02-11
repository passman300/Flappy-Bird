using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace WangC_MP1
{
    public class Bird
    {
        public const int MENU = 0;
        public const int PRE_GAME = 1;
        public const int ALIVE = 2;
        public const int DEAD = 3;

        // bird animation duration in millisecond
        public const int ANIM_TITLE_DUR = 400; // time during title screen
        public const int ANIM_GAME_DUR = 200; // time during game screen

        // max and min speeds of the bird
        private const int MAX_SPD = 15;
        private const int MIN_SPD = -10;

        // max and min rotations of the bird
        private const int MAX_ANGL = 90;
        private const int MIN_ANGL = -30;

        // the speed of gravity and flaps
        private const float GRAV = 0.5f;
        private const int FLAP = -10;

        // type of death
        public const int GRD_DEATH = 0;
        public const int PIPE_DEATH = 1;

        public int state;
        public int deathType;

        private SpriteBatch spriteBatch;

        private Animation anim;

        private int angle;

        private Vector2[] rstLoc = new Vector2[2];
        private Vector2 loc;

        private float spd;

        private SoundEffectInstance hitSndInst;
        private SoundEffectInstance deathSndInst;
        private bool deathSndFin;

        private int localFlapCnt;

        /// <summary>
        /// constructor of the bird class
        /// </summary>
        /// <param name="spriteBatch"> pases a global SpriteBatch into a local one within the bird class </param>
        /// <param name="anim"> passes the animation for the bird </param>
        public Bird(SpriteBatch spriteBatch, Animation anim)
        {
            this.spriteBatch = spriteBatch;

            this.anim = anim;

            hitSndInst = Assets.hitSnd.CreateInstance();
            deathSndInst = Assets.deathSnd.CreateInstance();

            state = MENU;

            deathSndFin = false;
        }

        /// <summary>
        /// method that sets the reset location  of bird in the title screen and pre game play screen
        /// </summary>
        /// <param name="titleLoc"> the reset location of the title screen </param>
        /// <param name="preGameLoc"> the reset location of the pre game play screen </param>
        public void SetRstLoc(Vector2 titleLoc, Vector2 preGameLoc)
        {
            rstLoc[MENU] = titleLoc;
            rstLoc[PRE_GAME] = preGameLoc;

            // set location to title location
            loc = titleLoc;
            anim.TranslateTo(loc.X, loc.Y);
        }

        /// <summary>
        /// method used to get the reset location
        /// </summary>
        /// <param name="rstType"> the type of reset location. either MENU or PRE_GAME </param>
        /// <returns> a vector2 of the reset position </returns>
        public Vector2 GetRstLoc(int rstType)
        {
            return rstLoc[rstType];
        }

        /// <summary>
        /// method that gets the rectangle of the bird (actually just the rectangle of the bird animation)
        /// </summary>
        /// <returns> the rectangle of the bird's animation </returns>
        public Rectangle GetRec()
        {
            return anim.GetDestRec();
        }

        /// <summary>
        /// method that sets location of the bird
        /// </summary>
        /// <param name="x"> X location of the bird </param>
        /// <param name="y"> Y location of the bird </param>
        public void SetLoc(float x, float y)
        {
            loc.X = x;
            loc.Y = y;

            anim.TranslateTo(loc.X, loc.Y);
        }

        /// <summary>
        /// method that gets the location the bird
        /// </summary>
        /// <returns> vector2 of the bird </returns>
        public Vector2 GetLoc()
        {
            return loc;
        }

        /// <summary>
        /// method that sets the bird animation to it's death mode. Where the animation is no longer active, and idling in frame 1
        /// </summary>
        public void SetDeathAnim()
        {
            anim.Deactivate();
            anim.SetIdleFrame(1);

            state = DEAD;
        }

        /// <summary>
        /// method that sets the animation duration
        /// </summary>
        /// <param name="time"> the time of the animation </param>
        public void SetAnimDur(int time)
        {
            anim.SetDuration(time);
        }

        /// <summary>
        /// method that returns the number of local flaps
        /// </summary>
        /// <returns> integer value of flaps </returns>
        public int GetFlapCount()
        {
            return localFlapCnt;
        }

        /// <summary>
        /// method the updates the bird
        /// </summary>
        /// <param name="gameTime"> passes the amount of time passed within the game </param>
        /// <param name="isInput"> boolean that passes in if any valid input was given, set default value to false</param>
        public void Update(GameTime gameTime, bool isInput = false)
        {
            anim.Update(gameTime);

            if (state == ALIVE || state == DEAD)
            {
                UpdateBirdLoc(isInput);
            }

            if (state == DEAD && hitSndInst.State == SoundState.Stopped && !deathSndFin)
            {
                deathSndFin = true;

                PlaySnd(deathSndInst);
            }

        }

        /// <summary>
        /// method that updates the bird location, based on the input given
        /// </summary>
        /// <param name="isInput">boolean that passes in if any valid input was given </param>
        private void UpdateBirdLoc(bool isInput)
        {
            if (state == ALIVE && isInput)
            {
                spd = FLAP;

                PlaySnd(Assets.flapSnd.CreateInstance());

                // increase flap count
                localFlapCnt++;
            }

            spd += GRAV;
            spd = MathHelper.Clamp(spd, MIN_SPD, MAX_SPD);

            // change the position of the bird with the speed
            loc.Y += spd;

            // translate the bird to that point
            anim.TranslateTo(loc.X, loc.Y);

            // update the bird's tilt angle
            UpdateDegRotation(spd);
        }

        /// <summary>
        /// method that updates the angle of rotation of the bird based on the speed of the bird
        /// </summary>
        /// <param name="spd"> the speed of the bird </param>
        private void UpdateDegRotation(float spd)
        {
            anim.SetAngleDeg(MathHelper.Lerp(MIN_ANGL, MAX_ANGL, (spd - MIN_SPD) / (MAX_SPD - MIN_SPD)));
        }

        /// <summary>
        /// method that plays the hit sound
        /// </summary>
        public void PlayHitSnd()
        {
            PlaySnd(hitSndInst);
        }

        /// <summary>
        /// method that plays the death sound
        /// </summary>
        public void PlayDeathSnd()
        {
            if (!deathSndFin)
            {
                PlaySnd(deathSndInst);
                deathSndFin = true;
            }

        }

        /// <summary>
        /// method that plays any sound
        /// </summary>
        /// <param name="sndInst"> the sound instance which is to be played </param>
        /// <param name="vol"> the volume of the sound instance, which is set to 1f as the default value </param>
        private void PlaySnd(SoundEffectInstance sndInst, float vol = 1f)
        {
            sndInst.Volume = vol;
            sndInst.Play();
        }

        /// <summary>
        /// method to reset the bird
        /// </summary>
        /// <param name="rstType"> the reset type of the bird, either, MENU or PRE_GAME</param>
        public void Rst(int rstType)
        {
            anim.Activate(true);


            switch (rstType)
            {
                case MENU:
                    SetAnimDur(ANIM_TITLE_DUR);

                    spd = 0;
                    angle = 0;

                    loc = rstLoc[MENU];

                    state = MENU;
                    break;
                case PRE_GAME:
                    spd = FLAP;
                    angle = 0;

                    loc = rstLoc[PRE_GAME];

                    deathSndFin = false;

                    state = PRE_GAME;

                    localFlapCnt = 0;
                    break;

            }

            anim.SetAngleDeg(angle);
            anim.TranslateTo(loc.X, loc.Y);
        }

        /// <summary>
        /// method that draws the bird
        /// </summary>
        public void Draw()
        {
            anim.DrawRotated(spriteBatch, Color.White);
        }
    }
}
