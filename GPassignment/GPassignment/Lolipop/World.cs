using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;



/// <summary>
/// This World class will set up the world for you.
/// Level one!
/// Just the world.
/// </summary>

namespace Cemetery_Escape
{
    class World
    {
        #region Variables

        const int NORMALZOMBIE = 0, JUMPINGZOMBIE = 1, SLACKERZOMBIE = 2, DISSAPPESRZOMBIE = 3;

        //for chaser
        public Chaser chaser;
        float interval = 0f, spawnInterval = 0f;
        bool runOnce = false;

        public string mapID = "ahaha";
        public int[,] tileMap;
        int tileSize;
        int mapSizeX;
        int mapSizeY;
        Texture2D texture;
        List<Texture2D> textureList;
        public List<Rectangle> blocks;
        public List<Rectangle> blocksTop;
        public List<Rectangle> blocksBottom;
        public List<Rectangle> blocksLeft;
        public List<Rectangle> blocksRight;
        public List<Rectangle> tile;
        public int mapX;
        public int mapW;
        public int finalscore;

        public Rectangle finish;

        //zombie eh eh
        public List<Zombie> zombieList;
        public bool spawnReady = true;
        int oldZombieTotal = 0;

        //background variable
        Texture2D backgroundTexture;
        Rectangle backgroundDestinationRectangle;

        //credit variable
        //porn video
        Video video;
        public VideoPlayer player;
        Texture2D videoTexture;

        //tombstone cordiate
        public List<Rectangle> tombStoneList;

        //health powerup locations
        public List<Rectangle> healthPowerUp;

        //fog of war powerup locations
        public List<Rectangle> fogPowerUp;

        //root location
        public List<Root> rootLocationList;

        //for text pop up
        public List<onScreenText> textList;
        SpriteFont spriteFont;


        //sound
        SoundEffect spawnSound;
        SoundEffectInstance spawnSoundInstance;
        
        #endregion

        #region Main Methods

        public World(int mapNumber)
        {
            int rows = 0, columns = 0;
            string line;
            string[] size;

            //initialise falling object 
            //fallingObject = new FallingObject();

            if(mapNumber != 1)
                mapID = "Maps" + mapNumber + ".txt";
            else
                mapID = "Maps.txt";

            Global.highscore.mapID = mapID;
            // counting the row and columns
            StreamReader mapReader = new StreamReader("Content/Maps/" + mapID);
            while ((line = mapReader.ReadLine()) != null)
            {
                if (line == "ENDMAP")
                {
                    break;
                }
                rows = line.Count();
                ++columns;
            }
            mapReader.Close();

            mapReader = new StreamReader("Content/Maps/" + mapID);

            ///adding the tiles value to the array
            tileMap = new int[columns, rows];
            int counterRow = 0, counterColumns = 0;
            ///
            while (counterColumns != columns)
            {
                counterRow = 0;
                size = mapReader.ReadLine().Split(',');
                foreach (string letter in size)
                {
                    tileMap[counterColumns, counterRow] = Convert.ToInt32(letter);
                    ++counterRow;
                }
                ++counterColumns;
            }

            tileSize = 40;
            textureList = new List<Texture2D>();                
            mapSizeX = tileMap.GetLength(1);                    /// get X
            mapSizeY = tileMap.GetLength(0);                    /// get Y
            blocks = new List<Rectangle>();                     
            blocksTop = new List<Rectangle>();
            blocksLeft = new List<Rectangle>();
            blocksRight = new List<Rectangle>();
            blocksBottom = new List<Rectangle>();
            tile = new List<Rectangle>();
            zombieList = new List<Zombie>();
            tombStoneList = new List<Rectangle>();
            healthPowerUp = new List<Rectangle>();
            fogPowerUp = new List<Rectangle>();
            rootLocationList = new List<Root>();
            mapX = 0;
            textList = new List<onScreenText>();

            /// 
            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {   //1 - Top land with grass
                    //2 - skeleton in side land
                    //3 - only land!
                    //4 - tombstone
                    //5 - Exit
                    //6 - Health power up
                    //7 - fog of war power up
                    if (tileMap[y, x] == 1 || tileMap[y, x] == 2 || tileMap[y, x] == 3 || tileMap[y, x] == 4 || tileMap[y, x] == 5 || tileMap[y, x] == 6 || tileMap[y, x] == 7 || tileMap[y, x] == 8)
                    {
                        //if tombstone
                        if (tileMap[y, x] == 4)
                        {
                            blocks.Add(new Rectangle(x * tileSize - mapX, y * tileSize, 35, 30));
                            tombStoneList.Add(new Rectangle(x * tileSize - mapX, y * tileSize, 35, 30));
                        }
                        else if (tileMap[y, x] == 5)
                        {
                            finish = new Rectangle(x * tileSize - mapX, y * tileSize, 40, 40);
                        }
                        else if (tileMap[y, x] == 6)
                        {
                            healthPowerUp.Add(new Rectangle(x * tileSize - mapX, y * tileSize, 35, 30));
                           // tileMap[y, x] = 0;
                        }
                        else if (tileMap[y, x] == 7)
                        {
                            fogPowerUp.Add(new Rectangle(x * tileSize - mapX, y * tileSize, 35, 30));
                            //tileMap[y, x] = 0;
                        }
                        else if (tileMap[y, x] == 8)
                        {
                            tileMap[y, x] = 0;
                            rootLocationList.Add(new Root(new Vector2(x * tileSize - mapX, y * tileSize)));
                        }
                        else
                            blocks.Add(new Rectangle(x * tileSize - mapX, y * tileSize, 35, 30));                  

                    }
                }
            }

