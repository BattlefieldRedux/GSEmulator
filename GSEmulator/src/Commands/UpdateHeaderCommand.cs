using GSEmulator.Model;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSEmulator.Commands
{
    class UpdateHeaderCommand : Command
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private string mKey;
        private string mValue;

        //	key \00 value \0A
        public UpdateHeaderCommand(string[] tokens) : base()
        {
            if (tokens.Length != 2) {
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
                    LOGGER.Debug("Updating header '{0}' with '{1}'", mKey, mValue);
                    server.SetFieldValue(fieldIdx, mValue);
                    break;
                }
            }
        }
    }
}
