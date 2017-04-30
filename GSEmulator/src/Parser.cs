using GSEmulator.Commands;
using System;

namespace GSEmulator
{
    class Parser
    {

       public static Command Parse(String line) {

            String[] tokens = line.Split((char)0x00);

            switch (tokens[0])
            {
                case "player_u":
                    return new UpdatePlayerCommand(tokens);
                case "player_a":
                    return new AddPlayerCommand(tokens);
                case "player_r":
                    return new RemovePlayerCommand(tokens);
                default:
                    return new UpdateHeaderCommand(tokens);
            }
        }


    }
}
