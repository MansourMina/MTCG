﻿namespace MTCG.Services
{
    public class StatisticService
    {
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }

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
    }
}
