namespace GSEmulator.Model
{
    class Team
    {
        public static readonly int NumFields = 2;
        public static readonly string NAME_KEY = "team_t";
        public static readonly string SCORE_KEY = "score_t";

        public string Name { get; set; }
        public int Score { get; set; }

        public string GetField(int fieldIdx)
        {
            switch (fieldIdx)
            {
                case 0:
                    return Name;
                case 1:
                    return Score.ToString();
                default:
                    return "";
            }
        }

        public static string GetFieldName(int fieldIdx)
        {
            switch (fieldIdx)
            {
                case 0:
                    return NAME_KEY;
                case 1:
                    return SCORE_KEY;
                default:
                    return "";
            }
        }
    }
}
