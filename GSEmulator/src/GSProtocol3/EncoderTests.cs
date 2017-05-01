using GSEmulator.Model;

namespace GSEmulator.GSProtocol3
{
    class EncoderTests
    {
        
        public static Server CreateServer(uint players)
        {
            Server server = new Server();
            server.IPAddress = "192.168.1.75";
            server.QueryPort = 29901;
            server.HostName = "[PR v1.4.9.0] Project Reality Server";
            server.GameName = "battlefield2";
            server.GameVersion = "1.5.3153-802.0";
            server.HostPort = 16567;
            server.GameVariant = "pr";
            server.MapName = "Asad Khal";
            server.GameType = "gpm_cq";
            server.MapSize = 16;
            server.NumPlayers = players;
            server.MaxPlayers = 100;
            server.ReservedSlots = 0;
            server.GameMode = "openplaying";
            server.Password = false;
            server.TimeLimit = 14400;
            server.RoundTime = 1;
            server.OS = "win32";
            server.Dedicated = true;
            server.Ranked = false;
            server.AntiCheat = false;
            server.BattleRecorder = false;
            server.BRIndex = "http://";
            server.BRDownload = "http://";
            server.Voip = true;
            server.AutoBalance = false;
            server.FriendlyFire = true;
            server.TKMode = "Punish";
            server.StartDelay = 30;
            server.SpawnTime = 300;
            server.ServerText = "Welcome to a Project Reality server!|Asad Khal AAS Alt";
            server.ServerLogo = "";
            server.CommunityWebsite = "";
            server.ScoreLimit = 1000;
            server.TicketRatio =100;
            server.TeamRatio = 100;
            server.Team1Name = "HAMAS";
            server.Team2Name = "IDF";
            server.CoopEnabled = false;
            server.Pure = false;
            server.Unlocks = false;
            server.Fps = 32;
            server.Plasma = false;
            server.CoopBotRatio = 0;
            server.CoopBotCount = 0;
            server.CoopBotDifficulty = 0;
            server.NoVehicles = false;

            Team team1 = new Team()
            {
                Name = "HAMAS",
                Score = 0
            };

            Team team2 = new Team()
            {
                Name = "IDF",
                Score = 0
            };

            server.GetTeams()[0] = team1;
            server.GetTeams()[1] = team2;

            for (int i = 0; i < players; i++) {
                Player player = new Player("Name" + i, (uint)100000958)
                {
                    IsBot = false,
                    Team = (uint) (i < 35 ? 1 : 2),
                    Kills = 2,
                    Deaths = 4,
                    Score = 450,
                    Ping = 23
                };

                server.GetPlayers()[i] = player;
            }

            return server;
        }
    }
}