            foreach (Rectangle block in blocks)
            {
                blocksTop.Add(new Rectangle(block.Center.X - 5, block.Top, block.Width, block.Height));
            }
            foreach (Rectangle block in blocks)
            {
                blocksBottom.Add(new Rectangle(block.Center.X- 5, block.Bottom, block.Width, block.Height));
            }
            foreach (Rectangle block in blocks)
            {
                blocksLeft.Add(new Rectangle(block.Left, block.Y, 1, block.Height));
            }
            foreach (Rectangle block in blocks)
            {
                blocksRight.Add(new Rectangle(block.Right, block.Y, block.Width / 2, block.Height));
            }

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    tile.Add(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                }
            }

            mapW = tileSize * mapSizeX;
            mapReader.Close();

        }

        public void LoadContent(ContentManager Content)
        {
            //loading the highscore
            finalscore = Global.highscore.loadHighscore();
            //loading the background
            backgroundTexture = Content.Load<Texture2D>("Images/WorldMap/Background");
            //rectangle(LocationX, LocationY, Width, Height
            backgroundDestinationRectangle = new Rectangle(0, 0, 1000, 650);

            //loading of chaser
            chaser = new Chaser();
            chaser.LoadContent(Content);


            //loading sounds
            spawnSound = Content.Load<SoundEffect>("Sounds/Chaser_Voice");
            spawnSoundInstance = spawnSound.CreateInstance(); 
            spawnSoundInstance.IsLooped = true;

            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/in"));                 //0
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/grass"));              //1
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/skeleton"));           //2
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/ground"));             //3
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/tombstone"));          //4
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/gate"));               //5
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/health_powerup"));     //6
            textureList.Add(Content.Load<Texture2D>("Images/WorldMap/fog_powerup"));        //7

            if (mapID == "Maps999.txt")
            {
                //for Credits
                video = Content.Load<Video>("Images/Story/credits");
                player = new VideoPlayer();
            }

            //font
            spriteFont = Content.Load<SpriteFont>("FontForXY");

            //loading zombie from file
            string line,text;
            string[] size;
            StreamReader zombieReader = new StreamReader("Content/Maps/" + mapID);

            while ((line = zombieReader.ReadLine()) != null)
            {
                if(line.Equals("LOADZOMBIE"))  //0,200,90,2f,0
                {
                    line = zombieReader.ReadLine();
                    size = line.Split(','); 

                    LoadZombie(Convert.ToInt32(size[0]), Content, Convert.ToInt32(size[1]), Convert.ToInt32(size[2]), (float)Convert.ToDouble(size[3]), (float)Convert.ToDouble(size[4]));
                }
            }
            zombieReader.Close();

            //Text for map
            zombieReader = new StreamReader("Content/Maps/" + mapID);
            while ((line = zombieReader.ReadLine()) != null)
            {
                if (line.Equals("LOADTEXT"))  //0,200,90,2f,0
                {
                    text = zombieReader.ReadLine();
                    line = zombieReader.ReadLine();
                    size = line.Split(',');

                    textList.Add(new onScreenText(text,new Vector2(Convert.ToInt32(size[0]), Convert.ToInt32(size[1]) ) ) );
                }
            }
            zombieReader.Close();

            oldZombieTotal = zombieList.Count();

            foreach (Root r in rootLocationList)
            {
                r.LoadContent(Content);
            }
        }

        public void Update(GameTime gameTime, ContentManager Content)
        {
            interval += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (interval > 3f && !runOnce)
            {
                chaser.cameraPosition.Y = Global.player.cameraPosition.Y;
                chaser.worldPosition.Y = Global.player.worldPosition.Y;
                runOnce = !runOnce;
            }
            if (runOnce)
            {
                chaser.Update(gameTime);
                spawnSoundInstance.Play();
                if (interval > 7f)
                    spawnSoundInstance.Stop();
            }
            foreach (Zombie z in zombieList)
            {
                z.Update(gameTime);
            }

            foreach (Root r in rootLocationList)
            {
                r.Update(gameTime);
            }
            //HUD Update for player with the world settings
            Global.player.playerHUD.UpdatePointer(new Vector2(finish.X + 20,finish.Y + 20));
            Global.player.playerHUD.Update(gameTime);

            //chaser
            checkChaserCollision(Content, gameTime);
                
            //credit screen update vid
            if (mapID == "Maps999.txt")
            {
                player.IsLooped = true;
                player.Play(video);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mapID != "Maps999.txt")
            {
                //draw background first
                spriteBatch.Draw(backgroundTexture, backgroundDestinationRectangle, Color.White);
            }
            else
            {
                // Only call GetTexture if a video is playing or paused
                if (player.State != MediaState.Stopped)
                    videoTexture = player.GetTexture();

                // Draw the video, if we have a texture to draw.
                if (videoTexture != null)
                {
                    spriteBatch.Draw(videoTexture, new Rectangle(0, 0, 1000, 650), Color.White);
                }
            }


            for (int i = 0; i < mapSizeX; i++)
            {
                for (int j = 0; j < mapSizeY; j++)
                {
                    if (tileMap[j, i] != 8)
                    {
                        texture = textureList[tileMap[j, i]];
                        spriteBatch.Draw(texture, new Rectangle(i * tileSize - mapX, j * tileSize, tileSize, tileSize), Color.White);
                    }
                }
            }

            //draw power up
            //fog
            //foreach (Rectangle fog in fogPowerUp)
            //{
            //   spriteBatch.Draw(textureList[7], fog, Color.White);
            //}

            ////health
            //foreach (Rectangle health in healthPowerUp)
            //{
            //    spriteBatch.Draw(textureList[6], health, Color.White);
            //}

            //draw chaser
            if (runOnce)
            {
                chaser.Draw(spriteBatch);
            }
            foreach (Zombie z in zombieList)
            {
                z.Draw(spriteBatch);
            }

            foreach (onScreenText OST in textList)
            {
                spriteBatch.DrawString(spriteFont, OST.text, OST.cameraPosition, Color.White);
            }

            foreach (Root r in rootLocationList)
            {
                r.Draw(spriteBatch);
            }

            // fallingObject.Draw(spriteBatch);
        }

        #endregion

        #region zombie

        public void LoadZombie(int zombieBehavior, ContentManager Content, int startX, int startY, float speed, float classValue)
        { //if(enumerable.FirstOrDefault() != null)
            switch(zombieBehavior)
            {
                case NORMALZOMBIE :
                     zombieList.Add(new Zombie());
                     zombieList[zombieList.Count() - 1].LoadContent(Content, startX, startY, speed);
                     break;
                case JUMPINGZOMBIE:
                     zombieList.Add(new JumpingZombie());
                     zombieList[zombieList.Count() - 1].LoadContent(Content, startX, startY, speed, classValue);
                     break;
                case SLACKERZOMBIE:
                     zombieList.Add(new SlackerZombie());
                     zombieList[zombieList.Count() - 1].LoadContent(Content, startX, startY, speed, classValue);
                     break;
                case DISSAPPESRZOMBIE:
                    zombieList.Add(new DisappearZombie());
                     zombieList[zombieList.Count() - 1].LoadContent(Content, startX, startY, speed, classValue);
                     break;
            }
        }

        public void ZombieCamera(float i)
        {
            foreach (Zombie z in zombieList)
            {
                z.cameraPosition.X += i;
                //z.worldPosition.X += i;
            }
        }

        #endregion

        #region Chaser Methods

        private void checkChaserCollision(ContentManager Content, GameTime gameTime)
        {
            spawnInterval += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnInterval > 5f && spawnReady == false)
            {
                spawnReady = true;
                oldZombieTotal = zombieList.Count();
            }

            if (spawnReady)// && oldZombieTotal == zombieList.Count())
            {
                foreach (Rectangle tomb in tombStoneList)
                {
                    if ((chaser.worldPosition.X - tomb.X < 20 && chaser.worldPosition.Y - tomb.Y < 40) &&
                        (chaser.worldPosition.X - tomb.X > -20 && chaser.worldPosition.Y - tomb.Y > -40))
                        if (tomb.Intersects(chaser.rectangle))
                    {
                        spawnReady = !spawnReady;
                        spawnInterval = 0f;
                        int random = Global.GetRandomNumber(0,3);
                        float speed = chaser.speed;
                        //load zombie
                        LoadZombie(random, Content, (int)chaser.worldPosition.X, (int)chaser.worldPosition.Y - 40, speed, 1);
                        zombieList[zombieList.Count - 1].cameraPosition.X = chaser.cameraPosition.X;
                        zombieList[zombieList.Count - 1].cameraPosition.Y = chaser.cameraPosition.Y - 40;
                    }
                    
                }
            }

        }

        #endregion

        #region text camera
        public void textCamera(float toAdd)
        {
            foreach (onScreenText OST in textList)
            {
                OST.cameraPosition.X += toAdd;
            }

            foreach (Root RatedR in rootLocationList)
            {
                RatedR.cameraPosition.X += toAdd;
            }
        }
        #endregion

    }
}
