namespace ChromeDriverTests.Models.Bike
{
	public class Bikes
	{
        public BikeResponse[] bikes { get; set; }
	}
    public class BikeResponse
    {
        public string Id { get; set; }
        public StationResponse Station { get; set; }
        public string Status { get; set; }
    }
}
