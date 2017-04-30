using GSEmulator.Model;
using System;
using System.Collections.Generic;

namespace GSEmulator.GSProtocol3
{
    class Encoder
    {
        private Server mServer;
        private List<Message> mMessages;
        private Message mMessage;
        private byte[] mTimeStamp;

        public Encoder(Server server, byte[] timestamp)
        {
            mMessages = new List<Message>();
            mServer = server;
            mTimeStamp = timestamp;

        }

        public List<Message> Encode(bool headers, bool players, bool teams)
        {
            newMessage();

            if (headers)
            {
                int headerIdx = 0;
                while (!EncodeHeaders(ref headerIdx))
                    newMessage();
            }

            if (players)
            {
                int playerIdx = 0;
                int fieldIdx = 0;
                while (!EncodePlayers(ref playerIdx, ref fieldIdx))
                    newMessage();
            }

            if (teams)
            {
                int teamIdx = 0;
                int fieldIdx = 0;
                while (!EncodeTeams(ref teamIdx, ref fieldIdx))
                    newMessage();
            }

            mMessage.Index = (byte)mMessages.Count;
            mMessages.Add(mMessage);
            mMessage.IsLast = true;
            return mMessages;
        }

        private void newMessage()
        {
            Console.WriteLine("newMessage");
            if (mMessage != null)
            {

                mMessage.Index = (byte)mMessages.Count;
                mMessages.Add(mMessage);
            }

            mMessage = new Message(mTimeStamp);
        }

        private bool EncodeHeaders(ref int headerIdx)
        {
            Console.WriteLine("EncodeHeaders");
            mMessage.put((byte)Message.TYPE_HEADERS);

            for (; headerIdx < Server.NumFields; headerIdx++)
            {
                var field = Server.GetFieldName(headerIdx);
                var value = mServer.GetField(headerIdx);
                Console.WriteLine(String.Format(" {0} => {1}", field, value));

                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)
                    return false;

                if (mMessage.put(value) != value.Length || mMessage.put(0) != 1)
                    return false;
            }

            return mMessage.put(0) == 1;
        }

        private bool EncodePlayers(ref int playerIdx, ref int fieldIdx)
        {
            Console.WriteLine("EncodPlayers");
            mMessage.put((byte)Message.TYPE_PLAYERS);

            var players = mServer.GetPlayers();
            for (; fieldIdx < Player.NumFields; playerIdx = 0, fieldIdx++)
            {
                var field = Player.GetFieldName(fieldIdx);

                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)
                    return false;

                if (mMessage.put((byte)playerIdx) != 1)
                    return false;


                for (; playerIdx < players.Count; playerIdx++)
                {
                    var value = players[playerIdx].GetField(fieldIdx);

                    if (mMessage.put(value) != value.Length || mMessage.put(0) != 1)
                        return false;
                }

                if (mMessage.put(0) != 1)
                    return false;
            }
            return mMessage.put(0) == 1;
        }

        private bool EncodeTeams(ref int teamIdx, ref int fieldIdx)
        {
            Console.WriteLine("EncodTeams");
            mMessage.put((byte)Message.TYPE_TEAM);

            var players = mServer.GetTeams();
            for (; fieldIdx < Team.NumFields; teamIdx = 0, fieldIdx++)
            {
                var field = Team.GetFieldName(fieldIdx);
                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)
                    return false;

                if (mMessage.put((byte)teamIdx) != 1)
                    return false;


                for (; teamIdx < players.Count; teamIdx++)
                {
                    var value = players[teamIdx].GetField(fieldIdx);

                    if (mMessage.put(value) != value.Length || mMessage.put(0) != 1)
                        return false;
                }

                if (mMessage.put(0) != 1)
                    return false;
            }
            return mMessage.put(0) == 1;
        }
    }
}
