using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSEmulator.Model
{
    class Player
    {
        public static readonly int NumFields = 8;
        public static readonly string NAME_KEY = "player_";
        public static readonly string PING_KEY = "ping_";
        public static readonly string TEAM_KEY = "team_";
        public static readonly string SKILL_KEY = "skill_";
        public static readonly string SCORE_KEY = "score_";
        public static readonly string DEATHS_KEY = "deaths_";
        public static readonly string PID_KEY = "pid_";
        public static readonly string AIBot_KEY = "AIBot_";

        public string Name { get; set; }
        public uint Pid { get; set; }
        public bool IsBot { get; set; }
        public int Score { get; set; }
        public uint Kills { get; set; }
        public uint Deaths { get; set; }
        public uint Team { get; set; }
        public uint Ping { get; set; }

        public string GetField(int field)
        {
            switch (field)
            {
                case 0:
                    return Name;
                case 1:
                    return Score.ToString();
                case 2:
                    return Ping.ToString();
                case 3:
                    return Team.ToString();
                case 4:
                    return Deaths.ToString();
                case 5:
                    return Pid.ToString();
                case 6:
                    return Kills.ToString(); 
                case 7:
                    return (IsBot ? 1 : 0).ToString();
                default:
                    return "";
            }
        }

        public static string GetFieldName(int field)
        {
            switch (field)
            {
                case 0:
                    return NAME_KEY;
                case 1:
                    return SCORE_KEY;
                case 2:
                    return PING_KEY;
                case 3:
                    return TEAM_KEY;
                case 4:
                    return DEATHS_KEY;
                case 5:
                    return PID_KEY;
                case 6:
                    return SKILL_KEY;
                case 7:
                    return AIBot_KEY;
                default:
                    return "";
            }
        }
    }
}
