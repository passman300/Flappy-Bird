using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WangC_MP1
{
    class ScoreData
    {
        public const string FILE_TEMPLATE = "0,0\n0";

        private string filePath;

        private StreamReader inFile;
        private StreamWriter outFile;

        private int flapCnt;
        private int playsCnt;

        private List<int> scoresList = new List<int>();

        private int avgFlaps;
        private int avgScore;

        private int bestScore;

        /// <summary>
        /// constructor for the score data class
        /// </summary>
        /// <param name="filePath"> file path of the score data text file </param>
        public ScoreData(string filePath)
        {
            this.filePath = filePath;

            ReadFile();
        }

        public int GetFlapCount() { return flapCnt; }

        public void SetFlapsCounts(int value) { flapCnt = value; }

        public int GetPlaysCount() { return playsCnt; }

        public int GetAverageScore() { return avgScore; }

        public int GetAverageFlaps() { return avgFlaps; }

        public int GetBestScore() { return bestScore; }

        public void SetBestScore(int value) { bestScore = value; }

        /// <summary>
        /// gets the size of the score lists
        /// </summary>
        /// <returns> returns the size of the score list </returns>
        public int GetScoreListSize()
        {
            return scoresList.Count();
        }

        /// <summary>
        /// gets the value of the score in score list at a specific value
        /// </summary>
        /// <param name="index"> the index of the score which is to be returned </param>
        /// <returns> integer value of the indexed score </returns>
        public int GetScore(int index)
        {
            return scoresList[index];
        }

        /// <summary>
        /// method to read data from stats file, and save the data in respective varibles
        /// </summary>
        public void ReadFile()
        {
            // use try-catch to handle any errors
            try
            {
                // open the stats file
                inFile = File.OpenText(filePath);

                // if value is an int and greater or equal to 0, add the value to scores list
                string[] firstData; // use string as can't read array directly to int

                // read the first line and split it into an array
                firstData = inFile.ReadLine().Split(',');

                // try to covert the values of the first data array into ints and store them into their variables
                // if can't convert to int, set the values to 0
                if (!int.TryParse(firstData[0], out playsCnt) || playsCnt < 0)
                {
                    playsCnt = 0;
                }

                if (!int.TryParse(firstData[1], out flapCnt) || flapCnt < 0)
                {
                    flapCnt = 0;
                }

                // read  until the end of the file
                while (!inFile.EndOfStream)
                {
                    // create a temp, if value is an int and greater or equal to 0, add the value to scores list
                    if (int.TryParse(inFile.ReadLine(), out int tempScor) && tempScor >= 0)
                    {
                        scoresList.Add(tempScor);

                        // update best score
                        // set best score to the max of the temp score or the max score
                        bestScore = Math.Max(bestScore, tempScor);
                    }
                }

                inFile.Close();
            }
            // catch if the stats file does not exist
            catch (FileNotFoundException fnfe)
            {
                // create the file instead
                CreateFile();

                // read the file
                ReadFile();
            }
            catch (System.NullReferenceException nre)
            {
                inFile.Close();

                // create the file instead
                CreateFile();

                // read the file
                ReadFile();
            }
        }

        /// <summary>
        // method to write data to a stat txt file
        /// </summary>
        public void WriteStatsFile()
        {
            // create new text file (or over ride the old file)
            outFile = File.CreateText(filePath);

            // first line write the total amount of plays and flap count
            outFile.WriteLine(playsCnt + "," + flapCnt);

            // next lines write all the scores, using the scores list
            for (int i = 0; i < scoresList.Count(); i++)
            {
                outFile.WriteLine(scoresList[i]);
            }

            // close the outFile
            outFile.Close();
        }

        /// <summary>
        /// method used to create the stats file with template data
        /// </summary>
        private void CreateFile()
        {
            // create a empty txt file
            outFile = File.CreateText(filePath);

            // write the default stat values
            outFile.WriteLine(FILE_TEMPLATE);

            // close the file
            outFile.Close();
        }

        /// <summary>
        /// method used to update stats variables, playCount, average flaps and score, and sorting the score list
        /// </summary>
        public void UpdateStats()
        {
            playsCnt++;

            SortListDecending();
            avgFlaps = flapCnt / playsCnt;
            avgScore = SumScore() / scoresList.Count();

            WriteStatsFile();
        }

        /// <summary>
        /// method used to sort the score list from greatest to least
        /// </summary>
        private void SortListDecending()
        {
            // sort the score list (increasing)
            scoresList.Sort();

            // reverse the list to have it decreasing
            scoresList.Reverse();
        }

        /// <summary>
        /// method to add new score into the score list
        /// </summary>
        /// <param name="newScore"> integer value of new score that is added </param>
        public void AddScore(int newScore)
        {
            // add the value to scoresList
            scoresList.Add(newScore);
        }

        /// <summary>
        // method that returns the sum of all scores within the scores list
        /// </summary>
        /// <returns> the sum of scores in scores list </returns>
        private int SumScore()
        {
            // create a local temporary sum variable
            int sum = 0;

            // sum the values of scorsList
            foreach (int score in scoresList)
            {
                sum += score;
            }

            // return the value of sum
            return sum;
        }
    }
}
