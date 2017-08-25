using GSEmulator.Commands;
using System;

namespace GSEmulator
{
    class Parser
    {

       public static Command Parse(String line) {
            
            if(string.IsNullOrEmpty(line))
                return new VoidCommand();

            string[] tokens = line.Split((char)0x00);

            if( string.IsNullOrEmpty(tokens[0]))
                return new VoidCommand();

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
