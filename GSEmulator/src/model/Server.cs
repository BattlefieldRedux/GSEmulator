using System;
using System.Globalization;
using System.Linq;

namespace GSEmulator.Model
{
    class Server
    {
        public static readonly int NumFields = 45;
        private static readonly int MAX_PLAYER_INDEX = 256; // the maximum index value a player may have
        private static readonly int MAX_TEAM_SIZE = 2;

        public const string HOST_NAME = "hostname";
        public const string GAME_NAME = "gamename";
        public const string GAME_VERSION = "gamever";
        public const string MAP_NAME = "mapname";
        public const string GAME_TYPE = "gametype";
        public const string GAME_VARIANT = "gamevariant";
        public const string NUM_PLAYERS = "numplayers";
        public const string MAX_PLAYERS = "maxplayers";
        public const string GAME_MODE = "gamemode";                 // eg: openplaying
        public const string PASSWORD = "password";
        public const string TIME_LIMIT = "timelimit";
        public const string ROUND_TIME = "roundtime";
        public const string HOST_PORT = "hostport";
        public const string DEDICATED = "bf2_dedicated";
        public const string RANKED = "bf2_ranked";
        public const string ANTI_CHEAT = "bf2_anticheat";
        public const string OPERATIVE_SYSTEM = "bf2_os";
        public const string BATTLE_RECORDER = "bf2_autorec";
        public const string BATTLE_RECORDER_INDICE = "bf2_d_idx";
        public const string BATTLE_RECORDER_DOWNLOAD = "bf2_d_dl";
        public const string VOIP = "bf2_voip";
        public const string AUTO_BALANCE = "bf2_autobalanced";
        public const string FRIENDLY_FIRE = "bf2_friendlyfire";
        public const string TK_MODE = "bf2_tkmode";
        public const string START_DELAY = "bf2_startdelay";
        public const string SPAWN_TIME = "bf2_spawntime";
        public const string SPONSOR_TEXT = "bf2_sponsortext";
        public const string SPONSOR_LOGO_URL = "bf2_sponsorlogo_url";
        public const string COMMUNITY_LOGO_URL = "bf2_communitylogo_url";
        public const string SCORE_LIMIT = "bf2_scorelimit";
        public const string TICKET_RATIO = "bf2_ticketratio";
        public const string TEAM_RATIO = "bf2_teamratio";
        public const string TEAM_1_NAME = "bf2_team1";                          // Opfor Team
        public const string TEAM_2_NAME = "bf2_team2";                          // Blufor Team
        public const string BOTS = "bf2_bots";
        public const string PURE = "bf2_pure";
        public const string MAP_SIZE = "bf2_mapsize";
        public const string UNLOCKS = "bf2_globalunlocks";
        public const string FPS = "bf2_fps";
        public const string PLASMA = "bf2_plasma";
        public const string RESERVED_SLOTS = "bf2_reservedslots";
        public const string BOT_RATIO = "bf2_coopbotratio";
        public const string BOT_COUNT = "bf2_coopbotcount";
        public const string BOT_DIFFICULTY = "bf2_coopbotdiff";
        public const string NO_VEHICLES = "bf2_novehicles";
        //
        // ------------------------------
        // Properties
        public string IPAddress { get; set; }
        public uint QueryPort { get; set; }
        public string Country { get; set; }
        public string HostName { get; set; }
        public string GameName { get; set; }
        public string GameVersion { get; set; }
        public uint HostPort { get; set; }
        public string GameVariant { get; set; }
        public string MapName { get; set; }
        public string GameType { get; set; }
        public uint MapSize { get; set; }
        public uint NumPlayers
        {
            get
            {
                return (uint)this.Players
                    .Count((p) =>  { return p != null && p.IsBot == false; });
            }
        }
        public uint MaxPlayers { get; set; }
        public uint ReservedSlots { get; set; }
        public string GameMode { get; set; }
        public bool Password { get; set; }
        public uint TimeLimit { get; set; }
        public uint RoundTime { get; set; }
        public string OS { get; set; }
        public bool Dedicated { get; set; }
        public bool Ranked { get; set; }
        public bool AntiCheat { get; set; }
        public bool BattleRecorder { get; set; }
        public string BRIndex { get; set; }
        public string BRDownload { get; set; }
        public bool Voip { get; set; }
        public bool AutoBalance { get; set; }
        public bool FriendlyFire { get; set; }
        public string TKMode { get; set; }
        public uint StartDelay { get; set; }
        public float SpawnTime { get; set; }
        public string ServerText { get; set; }
        public string ServerLogo { get; set; }
        public string CommunityWebsite { get; set; }
        public uint ScoreLimit { get; set; }
        public uint TicketRatio { get; set; }
        public uint TeamRatio { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public bool CoopEnabled { get; set; }
        public bool Pure { get; set; }
        public bool Unlocks { get; set; }
        public uint Fps { get; set; }
        public bool Plasma { get; set; }
        public uint CoopBotRatio { get; set; }
        public uint CoopBotCount { get; set; }
        public uint CoopBotDifficulty { get; set; }
        public bool NoVehicles { get; set; }
        public Player[] Players;
        public Team[] Teams;


        public Server()
        {
            Players = new Player[MAX_PLAYER_INDEX];
            Teams = new Team[MAX_TEAM_SIZE];
            Teams[0] = new Team();
            Teams[1] = new Team();
        }

        public Player[] GetPlayers()
        {
            return Players;
        }

        public Team[] GetTeams()
        {
            return Teams;
        }

        public string GetField(int field)
        {
            switch (field)
            {
                case 0: return HostName;
                case 1: return GameName;
                case 2: return GameVersion;
                case 3: return MapName;
                case 4: return GameType;
                case 5: return GameVariant;
                case 6: return NumPlayers.ToString();
                case 7: return MaxPlayers.ToString();
                case 8: return GameMode;
                case 9: return (Password ? 1 : 0).ToString();
                case 10: return TimeLimit.ToString();
                case 11: return RoundTime.ToString();
                case 12: return HostPort.ToString();
                case 13: return (Dedicated ? 1 : 0).ToString();
                case 14: return (Ranked ? 1 : 0).ToString();
                case 15: return (AntiCheat ? 1 : 0).ToString();
                case 16: return OS;
                case 17: return (BattleRecorder ? 1 : 0).ToString();
                case 18: return BRIndex;
                case 19: return BRDownload;
                case 20: return (Voip ? 1 : 0).ToString();
                case 21: return (AutoBalance ? 1 : 0).ToString();
                case 22: return (FriendlyFire ? 1 : 0).ToString();
                case 23: return TKMode;
                case 24: return StartDelay.ToString();
                case 25: return SpawnTime.ToString("0.000000", CultureInfo.InvariantCulture);
                case 26: return ServerText;
                case 27: return ServerLogo;
                case 28: return CommunityWebsite;
                case 29: return ScoreLimit.ToString();
                case 30: return TicketRatio.ToString();
                case 31: return TeamRatio.ToString("0.000000", CultureInfo.InvariantCulture);
                case 32: return Team1Name;
                case 33: return Team2Name;
                case 34: return (CoopEnabled ? 1 : 0).ToString();
                case 35: return (Pure ? 1 : 0).ToString();
                case 36: return MapSize.ToString();
                case 37: return (Unlocks ? 1 : 0).ToString();
                case 38: return Fps.ToString();
                case 39: return (Plasma ? 1 : 0).ToString();
                case 40: return ReservedSlots.ToString();
                case 41: return CoopBotRatio.ToString();
                case 42: return CoopBotCount.ToString();
                case 43: return CoopBotDifficulty.ToString();
                case 44: return (NoVehicles ? 1 : 0).ToString();
                default:
                    return "";
            }
        }


        public void SetFieldValue(int fieldIdx, string value)
        {
            switch (fieldIdx)
            {
                case 0: HostName = value; break;
                case 1: GameName = value; break;
                case 2: GameVersion = value; break;
                case 3: MapName = value; break;
                case 4: GameType = value; break;
                case 5: GameVariant = value; break;
                //case 6: NumPlayers = UInt32.Parse(value); break;
                case 7: MaxPlayers = UInt32.Parse(value); break;
                case 8: GameMode = value; break;
                case 9: Password = value == "1" ? true : false; break;
                case 10: TimeLimit = UInt32.Parse(value); ; break;
                case 11: RoundTime = UInt32.Parse(value); ; break;
                case 12: HostPort = UInt32.Parse(value); ; break;
                case 13: Dedicated = value == "1" ? true : false; break;
                case 14: Ranked = value == "1" ? true : false; break;
                case 15: AntiCheat = value == "1" ? true : false; break;
                case 16: OS = value; break;
                case 17: BattleRecorder = value == "1" ? true : false; break;
                case 18: BRIndex = value; break;
                case 19: BRDownload = value; break;
                case 20: Voip = value == "1" ? true : false; break;
                case 21: AutoBalance = value == "1" ? true : false; break;
                case 22: FriendlyFire = value == "1" ? true : false; break;
                case 23: TKMode = value; break;
                case 24: StartDelay = UInt32.Parse(value); ; break;
                case 25: SpawnTime = float.Parse(value); break;
                case 26: ServerText = value; break;
                case 27: ServerLogo = value; break;
                case 28: CommunityWebsite = value; break;
                case 29: ScoreLimit = UInt32.Parse(value); break;
                case 30: TicketRatio = UInt32.Parse(value); break;
                case 31: TeamRatio = UInt32.Parse(value); break;
                case 32: this.Teams[0].Name = Team1Name = value; break;
                case 33: this.Teams[1].Name = Team2Name = value; break;
                case 34: CoopEnabled = value == "1" ? true : false; break;
                case 35: Pure = value == "1" ? true : false; break;
                case 36: MapSize = UInt32.Parse(value); break;
                case 37: Unlocks = value == "1" ? true : false; break;
                case 38: Fps = UInt32.Parse(value); break;
                case 39: Plasma = value == "1" ? true : false; break;
                case 40: ReservedSlots = UInt32.Parse(value); break;
                case 41: CoopBotRatio = UInt32.Parse(value); break;
                case 42: CoopBotCount = UInt32.Parse(value); break;
                case 43: CoopBotDifficulty = UInt32.Parse(value); break;
                case 44: NoVehicles = value == "1" ? true : false; break;
            }
        }



        public static string GetFieldName(int field)
        {
            switch (field)
            {
                case 0: return HOST_NAME;
                case 1: return GAME_NAME;
                case 2: return GAME_VERSION;
                case 3: return MAP_NAME;
                case 4: return GAME_TYPE;
                case 5: return GAME_VARIANT;
                case 6: return NUM_PLAYERS;
                case 7: return MAX_PLAYERS;
                case 8: return GAME_MODE;
                case 9: return PASSWORD;
                case 10: return TIME_LIMIT;
                case 11: return ROUND_TIME;
                case 12: return HOST_PORT;
                case 13: return DEDICATED;
                case 14: return RANKED;
                case 15: return ANTI_CHEAT;
                case 16: return OPERATIVE_SYSTEM;
                case 17: return BATTLE_RECORDER;
                case 18: return BATTLE_RECORDER_INDICE;
                case 19: return BATTLE_RECORDER_DOWNLOAD;
                case 20: return VOIP;
                case 21: return AUTO_BALANCE;
                case 22: return FRIENDLY_FIRE;
                case 23: return TK_MODE;
                case 24: return START_DELAY;
                case 25: return SPAWN_TIME;
                case 26: return SPONSOR_TEXT;
                case 27: return SPONSOR_LOGO_URL;
                case 28: return COMMUNITY_LOGO_URL;
                case 29: return SCORE_LIMIT;
                case 30: return TICKET_RATIO;
                case 31: return TEAM_RATIO;
                case 32: return TEAM_1_NAME;
                case 33: return TEAM_2_NAME;
                case 34: return BOTS;
                case 35: return PURE;
                case 36: return MAP_SIZE;
                case 37: return UNLOCKS;
                case 38: return FPS;
                case 39: return PLASMA;
                case 40: return RESERVED_SLOTS;
                case 41: return BOT_RATIO;
                case 42: return BOT_COUNT;
                case 43: return BOT_DIFFICULTY;
                case 44: return NO_VEHICLES;
                default: return "";
            }
        }
    }
}
