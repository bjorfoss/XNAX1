﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using SpacePirates.Player;

namespace SpacePirates
{
    class NetworkObject : Microsoft.Xna.Framework.Game
    {
        private static NetworkObject instance;
        static readonly object padlock = new Object();

        private NetworkSession networkSession;
        private PacketReader packetReader = new PacketReader();
        private PacketWriter packetWriter = new PacketWriter();

        private bool networkEnabled = true;

        private Human player;

        public NetworkObject()
        {
            //Include Game services.
            //Components.Add(new GamerServicesComponent(this)); -- need to be in the main.

            // Respond to the SignedInGamer event
            SignedInGamer.SignedIn +=
                new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
        }

        public static NetworkObject Instance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new NetworkObject();
                }
                return instance;
            }
        }

        public void CreateSession()
        {
            int maxLocalGamers = 1;
            int maxGamers = 10;
            int privateGamerSlots = 2;
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, maxLocalGamers, maxGamers, privateGamerSlots, null);
            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = false;

            HookSessionEvents();
        }

        private void HookSessionEvents()
        {
            networkSession.GamerJoined +=
                new EventHandler<GamerJoinedEventArgs>(
                    networkSession_GamerJoined);
        }

        public NetworkSession getNetworksession()
        {
            return networkSession;
        }

        public void setNetworkSession(NetworkSession session)
        {
            if (session.SessionState == NetworkSessionState.Lobby)
            {
                networkSession = session;
                HookSessionEvents();
            }
        }

        public void disposeNetworkSession()
        {
            networkSession.Dispose();
            networkSession = null;
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            e.Gamer.Tag = new Human(e.Gamer.Gamertag);
            player = e.Gamer.Tag as Human;
        }

        void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (!e.Gamer.IsLocal)
            {
                e.Gamer.Tag = new Human(e.Gamer.Gamertag);
            }
            else
            {
                e.Gamer.Tag = GetHuman(e.Gamer.Gamertag);
            }
        }

        Human GetHuman(String gamertag)
        {
            foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)     
            {
                if (signedInGamer.Gamertag == gamertag)
                {
                    return signedInGamer.Tag as Human;
                }
            }

            return new Human("Player");
        }

        public void testConnection()
        {
            try
            {
                NetworkSession test = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 8, 2, null);
                test.Dispose();
                test = null;
            }
            catch (NetworkNotAvailableException)
            {
                networkEnabled = false;
            }
            catch (GamerPrivilegeException)
            {
                networkEnabled = false;
            }
        }

        public bool getNetworked()
        {
            return networkEnabled; 
        }

        public Human getPlayer()
        {
            return player;
        }
    }
}
