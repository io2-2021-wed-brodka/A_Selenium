using ChromeDriverTests.Backend;
using ChromeDriverTests.Models;
using ChromeDriverTests.Models.Bike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ChromeDriverTests.Models.Tech;
using PageModels.Models;

namespace ChromeDriverTests
{
    class SetupMethods
    {
        private static HttpClient httpClient = new HttpClient();
        private string backendUrl = string.Empty;
        private JsonSerializerOptions options;

        public string UserToken { get; set; }
        public string AdminToken { get; set; }

        public SetupMethods(string backendUrl)
        {
            this.backendUrl = backendUrl;
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true                
            };
        }

        public async Task<ICollection<StationResponse>> GetAllStations()
        {
            var userRegisterRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(backendUrl + "/stations")
            };
            userRegisterRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

            using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
            userRegisterResponse.EnsureSuccessStatusCode();
            using var userRegisterContent = userRegisterResponse.Content;
            var stationsResponse = await userRegisterContent.ReadFromJsonAsync<Stations>();
            return stationsResponse.stations;
        }

        public async Task<IDictionary<string, HashSet<string>>> GetAllBikes()
        {
            var userRegisterRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(backendUrl + "/bikes")
            };
            userRegisterRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

            using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
            userRegisterResponse.EnsureSuccessStatusCode();
            using var userRegisterContent = userRegisterResponse.Content;
            var stationsResponse = await userRegisterContent.ReadFromJsonAsync<Bikes>();
            return stationsResponse.bikes.GroupBy(b => b.Station.Name, b => b, (key, g) => new { stationName = key, bikes = g.Select(b => b.Id).ToHashSet() })
                .ToDictionary(k=> k.stationName, v => v.bikes);
        }


        public async Task RegisterUser(string userUsername, string userPassword)
        {
            var newUser = new
            {
                Login = userUsername,
                Password = userPassword
            };

            var userRegisterRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(newUser, options: options), Encoding.UTF8, "application/json"),
                RequestUri = new Uri(backendUrl + "/register")
            };

            using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
            userRegisterResponse.EnsureSuccessStatusCode();
            using var userRegisterContent = userRegisterResponse.Content;
            var registerResponse = await userRegisterContent.ReadFromJsonAsync<RegisterResponse>();

            UserToken = registerResponse.Token;
        }

        public async Task LoginAdmin(string adminUsername, string adminPassword)
        {
            var newUser = new
            {
                Login = adminUsername,
                Password = adminPassword
            };

            var userRegisterRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(newUser, options: options), Encoding.UTF8, "application/json"),
                RequestUri = new Uri(backendUrl + "/login")
            };

            using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
            userRegisterResponse.EnsureSuccessStatusCode();
            using var userRegisterContent = userRegisterResponse.Content;
            var registerResponse = await userRegisterContent.ReadFromJsonAsync<LoginResponse>();

            AdminToken = registerResponse.Token;
        }

        public async Task AddTech(string techUsername, string techPassword)
        {
            var newTech = new 
            {
                Name = techUsername,
                Password = techPassword
            };

            var userRegisterRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(newTech, options: options), Encoding.UTF8, "application/json"),
                RequestUri = new Uri(backendUrl + "/techs")
            };
            userRegisterRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

            using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
            userRegisterResponse.EnsureSuccessStatusCode();            
        }

        public async Task<ICollection<StationResponse>> AddStation(IEnumerable<string> stationNames)
        {
            var responses = new List<StationResponse>();
            foreach (var stationName in stationNames)
            {
                var newStation = new
                {
                    Name = stationName,
                };

                var userRegisterRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonSerializer.Serialize(newStation, options: options), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri(backendUrl + "/stations")
                };
                userRegisterRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

                using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
                userRegisterResponse.EnsureSuccessStatusCode();
                using var userRegisterContent = userRegisterResponse.Content;
                responses.Add(await userRegisterContent.ReadFromJsonAsync<StationResponse>(options));
            }
            return responses;
        }

        public async Task<BikeResponse> AddBike(string stationId)
        {   
            var newBike = new
            {
                StationId = stationId,
            };            

            var addBikeRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(newBike, options: options), Encoding.UTF8, "application/json"),
                RequestUri = new Uri(backendUrl + "/bikes")
            };
            addBikeRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

            using var addBikeResponse = await httpClient.SendAsync(addBikeRequestMessage);
            addBikeResponse.EnsureSuccessStatusCode();
            using var addBikeContent = addBikeResponse.Content;
            return await addBikeContent.ReadFromJsonAsync<BikeResponse>();
        }

		public async Task AddMalfunction(Malfunction malfunction)
		{
			var malfunctionRequest = new HttpRequestMessage()
			{
				Method = HttpMethod.Post,
				Content = new StringContent(
					JsonSerializer.Serialize(malfunction, options: options), Encoding.UTF8, "application/json"
				),
				RequestUri = new Uri(backendUrl + "/malfunctions")
			};
			malfunctionRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			var response = await httpClient.SendAsync(malfunctionRequest);
			response.EnsureSuccessStatusCode();
		}

		public async Task RentBike(string bikeId)
		{
			var rentBikeRequest = new HttpRequestMessage()
			{
				Method = HttpMethod.Post,
				Content = new StringContent(
					JsonSerializer.Serialize(new RentBikeRequest() { Id = bikeId }, options: options),
					Encoding.UTF8, "application/json"
				),
				RequestUri = new Uri(backendUrl + "/bikes/rented")
			};
			rentBikeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			var response = await httpClient.SendAsync(rentBikeRequest);
			response.EnsureSuccessStatusCode();
		}

		public async Task ReturnBike(string bikeId, string stationId)
		{
			var returnBikeRequest = new HttpRequestMessage()
			{
				Method = HttpMethod.Post,
				Content = new StringContent(
					JsonSerializer.Serialize(new RentBikeRequest() { Id = bikeId }, options: options),
					Encoding.UTF8, "application/json"
				),
				RequestUri = new Uri($"{backendUrl}/stations/{stationId}/bikes")
			};
			returnBikeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			var response = await httpClient.SendAsync(returnBikeRequest);
			response.EnsureSuccessStatusCode();
		}
    }
}
