using GSEmulator.Model;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace GSEmulator.GSProtocol3
{
    class Encoder
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// Encodes the server associated with this encoder into the proper GameSpy format.
        /// 
        /// The encoding process allows selecting which information should be included, server headers, players and team information.
        /// </summary>
        /// <param name="headers">If true, server headers will be encodend in the message</param>
        /// <param name="players">If true, players information will be encodend in the message</param>
        /// <param name="teams">If true, teams information will be encodend in the message</param>
        /// <returns>A list of messages representing the gamespy encoded messages</returns>
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

            LOGGER.Debug("Server encoded!");
            mMessage.Index = (byte)mMessages.Count;
            mMessages.Add(mMessage);
            mMessage.IsLast = true;
            return mMessages;
        }

        private void newMessage()
        {
            LOGGER.Debug("Creating new message");

            if (mMessage != null)
            {
                mMessage.Index = (byte)mMessages.Count; // Update previous message index
                mMessages.Add(mMessage);
            }

            mMessage = new Message(mTimeStamp);
        }

        /// <summary>
        /// Encodes the server's header into gamespy format:
        ///  0x00 <key 1> 0x00 <value 1> 0x00 ... <key N> 0x00 <value N> 0x00  0x00
        /// 
        /// Where the first byte, 0x00, indicates we're handling with the server's headers
        ///
        /// </summary>
        /// <param name="headerIdx">The header's index to start enconding by</param>
        /// <returns>true if all headers were encoded else false</returns>
        private bool EncodeHeaders(ref int headerIdx)
        {
            LOGGER.Debug("EncodeHeaders");
            mMessage.put((byte)Message.TYPE_HEADERS);                               // We indicate we're writing server's headers                     

            for (; headerIdx < Server.NumFields; headerIdx++)
            {
                var field = Server.GetFieldName(headerIdx);
                var value = mServer.GetField(headerIdx);
                LOGGER.Trace(" {0} => {1}", field, value);

                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)    // We attempt to write the key and terminate it with 0x00
                    return false;

                if (value != null && mMessage.put(value) != value.Length)           // We attempt to write the value
                    return false;

                if (mMessage.put(0) != 1)                                           // we attempt to terminate the value with 0x00
                    return false;
            }

            return mMessage.put(0) == 1;                                            // All headers written, we terminate it with 0x00
        }

        /// <summary>
        /// Encodes the player's information into gamespy format:
        ///  0x01 <key 1> 00 <offset> <value 1 + offset> 0x00 ... <value NN> 0x00 0x00 <key 2> 0x00 <offset> <value 1 + offset> 0x00 ... <value NN> 0x00 0x00 0x00
        /// 
        /// Where the first byte, 0x01, indicates we're handling with the player's information
        /// </summary>
        /// <param name="playerIdx"></param>
        /// <param name="fieldIdx"></param>
        /// <returns></returns>
        private bool EncodePlayers(ref int playerIdx, ref int fieldIdx)
        {
            LOGGER.Debug("EncodePlayers");
            mMessage.put((byte)Message.TYPE_PLAYERS);

            var players = mServer
                            .GetPlayers()
                            .Where((player) => player != null)                      // We convert our sparced array into a dense one
                            .ToArray(); 

            for (; fieldIdx < Player.NumFields; playerIdx = 0, fieldIdx++)
            {
                var field = Player.GetFieldName(fieldIdx);

                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)    // We attempt to write the key and terminate it with 0x00
                    return false;

                if (mMessage.put((byte)playerIdx) != 1)                             // We attempt to write the value index
                    return false;


                for (; playerIdx < players.Length; playerIdx++)                       
                {
                    var value = players[playerIdx].GetField(fieldIdx);              

                    if (value != null && mMessage.put(value) != value.Length)       // We attempt to write the value
                        return false;

                    if (mMessage.put(0) != 1)                                       // And terminate it with 0x00
                        return false;
                }

                if (mMessage.put(0) != 1)                                           // All values for this key are written, we terminate it with 0x00
                    return false;
            }
            return mMessage.put(0) == 1;                                            // All values for all keys are written, we terminate it with 0x00
        }

        //TODO: EncodeTeams is the same as Encodedlayers! Refactor to make a single method
        private bool EncodeTeams(ref int teamIdx, ref int fieldIdx)
        {
            LOGGER.Debug("EncodeTeams");
            mMessage.put((byte)Message.TYPE_TEAM);

            var players = mServer.GetTeams();
            for (; fieldIdx < Team.NumFields; teamIdx = 0, fieldIdx++)
            {
                var field = Team.GetFieldName(fieldIdx);
                if (mMessage.put(field) != field.Length || mMessage.put(0) != 1)
                    return false;

                if (mMessage.put((byte)teamIdx) != 1)
                    return false;

                for (; teamIdx < players.Length; teamIdx++)
                {
                    var value = players[teamIdx].GetField(fieldIdx);

                    if (value != null && mMessage.put(value) != value.Length)
                        return false;

                    if (mMessage.put(0) != 1)
                        return false;
                }

                if (mMessage.put(0) != 1)
                    return false;
            }
            return mMessage.put(0) == 1;
        }
    }
}
