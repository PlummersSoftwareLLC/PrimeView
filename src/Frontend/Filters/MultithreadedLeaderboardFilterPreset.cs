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
			AlgorithmText = new string[] { Constants.WheelTag, Constants.OtherTag }.JoinFilterValues();
			FaithfulText = Constants.UnfaithfulTag;
			BitsText = new string[] { Constants.UnknownTag, Constants.OtherTag }.JoinFilterValues();
		}

		public override bool IsFixed => true;
	}
}
