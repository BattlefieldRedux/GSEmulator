using GSEmulator.Model;
using GSEmulator.Util;
using System;

namespace GSEmulator.Commands
{

    class AddPlayerCommand : Command
    {
        private string mName;
        private uint mPid;
        private int mPlayerIdx;

        // player_a \00 <id> \00 <name> \00 <pid> \0A
        public AddPlayerCommand(string[] tokens)
        {
            if (tokens.Length != 4)
            {
                Log.e("AddPlayerCommand", "Called AddPlayerCommand w/ wrong arguments");
                throw new ArgumentOutOfRangeException("AddPlayerCommand expects 4 tokens 'player_a <id> <name> <pid>'");
            }
                

            mPlayerIdx = Int32.Parse(tokens[1]);
            mName = tokens[2];
            mPid = UInt32.Parse(tokens[3]);
        }

        public override void Execute(ref Server server)
        {
            Player player = new Model.Player(mName, mPid);
            server.GetPlayers()[mPlayerIdx] = player;
        }

    }
}
