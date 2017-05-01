using GSEmulator.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSEmulator.Commands
{
    class UpdateHeaderCommand : Command
    {
        private string mKey;
        private string mValue;

        //	key \00 value \0A
        public UpdateHeaderCommand(string[] tokens) : base()
        {
            if (tokens.Length != 4)
                throw new ArgumentOutOfRangeException("UpdateHeaderCommand expects 2 tokens '<key> <value>'");

            mKey = tokens[0];
            mValue = tokens[1];
        }

        public override void Execute(ref Server server)
        {
            for (int fieldIdx = 0; fieldIdx < Server.NumFields; fieldIdx++)
            {
                if (Server.GetFieldName(fieldIdx) == mKey)
                {
                    server.SetFieldValue(fieldIdx, mValue);
                    break;
                }
            }
        }
    }
}
