// -*- coding: utf-8 -*-
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
public class RequestManager
{
	public string UUID;

	#region PUT
	public async Task<Player> CreatePlayer(string name)
	{
		string url = $"https://2025.nti-gamedev.ru/api/games/{UUID}/players/";
		using HttpClient client = new HttpClient();
		var request = new HttpRequestMessage(HttpMethod.Post, url);
		Player newPLayer = new Player();
		request.Content.Headers.Add("name", name);
		try
		{
			var response = client.SendAsync(request).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to create a new player. Status code: {response.StatusCode}");
			}
			var responceBody = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<Player>(responceBody);
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to create a new player. Check your internet connection. Error details: {e.Message}");
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
				throw new Exception($"Failed to create a new player. Status code: {response.StatusCode}");
			}
			var responceBody = await response.Content.ReadAsStringAsync();
			players = JsonConvert.DeserializeObject<List<Player>>(responceBody);

			return players;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to create a new player. Check your internet connection. Error details: {e.Message}");
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
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to get selected player. Status code: {response.StatusCode}");
			}
			var responceBody = await response.Content.ReadAsStringAsync();
			player = JsonConvert.DeserializeObject<Player>(responceBody);

			return player;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get selected player. Check your internet connection. Error details: {e.Message}");
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
			var response = client.GetAsync(url).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to get player's logs. Status code: {response.StatusCode}");
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get player's logs. Check your internet connection. Error details: {e.Message}");
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
				throw new Exception($"Failed to get player's shops. Status code: {response.StatusCode}");
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			shops = JsonConvert.DeserializeObject<List<Shop>>(responseBody);

			return shops;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get player's shops. Check your internet connection. Error details: {e.Message}");
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
				throw new Exception($"Failed to get player's shop. Status code: {response.StatusCode}");
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			shop = JsonConvert.DeserializeObject<Shop>(responseBody);

			return shop;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get player's shop. Check your internet connection. Error details: {e.Message}");
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
				throw new Exception($"Failed to get shop's logs. Status code: {response.StatusCode}");
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get shop's logs. Check your internet connection. Error details: {e.Message}");
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
				throw new Exception($"Failed to get logs. Status code: {response.StatusCode}");
			}
			var responseBody = await response.Content.ReadAsStringAsync();
			logs = JsonConvert.DeserializeObject<List<Log>>(responseBody);

			return logs;
		}
		catch (Exception e)
		{
			throw new Exception($"Failed to get logs. Check your internet connection. Error details: {e.Message}");
		}
	}
	#endregion

	#region POST

	#endregion

	#region DELETE

	#endregion
}
