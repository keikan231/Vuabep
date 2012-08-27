using System;

namespace CRS.Web.Models
{
    public class Rating
    {
        public int RateTimes { get; set; }
        public int MaxRate { get; set; }
        public double TotalRates { get; set; }
        public bool AllowRating { get; set; }
        public double Average
        {
            get { return RateTimes == 0 ? 0 : Math.Min(MaxRate, TotalRates / RateTimes); }
        }

        public Rating(int rateNumber, double totalScore, int maxRate = 5)
        {
            RateTimes = rateNumber;
            MaxRate = maxRate;
            TotalRates = totalScore;
        }
    }
}