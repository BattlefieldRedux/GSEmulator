using GSEmulator.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSEmulator.Commands
{
    class UpdatePlayerCommand : Command
    {
        int PlayerID;
        string Field;
        string Value;

        public UpdatePlayerCommand(string[] tokens) : base() {
        }

        public override void Execute(ref Server server)
        {
            throw new NotImplementedException();
        }
    }
}
