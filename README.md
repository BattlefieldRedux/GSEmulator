# GSEmulator
GSEmulator is an (incomplete) implementation of the Gamespy QR2 (Query and Report 2) module which allows to expand and/or modify the information that is reported and querying a game server. It maintains a state of the game by receiving updates (via python) directly from the game.

## How it works
Clients query the game server as usual but these UDP packets get intercepted and redirected to GSEmulator which then replies ( instead of the game).

## Installation
### Windows
In windows the intercepting part is done with [GSDiverter](https://github.com/UTurista/GSDiverter).

### Linux
In Linux, the interception can be done using iptables:

`sudo iptables -t nat -A PREROUTING -p udp --dport <GAME_QUERY_PORT> -j REDIRECT --to-port <GSEMULATOR_PORT>`
,where:
- `GAME_QUERY_PORT` is the port number used by the game to handle query requests
- `GSEMULATOR_PORT` is the port number binded to the emulator.

## Usage
```
GSEmulator.exe <options>:
  Options:
  -ip <ip address>              Defines the IP where this emulater will listen from
  -port <port>        REQUIRED  Defines the Port where this emulater will listen from
  -bfip <ip address>            Defines the IP where the game is listening from
  -bfport <port>      REQUIRED  Defines the port where the game is listening from
```
