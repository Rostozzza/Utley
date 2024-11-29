using System.Collections.Generic;

public class Log
{
	public string Comment { get; set; }
	public string PlayerName { get; set; }
	public string ShopName {  get; set; }
	public Dictionary<string, int> ResourcesChanged { get; set; }
}