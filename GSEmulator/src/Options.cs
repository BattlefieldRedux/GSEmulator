using System;
using System.Net;

namespace GSEmulator
{
    class Options
    {
        public IPAddress EmulatorIP { get; private set; }
        public int EmulatorPort { get; private set; }
        public IPAddress BattlefieldIP { get; private set; }
        public int BattlefieldPort { get; private set; }


        private Options()
        {
            // Do Nothing
        }


        public static Options FromCommandLine(string[] args)
        {
            IPAddress emulatorIP = IPAddress.Loopback;
            int emulatorPort = -1;
            IPAddress battlefieldIP = IPAddress.Loopback;
            int battlefieldPort = -1;

            // Parse the commands
            for (int i = 0; i < args.Length; i += 2)
            {

                string flag = args[i];
                string value = args[i + 1];

                switch (flag)
                {
                    case "-ip":
                        if (!IPAddress.TryParse(value, out emulatorIP))
                        {
                            throw new ParsingException("Invalid IP Address: '{0}'", value);
                        }
                        break;

                    case "-port":
                        if (!int.TryParse(value, out emulatorPort))
                        {
                            throw new ParsingException("Invalid Port: '{0}'", value);
                        }
                        break;

                    case "-bf2ip":
                    case "-bfip":
                        if (!IPAddress.TryParse(value, out battlefieldIP))
                        {
                            throw new ParsingException("Invalid Battlefield 2 IP Address: '{0}'", value);
                        }
                        break;

                    case "-bf2port":
                    case "-bfport":
                        if (!int.TryParse(value, out battlefieldPort))
                        {
                            throw new ParsingException("Invalid Battlefield 2 Port: '{0}'", value);
                        }
                        break;
                }
            }

            if (emulatorPort < 0)
                throw new ParsingException("Missing configuration: -port");

            if (battlefieldPort < 0)
                throw new ParsingException("Missing configuration: -bfport");

            return new Options()
            {
                EmulatorIP = emulatorIP,
                EmulatorPort = emulatorPort,
                BattlefieldIP = battlefieldIP,
                BattlefieldPort = battlefieldPort
            };
        }


        public static string ShowUsage()
        {

            string header = "GSEmulator.exe <options>:";
            string EmulatorIP = String.Format("{0, -20}{1, -10}{2}", "-ip <ip address>", "", "Defines the IP where this emulater will listen from");
            string EmulatorPort = String.Format("{0, -20}{1, -10}{2}", "-port <port>", "REQUIRED", "Defines the Port where this emulater will listen from");
            string BattlefieldIP = String.Format("{0, -20}{1, -10}{2}", "-bfip <ip address>", "", "Defines the IP where the game is listening from");
            string BattlefieldPort = String.Format("{0, -20}{1, -10}{2}", "-bfport <port>", "REQUIRED", "Defines the port where the game is listening from");

            return String.Format("USAGE: {0}\n  Options:\n  {1}\n  {2}\n  {3}\n  {4}", header, EmulatorIP, EmulatorPort, BattlefieldIP, BattlefieldPort);
        }


        public class ParsingException : Exception
        {
            public ParsingException(string message) : base(message)
            {
                // Do nothing
            }

            public ParsingException(string format, params object[] args) : base(String.Format(format, args))
            {
                // Do nothing
            }
        }
    }
}
