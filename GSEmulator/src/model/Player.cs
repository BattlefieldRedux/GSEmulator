﻿using System;

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

        public string Name { get; private set; }
        public uint Pid { get; private set; }
        public bool IsBot { get; set; }
        public int Score { get; set; }
        public uint Kills { get; set; }
        public uint Deaths { get; set; }
        public uint Team { get; set; }
        public uint Ping { get; set; }


        public Player(string name, uint pid, bool isbot) {
            Name = name;
            Pid = pid;
            IsBot = isbot;
        }


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

        public void SetFieldValue(int fieldIdx, string value)
        {
            switch (fieldIdx)
            {
                case 0:
                    Name = value;
                    break;
                case 1:
                    Score = Int32.Parse(value);
                    break;
                case 2:
                    Ping = UInt32.Parse(value);
                    break;
                case 3:
                    Team = UInt32.Parse(value);
                    break;
                case 4:
                    Deaths = UInt32.Parse(value);
                    break;
                case 5:
                    Pid = UInt32.Parse(value);
                    break;
                case 6:
                    Kills = UInt32.Parse(value);
                    break;
                case 7:
                    IsBot = value == "1" ? true : false;
                    break;
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
