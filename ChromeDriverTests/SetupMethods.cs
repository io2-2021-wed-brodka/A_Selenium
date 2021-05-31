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
using System.Text.Json;
using System.Threading.Tasks;
using ChromeDriverTests.Models.Tech;

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

		public async Task RegisterUser(string userUsername, string userPassword)
		{
			var newUser = new LoginRequest
			{
				Login = userUsername,
				Password = userPassword
			};

			var userRegisterRequestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new StringContent(JsonSerializer.Serialize(newUser, options: options), Encoding.UTF8,
					"application/json"),
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
			var newUser = new LoginRequest
			{
				Login = adminUsername,
				Password = adminPassword
			};

			var userRegisterRequestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new StringContent(JsonSerializer.Serialize(newUser, options: options), Encoding.UTF8,
					"application/json"),
				RequestUri = new Uri(backendUrl + "/login")
			};

			using var userRegisterResponse = await httpClient.SendAsync(userRegisterRequestMessage);
			userRegisterResponse.EnsureSuccessStatusCode();
			using var userRegisterContent = userRegisterResponse.Content;
			var registerResponse = await userRegisterContent.ReadFromJsonAsync<LoginResponse>();

			AdminToken = registerResponse.Token;
		}

		public async Task<ICollection<StationResponse>> AddStation(IEnumerable<string> stationNames)
		{
			var responses = new List<StationResponse>();
			foreach (var stationName in stationNames)
			{
				var newStation = new StationRequest
				{
					Name = stationName,
				};

				var userRegisterRequestMessage = new HttpRequestMessage
				{
					Method = HttpMethod.Post,
					Content = new StringContent(JsonSerializer.Serialize(newStation, options: options), Encoding.UTF8,
						"application/json"),
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
			var newBike = new BikeRequest
			{
				StationId = stationId,
			};

			var addBikeRequestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new StringContent(JsonSerializer.Serialize(newBike, options: options), Encoding.UTF8,
					"application/json"),
				RequestUri = new Uri(backendUrl + "/bikes")
			};
			addBikeRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			using var addBikeResponse = await httpClient.SendAsync(addBikeRequestMessage);
			addBikeResponse.EnsureSuccessStatusCode();
			using var addBikeContent = addBikeResponse.Content;
			return await addBikeContent.ReadFromJsonAsync<BikeResponse>();
		}

		public async Task AddTech(string techName, string password)
		{
			var techData = new TechRequest()
			{
				Name = techName,
				Password = password
			};

			var addTechRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new StringContent(
					JsonSerializer.Serialize(techData, options: options), Encoding.UTF8, "application/json"
				),
				RequestUri = new Uri(backendUrl + "/techs")
			};
			addTechRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			using var response = await httpClient.SendAsync(addTechRequest);
			response.EnsureSuccessStatusCode();
		}

		public async Task<StationListResponse> GetAllStations()
		{
			var getStationsRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(backendUrl + "/stations")
			};
			getStationsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);

			using var stationsRespone = await httpClient.SendAsync(getStationsRequest);
			stationsRespone.EnsureSuccessStatusCode();

			return await stationsRespone.Content.ReadFromJsonAsync<StationListResponse>();
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
			return stationsResponse.bikes.GroupBy(
					b => b.Station.Name, b => b,
					(key, g) => new { stationName = key, bikes = g.Select(b => b.Id).ToHashSet() }
				)
				.ToDictionary(k => k.stationName, v => v.bikes);
		}
	}
}