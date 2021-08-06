namespace PrimeView.Frontend.Filters
{
	public class MultithreadedLeaderboardFilterPreset : ResultFilterPreset
	{
		public MultithreadedLeaderboardFilterPreset()
		{
			Name = "Multithreaded leaderboard";
			ImplementationText = "";
			ParallelismText = "st";
			AlgorithmText = "wh~ot";
			FaithfulText = "uf";
			BitsText = "uk~ot";
		}

		public override bool IsFixed => true;
	}
}
