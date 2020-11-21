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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

///<Summary>
///This class is to display the highscore by the player
///</Summary>

namespace Cemetery_Escape
{
    class HighScore
    {
        #region Variables
        public int currentScore, finalscore = 0, totalScore=0, totalMap=0 ; 
        public string highscore;
        public string count = null;
        public int time;
        public System.IO.StreamWriter filewriter;
        public System.IO.StreamReader filereader;
        public string mapID;
        #endregion

        public void LoadContent(int tMap)
        {
            totalMap = tMap;
        }

        public int loadHighscore()
        {
            try
            {
                int mapNo = 1;
                if (mapID.Equals("Maps.txt"))
                    filereader = new System.IO.StreamReader("Content/Scores/highscore.txt");
                else
                {
                    while (true)
                    {
                        ++mapNo; //mapNo will be 2 on first execute

                        if (mapNo > totalMap) 
                            return 0;

                        if (mapID.Equals("Maps" + mapNo + ".txt"))
                        {
                            filereader = new System.IO.StreamReader("Content/Scores/highscore" + mapNo + ".txt");
                            break;
                        }

                    }
                }

                finalscore = Convert.ToInt32(filereader.ReadLine());
                filereader.Close();
                return finalscore;
            }
            catch(System.IO.IOException)
            {
                finalscore = 0;
                return finalscore;
            }
        }


        public void CalculateScore()
        {
            int mapNo = 1;
            time = (Global.player.playerHUD.getMin() * 60) + Global.player.playerHUD.getSec();
            currentScore = 5000 - (((int)Global.player.playerHealth * 10) + (time * 30));
            if (currentScore > finalscore)
            {
                //tempScore = finalscore;
                highscore = currentScore.ToString();
                if (mapID.Equals("Maps.txt"))
                {
                    filewriter = new System.IO.StreamWriter("Content/Scores/highscore.txt");
                    filewriter.WriteLine(highscore);
                    filewriter.Close();
                }
                else
                    while (mapNo <= totalMap)
                    {
                        ++mapNo;

                        if (mapID.Equals("Maps" + mapNo + ".txt"))
                        {
                            filewriter = new System.IO.StreamWriter("Content/Scores/highscore" + mapNo + ".txt");
                            filewriter.WriteLine(highscore);
                            filewriter.Close();
                            break;
                        }
                    }
            }
        }

        public int getCurrentScore()
        {
            time = (Global.player.playerHUD.getMin() * 60) + Global.player.playerHUD.getSec();
            try
            {
                return 5000 - (((int)Global.player.playerHealth * 10) + (time * 30));
            }
            catch (DivideByZeroException)
            {
                return 0;
            }

        }

        public void addTotalScore(int toAdd)
        {
            filewriter = new System.IO.StreamWriter("Content/Scores/totalScore.txt");
            filewriter.WriteLine(toAdd);
            filewriter.Close();
        }

        public int getTotalScore()
        {
            int thisPlease = 0;
            try
            {
                filereader = new System.IO.StreamReader("Content/Scores/totalScore.txt");
                thisPlease = Convert.ToInt32(filereader.ReadLine());
                filereader.Close();
                return thisPlease;
            }
            catch (System.IO.FileNotFoundException)
            {
                return 0;
            }
        }

    }
}
