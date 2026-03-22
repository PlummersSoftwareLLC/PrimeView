using PrimeView.Frontend.Tools;

namespace PrimeView.Frontend.Filters
{
    public class MultithreadedLeaderboardFilterPreset : ResultFilterPreset
    {
        public MultithreadedLeaderboardFilterPreset()
        {
            Name = "Multithreaded leaderboard";
            ImplementationText = string.Empty;
            ParallelismText = Constants.SinglethreadedTag;
            AlgorithmText = new[] { Constants.WheelTag, Constants.OtherTag }.JoinFilterValues();
            FaithfulText = Constants.UnfaithfulTag;
            BitsText = new[] { Constants.UnknownTag, Constants.OtherTag }.JoinFilterValues();
        }

        public override bool IsFixed => true;
    }
}
