namespace MTCG.Models
{
    public class Statistic
    {
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }

        public void addWin()
        {
            Wins++;
        }

        public void addLosses()
        {
            Losses++;
        }

        public void addDraw()
        {
            Draws++;
        }
    }
}
