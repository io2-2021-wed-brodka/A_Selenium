namespace ChromeDriverTests.Models
{
	public class StationResponse
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Status { get; set; }
		public int ActiveBikesCount { get; set; }
		public int BikesLimit { get; set; }
	}
}