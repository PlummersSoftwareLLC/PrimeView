using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Filters
{
	public class LeaderboardFilterPreset : ResultFilterPreset
	{
		public LeaderboardFilterPreset()
		{
			Name = "Leaderboard";
			ImplementationText = "";
			ParallelismText = "mt";
			AlgorithmText = "wh~ot";
			FaithfulText = "uf";
			BitsText = "uk~ot";
		}

		public override bool IsFixed => true;
	}
}
