using GSEmulator.Model;
using System;

namespace GSEmulator.Commands
{
    class AddPlayerCommand : Command
    {
        private string[] tokens;

        public AddPlayerCommand(string[] tokens)
        {
            this.tokens = tokens;
        }

        public override void Execute(ref Server server)
        {
            throw new NotImplementedException();
        }

    }
}
