using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using SpacePirates.Player;


namespace SpacePirates
{
    class MenuObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static MenuObject instance;
        static readonly object padlock = new Object();

        private bool enableMultiplayer = true;

        // Holds the width and the height of the viewport
        private int windowWidth;
        private int windowHeight;

        //Placeholders for keeping track of menu positions.
        private int mainmenu = 0;
        private int createdlobby = 1;
        private int joinedlobby = 2;
        private int searchLobbies = 3;
        private int victoryLobby = 4;

        private int currentMenu = 0; //Main menu.

        public bool active = false; // Is true if this is the currentObject in Game1.cs

        //Network sessions search.
        private AvailableNetworkSessionCollection availableSessions;
        private int selectedSessionIndex;

        private int selectedShipIndex = 0;

        private List<string> victoryText;

        private PacketReader packetReader = new PacketReader();
        private PacketWriter packetWriter = new PacketWriter();

        public bool isActive()
        {
            return active;
        }

        //Displayed game banner
        Texture2D banner;

        //"Buttons" for the main menu. Could likely get converted into true buttons but I've so far been unable to draw them properly.
        Texture2D newSession;
        Vector2 newSessionPos;
        Texture2D joinSession;
        Vector2 joinSessionPos;
        Texture2D quitSession;
        Vector2 quitSessionPos;
        Texture2D backButton;
        Vector2 backButtonPos;
        Texture2D startGame;
        Vector2 startGamePos;
        Texture2D readyButton;
        Vector2 readyButtonPos;

        SpriteFont text;

        Vector2 bannerPosition;

        //Handling the keyboard inputs.
        private KeyboardState oldKeyState = Keyboard.GetState();

        private MenuObject(int w, int h, ContentManager Content)
        {
            MenuObject self = this;

            self.windowWidth = w;
            self.windowHeight = h;

            self.bannerPosition = new Vector2(0, 0);

            self.banner = Content.Load<Texture2D>("banner");

            self.newSession = Content.Load<Texture2D>("MenuButtons/NewSession");
            self.newSessionPos = new Vector2(10, banner.Height + 10);
            self.joinSession = Content.Load<Texture2D>("MenuButtons/JoinSession");
            self.joinSessionPos = new Vector2(10, newSessionPos.Y + newSession.Height + 10);
            self.quitSession = Content.Load<Texture2D>("MenuButtons/Quit");
            self.quitSessionPos = new Vector2(10, joinSessionPos.Y + joinSession.Height + 10);
            self.startGame = Content.Load<Texture2D>("MenuButtons/StartGame");
            self.startGamePos = new Vector2(10, banner.Height + 10);
            self.readyButton = Content.Load<Texture2D>("MenuButtons/Ready");
            self.readyButtonPos = new Vector2(10, banner.Height + 10);
            self.backButton = Content.Load<Texture2D>("MenuButtons/BackButton");
            self.backButtonPos = new Vector2(10, windowHeight - 10 - backButton.Height);

            self.text = Content.Load<SpriteFont>("Graphics/Spritefonts/Menutext");
        }


        public static MenuObject Instance(int w, int h, ContentManager Content)
        {
            lock (padlock) {
                if (instance == null) {
                    instance = new MenuObject(w, h, Content);
                }
                return instance;
            }
        }

        public static MenuObject Instance()
        {
            lock (padlock)
            {
                return instance;
            }
        }

