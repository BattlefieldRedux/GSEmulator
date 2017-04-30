using System;
using GSEmulator.Model;

namespace GSEmulator.Commands
{
    class UpdateHeaderCommand : Command
    {
        private string[] tokens;

        public UpdateHeaderCommand(string[] tokens)
        {
            this.tokens = tokens;
        }

        public override void Execute(ref Server server)
        {
            throw new NotImplementedException();
        }
    }
}
