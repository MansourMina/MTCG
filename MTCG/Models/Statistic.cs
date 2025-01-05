namespace MTCG.Models
{
    public class Statistic
    {
        public string Id { get; private set; }

        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }

        public Statistic()
        {

        }
        public Statistic(string id, int wins, int losses, int draws)
        {
            Wins = wins;
            Losses = losses;
            Draws = draws;
            Id = id;
        }

        public void AddWin()
        {
            Wins++;
        }

        public void AddLosses()
        {
            Losses++;
        }

        public void AddDraw()
        {
            Draws++;
        }

        public void SetId(string id)
        {
            Id = id;
        }
    }
}