        //Handle menu screen input and update.
        public void executeGameLogic(GameTime gameTime)
        {
            KeyboardState newKeyState = Keyboard.GetState();

            if(currentMenu == mainmenu)
            {
                if (newKeyState.IsKeyDown(Keys.N) && !oldKeyState.IsKeyDown(Keys.N))//New Session.
                {
                    currentMenu = createdlobby;
                    if (NetworkObject.Instance().getNetworked())
                    {
                        if (!NetworkObject.Instance().CreateSession())
                            currentMenu = mainmenu;
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.J) && !oldKeyState.IsKeyDown(Keys.J))//Join Session.
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        currentMenu = searchLobbies;
                        int maxLocalPlayers = 1;
                        availableSessions = NetworkSession.Find(NetworkSessionType.SystemLink, maxLocalPlayers, null);
                        selectedSessionIndex = 0;
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.Q) && !oldKeyState.IsKeyDown(Keys.Q))//Quit game.
                {
                    Environment.Exit(0);
                }
            }
            else if (currentMenu == createdlobby)
            {
                ReceiveNetworkData();

                if (newKeyState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B))//Back to Main menu.
                {
                    currentMenu = mainmenu;
                    if (NetworkObject.Instance().getNetworked())
                    {
                        NetworkObject.Instance().disposeNetworkSession();
                        cleanAvailableSessions();
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.S) && !oldKeyState.IsKeyDown(Keys.S))//Start the game?
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        if (NetworkObject.Instance().getNetworksession().IsEveryoneReady)
                        {
                            NetworkObject.Instance().getNetworksession().StartGame();
                            active = false;
                        }
                    }
                    else
                    {
                        active = false;
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.R) && !oldKeyState.IsKeyDown(Keys.R))//Indicate ready.
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady && (gamer.Tag as Human).GetPickedTeam())
                                gamer.IsReady = true;
                            else
                                gamer.IsReady = false;
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.E) && !oldKeyState.IsKeyDown(Keys.E))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                                (gamer.Tag as Human).SetTeam(1);
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.L) && !oldKeyState.IsKeyDown(Keys.L))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                                (gamer.Tag as Human).SetTeam(2);
                        }
                    }                
                }
                else if (newKeyState.IsKeyDown(Keys.Left) && !oldKeyState.IsKeyDown(Keys.Left))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                            {
                                if (selectedShipIndex > 0)
                                {
                                    selectedShipIndex--;
                                    (gamer.Tag as Human).SetSelectedShip(GameObject.Instance().GetShipCollection().ElementAt(selectedShipIndex).Key);
                                }
                            }
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.Right) && !oldKeyState.IsKeyDown(Keys.Right))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                            {
                                if (selectedShipIndex < GameObject.Instance().GetShipCollection().Count - 1) //max
                                {
                                    selectedShipIndex++;
                                    (gamer.Tag as Human).SetSelectedShip(GameObject.Instance().GetShipCollection().ElementAt(selectedShipIndex).Key);
                                }
                            }
                        }
                    }
                }

                SendNetworkData();
            }
            else if (currentMenu == joinedlobby)
            {
                ReceiveNetworkData();

                if (newKeyState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B))//Back to Main menu.
                {
                    currentMenu = mainmenu;
                    cleanAvailableSessions();
                    NetworkObject.Instance().disposeNetworkSession();
                }
                else if (newKeyState.IsKeyDown(Keys.R) && !oldKeyState.IsKeyDown(Keys.R))//Indicate ready.
                {
                    if (NetworkObject.Instance().getNetworked())
                    { 
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady && (gamer.Tag as Human).GetPickedTeam())
                                gamer.IsReady = true;
                            else
                                gamer.IsReady = false;
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.E) && !oldKeyState.IsKeyDown(Keys.E))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if(!gamer.IsReady)
                                (gamer.Tag as Human).SetTeam(1);
                        }
                    }             
                }
                else if (newKeyState.IsKeyDown(Keys.L) && !oldKeyState.IsKeyDown(Keys.L))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                                (gamer.Tag as Human).SetTeam(2);
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.Left) && !oldKeyState.IsKeyDown(Keys.Left))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                            {
                                if (selectedShipIndex > 0)
                                {
                                    selectedShipIndex--;
                                    (gamer.Tag as Human).SetSelectedShip(GameObject.Instance().GetShipCollection().ElementAt(selectedShipIndex).Key);
                                }
                            }
                        }
                    }
                }
                else if (newKeyState.IsKeyDown(Keys.Right) && !oldKeyState.IsKeyDown(Keys.Right))
                {
                    if (NetworkObject.Instance().getNetworked())
                    {
                        foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                        {
                            if (!gamer.IsReady)
                            {
                                if (selectedShipIndex < GameObject.Instance().GetShipCollection().Count - 1) //max
                                {
                                    selectedShipIndex++;
                                    (gamer.Tag as Human).SetSelectedShip(GameObject.Instance().GetShipCollection().ElementAt(selectedShipIndex).Key);
                                }
                            }
                        }
                    }
                }

                SendNetworkData();

            }
            else if (currentMenu == searchLobbies)
            {
                if (newKeyState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B))//Back to Main menu.
                {
                    currentMenu = mainmenu;

                }
                else if (newKeyState.IsKeyDown(Keys.Up) && !oldKeyState.IsKeyDown(Keys.Up))
                {
                    if (selectedSessionIndex > 0)
                        selectedSessionIndex--;
                }
                else if (newKeyState.IsKeyDown(Keys.Down) && !oldKeyState.IsKeyDown(Keys.Down))
                {
                    if (selectedSessionIndex < availableSessions.Count)
                        selectedSessionIndex++;
                }
                else if (newKeyState.IsKeyDown(Keys.J) && !oldKeyState.IsKeyDown(Keys.J))//Join lobby.
                {
                    if (availableSessions.Count > 0)
                    {
                        currentMenu = joinedlobby;
                        NetworkSession networkSession = NetworkSession.Join(availableSessions[selectedSessionIndex]);

                        NetworkObject.Instance().setNetworkSession(networkSession);

                        cleanAvailableSessions();
                    }
                }
            }
            else if (currentMenu == victoryLobby)
            {
                if (newKeyState.IsKeyDown(Keys.B) && !oldKeyState.IsKeyDown(Keys.B))//Back to Main menu.
                {

                    foreach (SignedInGamer loc in SignedInGamer.SignedInGamers)
                    {
                        loc.Tag = new Human(loc.Gamertag);
                    }

                    currentMenu = mainmenu;
                    NetworkObject.Instance().disposeNetworkSession();
                }
            }

            oldKeyState = newKeyState;

            if (NetworkObject.Instance().getNetworked() && NetworkObject.Instance().getNetworksession() != null)
            {
                NetworkObject.Instance().getNetworksession().Update();
            }

        }

        //Draw the different menu screens in the game.
        public void executeDraw(SpriteBatch spriteBatch)
        {
            if (currentMenu == mainmenu)
            {
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(newSession, newSessionPos, Color.White);
                spriteBatch.Draw(joinSession, joinSessionPos, Color.White);
                spriteBatch.Draw(quitSession, quitSessionPos, Color.White);
                spriteBatch.DrawString(text, NetworkObject.Instance().getDebugString(), quitSessionPos + new Vector2(0, quitSession.Height + 40), Color.OrangeRed);
            }
            else if (currentMenu == createdlobby)
            {
                Color startCol = Color.Chocolate;
                Vector2 lastPos = startGamePos + new Vector2(startGame.Width + 40, 0);
                spriteBatch.Draw(banner, bannerPosition, Color.White);

                spriteBatch.Draw(readyButton, startGamePos + new Vector2(0, readyButton.Height + 10), Color.White);
                spriteBatch.DrawString(text, "Lobby", lastPos, Color.White);

                if (NetworkObject.Instance().getNetworked())
                {
                    if (NetworkObject.Instance().getNetworksession().IsEveryoneReady)
                        startCol = Color.White;

                    foreach (NetworkGamer gamer in NetworkObject.Instance().getNetworksession().AllGamers)
                    {
                        string name = gamer.Gamertag + " - Ship(" + (gamer.Tag as Human).GetShipSelection().ToString() + ")";
                        Color playerCol = Color.White;

                        if (gamer.IsReady)
                        {
                            name += " -- Ready!";
                        }
                        if ((gamer.Tag as Human).GetPickedTeam())
                        {
                            int nTeam = (gamer.Tag as Human).GetTeam();

                            if (nTeam == 1)
                                playerCol = Color.Red;
                            else
                                playerCol = Color.Blue;
                        }

                        spriteBatch.DrawString(text, name, lastPos + new Vector2(0, 30), playerCol);
                        lastPos.Y += 30;
                    }
                }
                else
                    startCol = Color.White;

                spriteBatch.Draw(startGame, startGamePos, startCol);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);
                spriteBatch.DrawString(text, "Press E for red team,\nL for blue team.\nLeft and right arrows to\ncycle ship selection.", startGamePos + new Vector2(0, readyButton.Height * 2 + 10), Color.White);
            }
            else if (currentMenu == joinedlobby)
            {
                Vector2 lastPos = startGamePos + new Vector2(readyButton.Width + 40, 0);
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(readyButton, readyButtonPos, Color.White);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);
                spriteBatch.DrawString(text, "Lobby", lastPos, Color.White);

                foreach (NetworkGamer gamer in NetworkObject.Instance().getNetworksession().AllGamers)
                {
                    string name = gamer.Gamertag + " - Ship(" + (gamer.Tag as Human).GetShipSelection().ToString() + ")";
                    Color playerCol = Color.White;

                    if (gamer.IsReady)
                    {
                        name += " -- Ready!";
                    }
                    if ((gamer.Tag as Human).GetPickedTeam())
                    {
                        int nTeam = (gamer.Tag as Human).GetTeam();

                        if (nTeam == 1)
                            playerCol = Color.Red;
                        else
                            playerCol = Color.Blue;
                    }

                    spriteBatch.DrawString(text, name, lastPos + new Vector2(0, 30), playerCol);
                    lastPos.Y += 30;
                }
                spriteBatch.DrawString(text, "Press E for red team,\nL for blue team.\nLeft and right arrows to\ncycle ship selection.", readyButtonPos + new Vector2(0, readyButton.Height + 10), Color.White);
            }
            else if (currentMenu == searchLobbies)
            {
                Vector2 lastPos = new Vector2(10, banner.Height + 40);
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);
                spriteBatch.DrawString(text, "Available games (J to join): " + availableSessions.Count.ToString(), lastPos, Color.White);

                for (int sessionIndex = 0; sessionIndex < availableSessions.Count; sessionIndex++)
                {
                    Color color = Color.Blue;

                    if (sessionIndex == selectedSessionIndex)
                        color = Color.Yellow;

                    spriteBatch.DrawString(text, availableSessions[sessionIndex].HostGamertag, lastPos + new Vector2(0, 30), color);
                    lastPos.Y += 30;
                }
            }
            else if (currentMenu == victoryLobby)
            {
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);

                Vector2 pos = bannerPosition + new Vector2(50, banner.Height + 10);
                for (int i = 0; i < victoryText.Count; i++)
                {
                    spriteBatch.DrawString(text, victoryText[i], pos, Color.White);
                    if (i < 2)
                        pos.Y += 20;
                    else
                        pos.Y += 40;
                }
            }
        }

        private void cleanAvailableSessions()
        {
            if (availableSessions != null)
            {
                availableSessions.Dispose();
                availableSessions = null;
            }
        }

        public void SetVictoryLobby()
        {
            victoryText = new List<string>();

            victoryText.Add(GameObject.Instance().getVictoryText());

            foreach (NetworkGamer ng in NetworkObject.Instance().getNetworksession().AllGamers)
            {
                victoryText.Add("");
                victoryText.Add((ng.Tag as Human).GetStats());               
            }

            currentMenu = victoryLobby;
            active = true;

            foreach(LocalNetworkGamer loc in NetworkObject.Instance().getNetworksession().LocalGamers)
            {
                if(loc.IsHost)
                    NetworkObject.Instance().getNetworksession().EndGame();
            }
        }

        //Receive network data for the lobby screen. Shows player choices and readiness.
        private void ReceiveNetworkData()
        {

            foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
            {
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;
                    gamer.ReceiveData(packetReader, out sender);

                    if (!sender.IsLocal)
                    {
                        Human senderHuman = sender.Tag as Human;

                        //This should be the same as was is sent in the send function.
                        senderHuman.SetTeam(packetReader.ReadInt32());
                        senderHuman.SetSelectedShip(packetReader.ReadString());

                    }
                }
            }

        }

        //Send the network data needed to show the selection between players in the lobby menu.
        private void SendNetworkData()
        {
            try
            {
                foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
                {
                    Human me = gamer.Tag as Human;

                    //This should be the same as is read in the read function.
                    packetWriter.Write(me.GetTeam());
                    packetWriter.Write(me.GetShipSelection());

                    gamer.SendData(packetWriter, SendDataOptions.None);
                }
            }
            catch (NullReferenceException)
            {
                //Debug!
            }
        }
    }
}

