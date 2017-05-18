using GSEmulator.Model;
using NLog;
using System;

namespace GSEmulator.Commands
{

    class AddPlayerCommand : Command
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        private string mName;
        private uint mPid;
        private int mPlayerIdx;
        private bool mIsBot;

        // player_a \00 <id> \00 <name> \00 <pid> \00 <isBot> \0A
        public AddPlayerCommand(string[] tokens)
        {
            if (tokens.Length != 5)
            {
                LOGGER.Fatal("Called AddPlayerCommand w/ wrong arguments");
                throw new ArgumentOutOfRangeException("AddPlayerCommand expects 4 tokens 'player_a <id> <name> <pid> <isBot>'");
            }


            mPlayerIdx = Int32.Parse(tokens[1]);
            mName = tokens[2];
            mPid = UInt32.Parse(tokens[3]);
            mIsBot = tokens[4] == "1";
        }

        public override void Execute(ref Server server)
        {
            Player player = new Player(mName, mPid, mIsBot);
            server.GetPlayers()[mPlayerIdx] = player;
        }

    }
}
