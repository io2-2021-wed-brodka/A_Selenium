using System.Collections.Generic;

namespace PageModels.Models
{
	public class StationDetails
	{
		public string StationName { get; set; }

		public int BikesLimit { get; set; }

		public List<Malfunction> RelatedMalfunctions { get; set; }
	}
}