using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace WangC_MP1
{
    static class Assets
    {
        // Credits: This class is made with the help of Alex

        /// <summary>
        /// set and get the local content manager
        /// </summary>
        public static ContentManager Content { get; set; } // used get and set, but everything else i'm to lazy to use get/set

        private static string loadPath;

        public static Song bgMusic;

        public static SoundEffect deathSnd;
        public static SoundEffect flapSnd;
        public static SoundEffect hitSnd;
        public static SoundEffect fadeSnd;
        public static SoundEffect pointSnd;

        public static Texture2D bgImg;
        public static Texture2D grdImg;

        public static Texture2D birdImg;
        public static Texture2D bottomPipeImg;
        public static Texture2D instrucImg;
        public static Texture2D coinsImg;

        public static Texture2D fadeImg;

        public static Texture2D gameOvrImg;
        public static Texture2D getReadyImg;

        public static Texture2D newImg;
        public static Texture2D rsltsBoxImg;

        public static Texture2D lrgNumsImg;
        public static Texture2D smallNumsImg;

        public static Texture2D sparkleImg;
        public static Texture2D menuBtnImg;
        public static Texture2D startBtnImg;
        public static Texture2D statsBtnImg;

        public static Texture2D statsBoxImg;
        public static Texture2D titleImg;
        public static Texture2D topPipeImg;

        /// <summary>
        /// method loads all assets (textures, sounds) to the game
        /// </summary>
        public static void Initialize()
        {
            // Switch the load path to music
            loadPath = "Audio/Music";

            // Load all music
            bgMusic = Load<Song>("Nature");

            // Switch the load path to sound
            loadPath = "Audio/Sound";

            // Load all sounds
            deathSnd = Load<SoundEffect>("Die");
            flapSnd = Load<SoundEffect>("Flap");
            hitSnd = Load<SoundEffect>("Hit");
            pointSnd = Load<SoundEffect>("Point");
            fadeSnd = Load<SoundEffect>("MenuSwoosh");

            // Switch the load path to backgrounds
            loadPath = "Images/Backgrounds";

            // Load all backgrounds
            bgImg = Load<Texture2D>("Background");
            grdImg = Load<Texture2D>("Ground");

            // Switch the load path to sprites
            loadPath = "Images/Sprites";

            // Load all sprites
            birdImg = Load<Texture2D>("Bird");

            topPipeImg = Load<Texture2D>("TopPipe");
            bottomPipeImg = Load<Texture2D>("BottomPipe");

            fadeImg = Load<Texture2D>("Fader");

            instrucImg = Load<Texture2D>("ClickInstruction");
            gameOvrImg = Load<Texture2D>("GameOver");
            getReadyImg = Load<Texture2D>("GetReady");

            rsltsBoxImg = Load<Texture2D>("ResultsBox");
            statsBoxImg = Load<Texture2D>("StatsPage");
            newImg = Load<Texture2D>("New");

            lrgNumsImg = Load<Texture2D>("BigNums");
            smallNumsImg = Load<Texture2D>("SmallNums");

            coinsImg = Load<Texture2D>("Coins");
            sparkleImg = Load<Texture2D>("Sparkle");

            menuBtnImg = Load<Texture2D>("MenuBtn");
            startBtnImg = Load<Texture2D>("StartBtn");
            statsBtnImg = Load<Texture2D>("StatsBtn");

            titleImg = Load<Texture2D>("Title");
        }

        /// <summary>
        /// loads the respective type of data with file path into a variable
        /// </summary>
        /// <typeparam name="T"> data type </typeparam>
        /// <param name="file"> file path asset </param>
        /// <returns> the data type 'T' </returns>
        private static T Load<T>(string file) => Content.Load<T>($"{loadPath}/{file}");
    }
}
