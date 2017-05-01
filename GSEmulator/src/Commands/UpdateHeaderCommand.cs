using GSEmulator.Model;
using GSEmulator.Util;
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
            if (tokens.Length != 2) {
                Log.e("UpdateHeaderCommand", "Called UpdateHeaderCommand w/ wrong arguments");
                throw new ArgumentOutOfRangeException("UpdateHeaderCommand expects 2 tokens '<key> <value>'");
            }

            mKey = tokens[0];
            mValue = tokens[1];
        }

        public override void Execute(ref Server server)
        {
            for (int fieldIdx = 0; fieldIdx < Server.NumFields; fieldIdx++)
            {
                if (Server.GetFieldName(fieldIdx) == mKey)
                {
                    Log.d("UpdatePlayerCommand", String.Format("Executing key  {0} at {1} with '{0}'", mKey, fieldIdx, mValue));
                    server.SetFieldValue(fieldIdx, mValue);
                    break;
                }
            }
        }
    }
}
