// -*- coding: utf-8 -*-
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class RequestManager
{
	public string UUID = "85820b3e-e70b-4696-9954-dbed1d942244";
	public bool isAPIActive;

	public RequestManager(bool isAPIActive)
	{
		this.isAPIActive = isAPIActive;
	}

	#region PUT
	/// <summary>
	/// Sends request for updating specified player resources
	/// </summary>
	/// <param name="username"></param>
	/// <param name="resources"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Dictionary<string, string>> UpdatePlayerResources(string username, Dictionary<string, string> resources)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{username}/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json");
		Debug.Log(JsonConvert.SerializeObject(resources));
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to update player resources. Status code: {response.StatusCode}");
				return null;
			}

			var responseBody = await response.Content.ReadAsStringAsync();
			var responseResources = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
			return responseResources;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to update player resources. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	public async Task<Dictionary<string, string>> UpdatePlayerResources(string username, string shopName, Dictionary<string, string> resources)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{username}/shops/{shopName}/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(resources), Encoding.UTF8, "application/json");
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to update player's shop resources. Status code: {response.StatusCode}");
				return null;
			}

			var responseBody = await response.Content.ReadAsStringAsync();
			var responseResources = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
			return responseResources;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to update player's shop resources. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}
	#endregion

	#region GET

	/// <summary>
	/// Returns list of all registered players
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<List<Player>> GetAllPlayers()
	{
		List<Player> players = new List<Player>();
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/";
		HttpClient client = new HttpClient();
		try
		{
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to create a new player. Status code: {response.StatusCode}");
				return null;
			}
			var responceBody = await response.Content.ReadAsStringAsync();
			players = JsonConvert.DeserializeObject<List<Player>>(responceBody);
			return players;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to create a new player. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns a specific registered player by name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Player> GetPlayer(string name)
	{
		Player player = new Player();
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/";
		HttpClient client = new HttpClient();
		try
		{
			var response = await client.GetAsync(url);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get selected player. Status code: {response.StatusCode}");
				return null;
			}
			var responceBody = await response.Content.ReadAsStringAsync();
			player = JsonConvert.DeserializeObject<Player>(responceBody);

			return player;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get selected player. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns player's logs if present.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<List<Log>> GetPlayerLogs(string name)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/";
		List<Log> logs = new List<Log>();
		HttpClient client = new HttpClient();
		try
		{
			var response = await client.GetAsync(url);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get player's logs. Status code: {response.StatusCode}");
				return null;
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get player's logs. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns all shops belonging to the player
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<List<Shop>> GetPlayerShops(string name)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/";
		List<Shop> shops = new List<Shop>();
		HttpClient client = new HttpClient();

		try
		{
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get player's shops. Status code: {response.StatusCode}");
				return null;
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			shops = JsonConvert.DeserializeObject<List<Shop>>(responseBody);

			return shops;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get player's shops. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns a shop belonging to the player by name
	/// </summary>
	/// <param name="name"></param>
	/// <param name="shopName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Shop> GetPlayerShop(string name, string shopName)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/{shopName}/";
		Shop shop = new Shop();
		HttpClient client = new HttpClient();
		try
		{
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get player's shop. Status code: {response.StatusCode}");
				return null;
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			shop = JsonConvert.DeserializeObject<Shop>(responseBody);

			return shop;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get player's shop. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns all logs belonging to a selected player
	/// </summary>
	/// <param name="name"></param>
	/// <param name="shopName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<List<Log>> GetShopLogs(string name, string shopName)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{name}/shops/{shopName}/logs/";
		List<Log> logs = new List<Log>();
		HttpClient client = new HttpClient();
		try
		{
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get shop's logs. Status code: {response.StatusCode}");
				return null;
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get shop's logs. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Returns all logs
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<List<Log>> GetAllLogs()
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/";
		List<Log> logs = new List<Log>();
		HttpClient client = new HttpClient();
		try
		{
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to get logs. Status code: {response.StatusCode}");
				return null;
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to get logs. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}
	#endregion

	#region POST
	/// <summary>
	/// Creates a request for an API to create an player by name. Returns an instance of class "Player"
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Player> CreatePlayer(Player newPlayer)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(newPlayer), Encoding.UTF8, "application/json");
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to create a new player. Status code: {response.StatusCode}. model: {response.Content.ReadAsStringAsync().Result}");
				return null;
			}

			var responseBody = await response.Content.ReadAsStringAsync();
			newPlayer = JsonConvert.DeserializeObject<Player>(responseBody);
			return newPlayer;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to create a new player. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Creates a request for an API to create new log;
	/// </summary>
	/// <param name="comment"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Log> CreateLog(Log log)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(log), Encoding.UTF8, "application/json");
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to create a new log. Status code: {response.StatusCode}");
				return null;
			}
			Log responceLog;
			var responseBody = await response.Content.ReadAsStringAsync();
			responceLog = JsonConvert.DeserializeObject<Log>(responseBody);
			return responceLog;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to create a new log. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Creates shop for an selected player;
	/// </summary>
	/// <param name="username"></param>
	/// <param name="shop"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Shop> CreateShop(string username, Shop shop)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{username}/shops/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(shop), Encoding.UTF8, "application/json");
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to create a new shop. Status code: {response.StatusCode}");
				return null;
			}
			Shop responceLog;
			var responseBody = await response.Content.ReadAsStringAsync();
			responceLog = JsonConvert.DeserializeObject<Shop>(responseBody);
			Mathf.PerlinNoise1D(Time.time);
			return responceLog;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to create a new shop. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}

	/// <summary>
	/// Creates player-tied log;
	/// </summary>
	/// <param name="playerLog"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Log> CreateShopLog(Log playerLog)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
		request.Content = new StringContent(JsonConvert.SerializeObject(playerLog), Encoding.UTF8, "application/json");
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to create a new log. Status code: {response.StatusCode}");
				return null;
			}
			Log responceLog;
			var responseBody = await response.Content.ReadAsStringAsync();
			responceLog = JsonConvert.DeserializeObject<Log>(responseBody);
			return responceLog;
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to create a new log. Check your internet connection. Error details: {e.Message}");
			return null;
		}
	}
	#endregion

	#region DELETE
	public async Task DeletePlayer(string username)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{username}/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to delete player. Status code: {response.StatusCode}");
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to delete player. Check your internet connection. Error details: {e.Message}");
		}
	}

	public async Task DeletePlayerShop(string username, string shopName)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{username}/shops/{shopName}/";
		using HttpClient client = new HttpClient();
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
		try
		{
			var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Debug.Log($"Failed to delete player's shop. Status code: {response.StatusCode}");
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to delete player's shop. Check your internet connection. Error details: {e.Message}");
		}
	}
	#endregion

	//DO NOT FUCKING USE THIS FUNCTION! OR ELSE I WILL FIND YOU AND YOUR ENTIER BLOODLINE WILL BE EXTERMINATED :3
	public async Task<string> GeenrateUUID()
	{
		var teamName = "BoyZBand";
		var talantId = "51123";
		var apiUrl = "https://2025.nti-gamedev.ru/api/games/";
		var nonce = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
		var signatureInput = talantId + nonce;
		string signature;
		using (var md5 = MD5.Create())
		{
			var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(signatureInput));
			signature = BitConverter.ToString(hash).Replace("-", "").ToLower();
		}
		using var client = new HttpClient();
		var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		request.Headers.Add("Nonce", nonce);
		request.Headers.Add("Talant-Id", talantId);
		request.Headers.Add("Signature", signature);
		var requestBody = new
		{
			team_name = teamName
		};
		string jsonBody = JsonConvert.SerializeObject(requestBody);
		request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
		var response = await client.SendAsync(request);
		return response.Content.ReadAsStringAsync().Result;
	}
}
