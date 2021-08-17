using PrimeView.Frontend.Tools;

namespace PrimeView.Frontend.Filters
{
	public class LeaderboardFilterPreset : ResultFilterPreset
	{
		public LeaderboardFilterPreset()
		{
			Name = "Leaderboard";
			ImplementationText = string.Empty;
			ParallelismText = Constants.MultithreadedTag;
			AlgorithmText = new string[] { Constants.WheelTag, Constants.OtherTag }.JoinFilterValues();
			FaithfulText = Constants.UnfaithfulTag;
			BitsText = new string[] { Constants.UnknownTag, Constants.OtherTag }.JoinFilterValues();
		}

		public override bool IsFixed => true;
	}
}
