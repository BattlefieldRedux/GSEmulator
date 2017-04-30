
using GSEmulator.Model;

namespace GSEmulator.Commands
{
    abstract class Command
    {
        public abstract void Execute(ref Server server);

    }
}