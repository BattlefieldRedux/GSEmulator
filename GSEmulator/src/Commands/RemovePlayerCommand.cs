using GSEmulator.Model;
using NLog;
using System;


namespace GSEmulator.Commands
{
    class RemovePlayerCommand : Command
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private int mPlayerIdx;

        // player_r \00 <id> \0A
        public RemovePlayerCommand(string[] tokens)
        {
            if (tokens.Length != 2)
            {
                LOGGER.Fatal("Called RemovePlayerCommand w/ wrong arguments");
                throw new ArgumentOutOfRangeException("RemovePlayerCommand expects 2 tokens 'player_r <id>'");
            }
               

            mPlayerIdx = Int32.Parse(tokens[1]);
        }

        public override void Execute(ref Server server)
        {
            var players = server.GetPlayers();
            if (players.Length > mPlayerIdx)
                players[mPlayerIdx] = null;
        }
    }
}
