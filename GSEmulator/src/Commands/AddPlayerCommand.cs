using GSEmulator.Model;
using System;

namespace GSEmulator.Commands
{

    class AddPlayerCommand : Command
    {
        private string[] tokens;
        private string mName;
        private uint mPid;
        private int mPlayerIdx;

        // player_a \00 <id> \00 <name> \00 <pid> \0A
        public AddPlayerCommand(string[] tokens)
        {
            this.tokens = tokens;
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
